using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using Havok;
using Sandbox.Audio;
using Sandbox.Input;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;
using SteamSDK;
using SysUtils.Utils;
using VRage.Common.Plugins;
using VRage.Common.Utils;

namespace SEModAPIInternal.API.Server
{
	public class ServerAssemblyWrapper
	{
		#region "Attributes"

		private static ServerAssemblyWrapper m_instance;
		private static Assembly m_assembly;
		private static AppDomain m_domain;

		public static string DedicatedServerNamespace = "83BCBFA49B3A2A6EC1BC99583DA2D399";
		public static string DedicatedServerClass = "49BCFF86BA276A9C7C0D269C2924DE2D";

		public static string DedicatedServerStartupBaseMethod = "26A7ABEA729FAE1F24679E21470F8E98";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected ServerAssemblyWrapper( )
		{
			m_instance = this;

			string assemblyPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "SpaceEngineersDedicated.exe" );
			m_assembly = Assembly.UnsafeLoadFrom( assemblyPath );

			/*
			byte[] b = File.ReadAllBytes(assemblyPath);
			Assembly rawServerAssembly = Assembly.Load(b);
			m_domain = AppDomain.CreateDomain("Server Domain");
			m_assembly = m_domain.Load(rawServerAssembly.GetName());
			*/

			Console.WriteLine( "Finished loading ServerAssemblyWrapper" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static AppDomain ServerDomain
		{
			get
			{
				return m_domain;
			}
		}

		public static ServerAssemblyWrapper Instance
		{
			get
			{
				if ( m_instance == null )
					m_instance = new ServerAssemblyWrapper( );

				return m_instance;
			}
		}

		public static Type InternalType
		{
			get
			{
				if ( m_assembly == null )
				{
					byte[ ] b = File.ReadAllBytes( "SpaceEngineersDedicated.exe" );
					m_assembly = Assembly.Load( b );
				}

				Type dedicatedServerType = m_assembly.GetType( DedicatedServerNamespace + "." + DedicatedServerClass );
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
				Console.WriteLine( ex );
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

			try
			{
				MyPlugins.Unload( );
			}
			catch { }

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
				//Make sure the log, if running, is closed out before we begin
				if ( MyLog.Default != null )
					MyLog.Default.Close( );

				SandboxGameAssemblyWrapper.Instance.SetNullRender( true );
				MyFileSystem.Reset( );

				//Prepare the parameters
				bool isUsingInstance = false;
				if ( instanceName != "" )
					isUsingInstance = true;
				object[ ] methodParams = new object[ ]
				{
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
				LogManager.APILog.WriteLine( "Win32Exception - Server crashed" );

				LogManager.APILog.WriteLine( ex );
				LogManager.APILog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				LogManager.ErrorLog.WriteLine( Environment.StackTrace );

				return false;
			}
			catch ( ExternalException ex )
			{
				LogManager.APILog.WriteLine( "ExternalException - Server crashed" );

				LogManager.APILog.WriteLine( ex );
				LogManager.APILog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				LogManager.ErrorLog.WriteLine( Environment.StackTrace );

				return false;
			}
			catch ( TargetInvocationException ex )
			{
				LogManager.APILog.WriteLine( "TargetInvocationException - Server crashed" );

				LogManager.APILog.WriteLine( ex );
				LogManager.APILog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				LogManager.ErrorLog.WriteLine( Environment.StackTrace );

				return false;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( "Exception - Server crashed" );

				LogManager.APILog.WriteLine( ex );
				LogManager.APILog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				LogManager.ErrorLog.WriteLine( Environment.StackTrace );

				return false;
			}
			/*
		finally
		{
			m_instance = null;
			Reset();
			if (m_domain != null)
			{
				AppDomain.Unload(m_domain);
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
				Console.WriteLine("Took " + cleanupTime.TotalSeconds.ToString() + " seconds to clean up entities");
				*/
				Object mainGame = SandboxGameAssemblyWrapper.MainGame;
				BaseObject.InvokeEntityMethod( mainGame, "Dispose" );

				/*
				Reset();
				AppDomain.Unload(m_domain);
				m_domain = null;
				 */
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}