using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Linq;

using SEModAPI.Support;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using SEModAPIExtensions.API;

namespace SEServerExtender
{
	public static class Program
	{
		public class WindowsService : ServiceBase
		{
			public WindowsService( )
			{
				ServiceName = "SEServerExtender";
				CanPauseAndContinue = false;
				CanStop = true;
				AutoLog = true;
			}

			protected override void OnStart( string[ ] args )
			{
				LogManager.APILog.WriteLine( string.Format( "Starting SEServerExtender Service with {0} arguments ...", args.Length ) );

				Start( args );
			}

			protected override void OnStop( )
			{
				LogManager.APILog.WriteLine( "Stopping SEServerExtender Service..." );

				Program.Stop( );
			}
		}

		static SEServerExtender _serverExtenderForm;
		static Server _server;

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		static void Main( string[ ] args )
		{
			if ( !Environment.UserInteractive )
			{
				using ( var service = new WindowsService( ) )
				{
					ServiceBase.Run( service );
				}
			}
			else
			{
				Start( args );
			}
		}

		private static void Start( string[ ] args )
		{
			//Setup error handling for unmanaged exceptions
			AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );

			//AppDomain.CurrentDomain.ClearEventInvocations("_unhandledException");

			LogManager.APILog.WriteLine( string.Format( "Starting SEServerExtender with {0} arguments ...", args.Length ) );

			CommandLineArgs extenderArgs = new CommandLineArgs
										   {
											   AutoStart = false,
											   WorldName = string.Empty,
											   InstanceName = string.Empty,
											   NoGui = false,
											   NoConsole = false,
											   Debug = false,
											   GamePath = string.Empty,
											   NoWcf = false,
											   Autosave = 0,
											   WcfPort = 0,
											   Path = string.Empty,
											   CloseOnCrash = false,
											   RestartOnCrash = false,
											   Args = string.Join( " ", args.Select( x => string.Format( "\"{0}\"", x ) ) )
										   };

			//Setup the default args

			//Process the args
			foreach ( string arg in args )
			{
				if ( arg.Split( '=' ).Length > 1 )
				{
					string argName = arg.Split( '=' )[ 0 ];
					string argValue = arg.Split( '=' )[ 1 ];

					if ( argName.ToLower( ).Equals( "instance" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 0, argValue.Length - 1 );
						extenderArgs.InstanceName = argValue;
					}
					else if ( argName.ToLower( ).Equals( "gamepath" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 0, argValue.Length - 1 );
						extenderArgs.GamePath = argValue;
					}
					else if ( argName.ToLower( ).Equals( "autosave" ) )
					{
						try
						{
							extenderArgs.Autosave = int.Parse( argValue );
						}
						catch
						{
							//Do nothing
						}
					}
					else if ( argName.ToLower( ).Equals( "wcfport" ) )
					{
						try
						{
							extenderArgs.WcfPort = ushort.Parse( argValue );
						}
						catch
						{
							//Do nothing
						}
					}
					else if ( argName.ToLower( ).Equals( "path" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 0, argValue.Length - 1 );
						extenderArgs.Path = argValue;
					}
				}
				else
				{
					if ( arg.ToLower( ).Equals( "autostart" ) )
					{
						extenderArgs.AutoStart = true;
					}
					if ( arg.ToLower( ).Equals( "nogui" ) )
					{
						extenderArgs.NoGui = true;

						//Implies autostart
						extenderArgs.AutoStart = true;
					}
					if ( arg.ToLower( ).Equals( "noconsole" ) )
					{
						extenderArgs.NoConsole = true;

						//Implies nogui and autostart
						extenderArgs.NoGui = true;
						extenderArgs.AutoStart = true;
					}
					if ( arg.ToLower( ).Equals( "debug" ) )
					{
						extenderArgs.Debug = true;
					}
					if ( arg.ToLower( ).Equals( "nowcf" ) )
					{
						extenderArgs.NoWcf = true;
					}
					if ( arg.ToLower( ).Equals( "closeoncrash" ) )
					{
						extenderArgs.CloseOnCrash = true;
					}
					if ( arg.ToLower( ).Equals( "autosaveasync" ) )
					{
						extenderArgs.AutoSaveSync = false;
					}
					if ( arg.ToLower( ).Equals( "autosavesync" ) )
					{
						extenderArgs.AutoSaveSync = true;
					}
					if ( arg.ToLower( ).Equals( "restartoncrash" ) )
					{
						extenderArgs.RestartOnCrash = true;
					}
					if ( arg.ToLower( ).Equals( "wrr" ) )
					{
						extenderArgs.WorldRequestReplace = true;
					}
					if ( arg.ToLower( ).Equals( "wrm" ) )
					{
						extenderArgs.WorldDataModify = true;
					}
				}
			}

			if ( extenderArgs.NoWcf )
				extenderArgs.WcfPort = 0;

			if ( !string.IsNullOrEmpty( extenderArgs.Path ) )
			{
				extenderArgs.InstanceName = string.Empty;
			}

