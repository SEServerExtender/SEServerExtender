using System.Linq;
using SEModAPI.API;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Plugins;
using VRage.Utils;

namespace SEModAPIInternal.API.Server
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Reflection;
	using System.Runtime.ExceptionServices;
	using System.Runtime.InteropServices;
	using System.Security;
	using Havok;
	using Sandbox;
	using Sandbox.Game;
	using SEModAPI.API.Sandbox;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;
	using SpaceEngineers.Game;
	using SteamSDK;
	using VRage.Audio;
	using VRage.Dedicated;
	using VRage.FileSystem;
	using VRage.Input;

	public class DedicatedServerAssemblyWrapper
	{
		private static DedicatedServerAssemblyWrapper _instance;
		//private static Assembly _assembly;

		public const string DedicatedServerNamespace = "VRage.Dedicated";
		public const string DedicatedServerClass = "DedicatedServer";
		public const string DedicatedServerRunMainMethod = "RunMain";

	    public static bool IsStable;

		#region "Constructors and Initializers"

		protected DedicatedServerAssemblyWrapper( )
		{
			_instance = this;

			//string assemblyPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "VRage.Dedicated.dll" );
			//_assembly = Assembly.UnsafeLoadFrom( assemblyPath );

			ApplicationLog.BaseLog.Info( "Finished loading DedicatedServerAssemblyWrapper" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static DedicatedServerAssemblyWrapper Instance
		{
			get { return _instance ?? ( _instance = new DedicatedServerAssemblyWrapper( ) ); }
		}

		public static Type InternalType
		{
			get
			{
				return typeof( DedicatedServer );
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type dedicatedServerType = InternalType;
				if ( dedicatedServerType == null )
					throw new Exception( "Could not find internal type for DedicatedServerAssemblyWrapper" );
				bool result = true;
				result &= Reflection.HasMethod( dedicatedServerType, DedicatedServerRunMainMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		private void SteamReset( )
		{
			if ( SteamServerAPI.Instance != null && SteamServerAPI.Instance.GameServer != null && SteamServerAPI.Instance.GameServer.GameDescription != null )
			{
				try
				{
					SteamServerAPI.Instance.GameServer.Dispose( );
				}
				catch ( Exception )
				{
					//Do nothing
				}
			}
			if ( SteamAPI.Instance != null )
			{
				SteamAPI.Instance.Dispose( );
			}
		}

		private void InputReset( )
		{
			try
			{
				/*
				if ( MyGuiGameControlsHelpers.GetGameControlHelper( MyGameControlEnums.FORWARD ) != null )
				{
					//MyGuiGameControlsHelpers.UnloadContent( );
				}
				 */
			}
			catch ( Exception )
			{
				//Do nothing
			}

			if ( MyInput.Static != null )
				MyInput.Static.UnloadData( );
		}

		private void PhysicsReset( )
		{
			try
			{
				//TODO - Find out the proper way to get Havok to clean everything up so we don't get pointer errors on the next start
				//HkBaseSystem.Quit();
				HkBaseSystem.Quit( );
			}
			catch ( Exception )
			{
				//Do nothing for now
			}
		}

		private void Reset( )
		{
			SteamReset( );

			/*
			if (MyAPIGateway.Session != null)
			{
				MyAPIGateway.Session.UnloadDataComponents();
				MyAPIGateway.Session.UnloadMultiplayer();
				MyAPIGateway.Session.Unload();
			}
			*/

			MyAudio.Static.UnloadData( );
			MyAudio.UnloadData( );
			MyFileSystem.Reset( );

			InputReset( );

			PhysicsReset( );
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		public bool StartServer( string instanceName = "", string overridePath = "", bool useConsole = true )
		{
			try
			{
				Sandbox.Engine.Platform.Game.IsDedicated = true;
				MyFileSystem.Reset( );

				//Prepare the parameters
				bool isUsingInstance = instanceName != string.Empty;
				object[ ] methodParams = {
					instanceName,
					overridePath,
					isUsingInstance,
					useConsole
				};

				//Initialize config
				SpaceEngineersGame.SetupPerGameSettings();
			    SpaceEngineersGame.SetupBasicGameInfo();
                if(MyFinalBuildConstants.APP_VERSION == null) //KEEN WHAT THE FUCK
                    MyFinalBuildConstants.APP_VERSION = MyPerGameSettings.BasicGameInfo.GameVersion; 
                MyPerGameSettings.SendLogToKeen = DedicatedServer.SendLogToKeen;
				MyPerServerSettings.GameName = MyPerGameSettings.GameName;
				MyPerServerSettings.GameNameSafe = MyPerGameSettings.GameNameSafe;
				MyPerServerSettings.GameDSName = MyPerServerSettings.GameNameSafe + "Dedicated";
				MyPerServerSettings.GameDSDescription = "Your place for space engineering, destruction and exploring.";
				MyPerServerSettings.AppId = 0x3bc72;
                
                //Start the server
                MethodInfo dedicatedServerRunMainMethod = InternalType.GetMethod( DedicatedServerRunMainMethod, BindingFlags.Static | BindingFlags.NonPublic );
				dedicatedServerRunMainMethod.Invoke( null, methodParams );
			    ApplicationLog.BaseLog.Info( MyLog.Default.GetFilePath());

                return true;
			}
            /* these are all redundant
			catch ( Win32Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );

				return false;
			}
			catch ( ExternalException ex )
			{
				ApplicationLog.BaseLog.Error( ex );

				return false;
			}
			catch ( TargetInvocationException ex )
			{
                //Generally, we won't log this, since it will always be thrown on server stop.
                //if ( ExtenderOptions.IsDebugging )
                    ApplicationLog.BaseLog.Error( ex );
                //ApplicationLog.BaseLog.Trace( ex );

                return false;
			}
            */
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );

				return false;
			}
			/*
		finally
		{
			_instance = null;
			Reset();
			if (_domain != null)
			{
				AppDomain.Unload(_domain);
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
			 */
		}

		public void StopServer( )
		{
			try
			{
				/*
				DateTime startedEntityCleanup = DateTime.Now;
				foreach (BaseEntity entity in SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>())
				{
					entity.Dispose();
				}
				TimeSpan cleanupTime = DateTime.Now - startedEntityCleanup;
				ApplicationLog.BaseLog.Debug("Took " + cleanupTime.TotalSeconds.ToString() + " seconds to clean up entities");
				*/
				MySandboxGame.Static.Exit( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}