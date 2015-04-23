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
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;
	using SteamSDK;
	using VRage.Audio;
	using VRage.FileSystem;
	using VRage.Input;

	public class ServerAssemblyWrapper
	{
		private static ServerAssemblyWrapper _instance;
		private static Assembly _assembly;
		//private static AppDomain _domain;

		public static string DedicatedServerNamespace = "";
		public static string DedicatedServerClass = "Sandbox.AppCode.App.MyProgram";

		public static string DedicatedServerStartupBaseMethod = "RunMain";

		#region "Constructors and Initializers"

		/// <exception cref="SecurityException">A codebase that does not start with "file://" was specified without the required <see cref="T:System.Net.WebPermission" />. </exception>
		/// <exception cref="BadImageFormatException">Not a valid assembly. -or- assembly was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="FileLoadException">A file that was found could not be loaded. </exception>
		/// <exception cref="FileNotFoundException">Assembly is not found, or the module you are trying to load does not specify a filename extension. </exception>
		/// <exception cref="PathTooLongException">The assembly name is longer than MAX_PATH characters.</exception>
		/// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain. </exception>
		protected ServerAssemblyWrapper( )
		{
			_instance = this;

			string assemblyPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "SpaceEngineersDedicated.exe" );
			_assembly = Assembly.UnsafeLoadFrom( assemblyPath );

			ApplicationLog.BaseLog.Info( "Finished loading ServerAssemblyWrapper" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static ServerAssemblyWrapper Instance
		{
			get { return _instance ?? ( _instance = new ServerAssemblyWrapper( ) ); }
		}

		public static Type InternalType
		{
			get
			{
				if ( _assembly == null )
				{
					byte[ ] b = File.ReadAllBytes( "SpaceEngineersDedicated.exe" );
					_assembly = Assembly.Load( b );
				}

				Type dedicatedServerType = _assembly.GetType( DedicatedServerNamespace + "." + DedicatedServerClass );
				return dedicatedServerType;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = InternalType;
				if ( type1 == null )
					throw new Exception( "Could not find internal type for ServerAssemblyWrapper" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, DedicatedServerStartupBaseMethod );

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
				SandboxGameAssemblyWrapper.Instance.SetNullRender( true );
				MyFileSystem.Reset( );

				//Prepare the parameters
				bool isUsingInstance = instanceName != string.Empty;
				object[ ] methodParams = {
					instanceName,
					overridePath,
					isUsingInstance,
					useConsole
				};

				//Start the server
				MethodInfo serverStartupMethod = InternalType.GetMethod( DedicatedServerStartupBaseMethod, BindingFlags.Static | BindingFlags.NonPublic );
				serverStartupMethod.Invoke( null, methodParams );

				return true;
			}
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
				if ( ApplicationLog.BaseLog.IsTraceEnabled )
					ApplicationLog.BaseLog.Trace( ex );

				return false;
			}
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
				Object mainGame = SandboxGameAssemblyWrapper.MainGame;
				BaseObject.InvokeEntityMethod( mainGame, "Dispose" );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}