			if ( !Environment.UserInteractive )
			{
				extenderArgs.NoConsole = true;
				extenderArgs.NoGui = true;
				extenderArgs.AutoStart = true;
			}

			if ( extenderArgs.Debug )
				SandboxGameAssemblyWrapper.IsDebugging = true;

			try
			{
				bool unitTestResult = BasicUnitTestManager.Instance.Run( );
				if ( !unitTestResult )
					SandboxGameAssemblyWrapper.IsInSafeMode = true;

				_server = Server.Instance;
				_server.CommandLineArgs = extenderArgs;
				_server.IsWCFEnabled = !extenderArgs.NoWcf;
				_server.WCFPort = extenderArgs.WcfPort;
				_server.Init( );

				ChatManager.ChatCommand guiCommand = new ChatManager.ChatCommand { Command = "gui", Callback = ChatCommand_GUI };
				ChatManager.Instance.RegisterChatCommand( guiCommand );

				if ( extenderArgs.AutoStart )
				{
					_server.StartServer( );
				}

				if ( !extenderArgs.NoGui )
				{
					Thread uiThread = new Thread( StartGui );
					uiThread.SetApartmentState( ApartmentState.STA );
					uiThread.Start( );
				}
				else if ( Environment.UserInteractive )
					Console.ReadLine( );
			}
			catch ( AutoException eEx )
			{
				if ( !extenderArgs.NoConsole )
					Console.WriteLine( "AutoException - {0}\n\r{1}", eEx.AdditionnalInfo, eEx.GetDebugString( ) );
				if ( !extenderArgs.NoGui )
					MessageBox.Show( string.Format( "{0}\n\r{1}", eEx.AdditionnalInfo, eEx.GetDebugString( ) ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error );

				if ( extenderArgs.NoConsole && extenderArgs.NoGui )
					throw eEx.GetBaseException( );
			}
			catch ( TargetInvocationException ex )
			{
				if ( !extenderArgs.NoConsole )
					Console.WriteLine( "TargetInvocationException - {0}\n\r{1}", ex, ex.InnerException );
				if ( !extenderArgs.NoGui )
					MessageBox.Show( string.Format( "{0}\n\r{1}", ex, ex.InnerException ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error );

				if ( extenderArgs.NoConsole && extenderArgs.NoGui )
					throw;
			}
			catch ( Exception ex )
			{
				if ( !extenderArgs.NoConsole )
					Console.WriteLine( "Exception - {0}", ex );
				if ( !extenderArgs.NoGui )
					MessageBox.Show( ex.ToString( ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error );

				if ( extenderArgs.NoConsole && extenderArgs.NoGui )
					throw;
			}
		}

		private static void Stop( )
		{
			if ( _server != null && _server.IsRunning )
				_server.StopServer( );
			if ( _serverExtenderForm != null && _serverExtenderForm.Visible )
				_serverExtenderForm.Close( );

			if ( _server.ServerThread != null )
			{
				_server.ServerThread.Join( 20000 );
			}
		}

		public static void Application_ThreadException( Object sender, ThreadExceptionEventArgs e )
		{
			Console.WriteLine( "Application.ThreadException - {0}", e.Exception );

			if ( LogManager.APILog != null && LogManager.APILog.LogEnabled )
			{
				LogManager.APILog.WriteLine( "Application.ThreadException" );
				LogManager.APILog.WriteLine( e.Exception );
			}
			if ( LogManager.ErrorLog != null && LogManager.ErrorLog.LogEnabled )
			{
				LogManager.ErrorLog.WriteLine( "Application.ThreadException" );
				LogManager.ErrorLog.WriteLine( e.Exception );
			}
		}

		public static void AppDomain_UnhandledException( Object sender, UnhandledExceptionEventArgs e )
		{
			Console.WriteLine( "AppDomain.UnhandledException - {0}", e.ExceptionObject );

			if ( LogManager.APILog != null && LogManager.APILog.LogEnabled )
			{
				LogManager.APILog.WriteLine( "AppDomain.UnhandledException" );
				LogManager.APILog.WriteLine( (Exception)e.ExceptionObject );
			}
			if ( LogManager.ErrorLog != null && LogManager.ErrorLog.LogEnabled )
			{
				LogManager.ErrorLog.WriteLine( "AppDomain.UnhandledException" );
				LogManager.ErrorLog.WriteLine( (Exception)e.ExceptionObject );
			}
		}

		static void ChatCommand_GUI( ChatManager.ChatEvent chatEvent )
		{
			Thread uiThread = new Thread( StartGui );
			uiThread.SetApartmentState( ApartmentState.STA );
			uiThread.Start( );
		}

		[STAThread]
		static void StartGui( )
		{
			if ( !Environment.UserInteractive )
				return;

			Application.EnableVisualStyles( );
			Application.SetCompatibleTextRenderingDefault( false );
			if ( _serverExtenderForm == null || _serverExtenderForm.IsDisposed )
				_serverExtenderForm = new SEServerExtender( _server );
			else if ( _serverExtenderForm.Visible )
				return;

			Application.Run( _serverExtenderForm );
		}
	}
}
