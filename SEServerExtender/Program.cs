namespace SEServerExtender
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceProcess;
	using System.Threading;
	using System.Windows.Forms;
	using NLog;
	using NLog.Targets;
	using SEModAPI.Support;
	using SEModAPIExtensions.API;
	using SEModAPIInternal.API.Chat;
	using SEModAPIInternal.API.Common;

	public static class Program
	{
		private static int _maxChatHistoryMessageAge = 3600;
		private static int _maxChatHistoryMessageCount = 100;
		public static readonly Logger ChatLog = LogManager.GetLogger( "ChatLog" );
		public static readonly Logger BaseLog = LogManager.GetLogger( "BaseLog" );
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
				BaseLog.Info( "Starting SEServerExtender Service with {0} arguments ...", args.Length );

				Start( args );
			}

			protected override void OnStop( )
			{
				BaseLog.Info( "Stopping SEServerExtender Service...." );

				Program.Stop( );
			}
		}

		internal static SEServerExtender ServerExtenderForm;
		internal static Server Server;
		public static ServiceHost ServerServiceHost;
		internal static CommandLineArgs CommandLineArgs;

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		static void Main( string[ ] args )
		{
			FileTarget baseLogTarget = LogManager.Configuration.FindTargetByName( "BaseLog" ) as FileTarget;
			if ( baseLogTarget != null )
			{
				baseLogTarget.FileName = baseLogTarget.FileName.Render( new LogEventInfo { TimeStamp = DateTime.Now } );
			}

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

			BaseLog.Info( "Starting SEServerExtender with {0} arguments ...", args.Length );

			CommandLineArgs extenderArgs = CommandLineArgs = new CommandLineArgs
							  {
								  AutoStart = false,
								  WorldName = string.Empty,
								  InstanceName = string.Empty,
								  NoGui = false,
								  NoConsole = false,
								  Debug = false,
								  GamePath = Directory.GetParent( Directory.GetCurrentDirectory( ) ).FullName,
								  NoWcf = false,
								  Autosave = 0,
								  Path = string.Empty,
								  CloseOnCrash = false,
								  RestartOnCrash = false,
								  Args = string.Join( " ", args.Select( x => string.Format( "\"{0}\"", x ) ) )
							  };

			if ( ConfigurationManager.AppSettings[ "WCFChatMaxMessageHistoryAge" ] != null )
				if ( !int.TryParse( ConfigurationManager.AppSettings[ "WCFChatMaxMessageHistoryAge" ], out _maxChatHistoryMessageAge ) )
				{
					ConfigurationManager.AppSettings.Add( "WCFChatMaxMessageHistoryAge", "3600" );
				}
			if ( ConfigurationManager.AppSettings[ "WCFChatMaxMessageHistoryCount" ] != null )
				if ( !int.TryParse( ConfigurationManager.AppSettings[ "WCFChatMaxMessageHistoryCount" ], out _maxChatHistoryMessageCount ) )
				{
					ConfigurationManager.AppSettings.Add( "WCFChatMaxMessageHistoryCount", "100" );
				}

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
						//extenderArgs.AutoStart = true;
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

				Server = Server.Instance;
				Server.CommandLineArgs = extenderArgs;
				Server.IsWCFEnabled = !extenderArgs.NoWcf;
				Server.Init( );

				ChatManager.ChatCommand guiCommand = new ChatManager.ChatCommand( "gui", ChatCommand_GUI, false );
				ChatManager.Instance.RegisterChatCommand( guiCommand );

				if ( extenderArgs.AutoStart )
				{
					Server.StartServer( );
				}

				if ( !extenderArgs.NoWcf )
				{
					BaseLog.Info( "Opening up WCF service listener" );
					ServerServiceHost = new ServiceHost( typeof( ServerService.ServerService ) );
					ServerServiceHost.Open( );
					ChatManager.Instance.ChatMessage += ChatManager_ChatMessage;
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
					BaseLog.Info( "AutoException - {0}\n\r{1}", eEx.AdditionnalInfo, eEx.GetDebugString( ) );
				if ( !extenderArgs.NoGui )
					MessageBox.Show( string.Format( "{0}\n\r{1}", eEx.AdditionnalInfo, eEx.GetDebugString( ) ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error );

				if ( extenderArgs.NoConsole && extenderArgs.NoGui )
					throw eEx.GetBaseException( );
			}
			catch ( TargetInvocationException ex )
			{
				if ( !extenderArgs.NoConsole )
					BaseLog.Info( "TargetInvocationException - {0}\n\r{1}", ex, ex.InnerException );
				if ( !extenderArgs.NoGui )
					MessageBox.Show( string.Format( "{0}\n\r{1}", ex, ex.InnerException ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error );

				if ( extenderArgs.NoConsole && extenderArgs.NoGui )
					throw;
			}
			catch ( Exception ex )
			{
				if ( !extenderArgs.NoConsole )
					BaseLog.Info( "Exception - {0}", ex );
				if ( !extenderArgs.NoGui )
					MessageBox.Show( ex.ToString( ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error );

				if ( extenderArgs.NoConsole && extenderArgs.NoGui )
					throw;
			}
		}

		private static void ChatManager_ChatMessage( ulong userId, string playerName, string message )
		{
			lock ( ChatSessionManager.SessionsMutex )
				foreach ( KeyValuePair<Guid, ChatSession> s in ChatSessionManager.Instance.Sessions )
				{
					s.Value.Messages.Add( new ChatMessage
										 {
											 Message = message,
											 Timestamp = DateTimeOffset.Now,
											 User = playerName,
											 UserId = userId
										 } );
					if ( s.Value.Messages.Count > _maxChatHistoryMessageCount )
						s.Value.Messages.RemoveAt( 0 );
					while ( s.Value.Messages.Any( ) && ( DateTimeOffset.Now - s.Value.Messages[ 0 ].Timestamp ).TotalSeconds > _maxChatHistoryMessageAge )
						s.Value.Messages.RemoveAt( 0 );
				}
		}

		private static void Stop( )
		{
			if ( Server != null && Server.IsRunning )
				Server.StopServer( );
			if ( ServerExtenderForm != null && ServerExtenderForm.Visible )
				ServerExtenderForm.Close( );

			if ( Server.ServerThread != null )
			{
				Server.ServerThread.Join( 20000 );
			}
			if ( ServerServiceHost != null )
				ServerServiceHost.Close( );
		}

		public static void Application_ThreadException( Object sender, ThreadExceptionEventArgs e )
		{
			BaseLog.Error( "Application Thread Exception", e.Exception );
		}

		public static void AppDomain_UnhandledException( Object sender, UnhandledExceptionEventArgs e )
		{
			BaseLog.Error( "AppDomain.UnhandledException - {0}", e.ExceptionObject );
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
			if ( ServerExtenderForm == null || ServerExtenderForm.IsDisposed )
				ServerExtenderForm = new SEServerExtender( Server );
			else if ( ServerExtenderForm.Visible )
				return;

			Application.Run( ServerExtenderForm );
		}
	}
}
