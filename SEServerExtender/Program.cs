using System.Collections;
using System.Management;
using SEModAPIInternal.Support;

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
	using NLog.Layouts;
	using NLog.Targets;
	using SEModAPI.API;
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
		public static readonly Logger PluginLog = LogManager.GetLogger( "PluginLog" );
		public class WindowsService : ServiceBase
		{
			public WindowsService( )
			{
				CanPauseAndContinue = false;
				CanStop = true;
				AutoLog = true;


			}

			protected override void OnStart( string[ ] args )
			{
				BaseLog.Info( "Starting SEServerExtender Service with {0} arguments: {1}", args.Length, string.Join( "\r\n\t", args ) );

			    List<string> listArg = args.ToList();
			    string serviceName = string.Empty;
                string gamePath = new DirectoryInfo(PathManager.BasePath).Parent.FullName;

                // Instance autodetect
			    if (args.All(item => !item.Contains("instance")))
			    {
                    BaseLog.Info( "No instance specified, guessing it ...");
			        int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
			        String query = "SELECT Name FROM Win32_Service where ProcessId = " + processId;
			        ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
			        ManagementObjectCollection collection = searcher.Get();
			        IEnumerator enumerator = collection.GetEnumerator();
			        enumerator.MoveNext();
			        ManagementObject managementObject = (ManagementObject) enumerator.Current;

			        serviceName = managementObject["Name"].ToString();
                    BaseLog.Info( "Instance detected : {0}", serviceName);
                    listArg.Add("instance=" + serviceName);
			    }

                // gamepath autodetect
                if (args.All(item => !item.Contains("gamepath")))
                {
                    BaseLog.Info("No gamepath specified, guessing it ...");
                    
                    BaseLog.Info("gamepath detected : {0}", gamePath);
                    listArg.Add("gamepath=\"" + gamePath + "\"");
                }

                // It's a service, it's mandatory to use noconsole (nogui and autostart implied)
			    if (args.All(item => !item.Contains("noconsole")))
			    {
                    BaseLog.Info("Service Startup, noconsole is mandatory, adding it ...");
                    listArg.Add("noconsole");
			    }

                // It's a service, storing the logs in the instace directly
                if (args.All(item => !item.Contains("logpath")) && !String.IsNullOrWhiteSpace(serviceName))
			    {
                    listArg.Add("logpath=\"C:\\ProgramData\\SpaceEngineersDedicated\\" + serviceName + "\"");
			    }
                if (args.All(item => !item.Contains("instancepath")) && !String.IsNullOrWhiteSpace(serviceName))
                {
                    listArg.Add("instancepath=\"C:\\ProgramData\\SpaceEngineersDedicated\\" + serviceName + "\"");
                }

			    Start( listArg.ToArray() );
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
            //register object builder assembly
            string path = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "SpaceEngineers.ObjectBuilders.DLL" );
            VRage.Plugins.MyPlugins.RegisterGameObjectBuildersAssemblyFile( path );

            //Setup error handling for unmanaged exceptions
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );

			//AppDomain.CurrentDomain.ClearEventInvocations("_unhandledException");

			BaseLog.Info( "Starting SEServerExtender with {0} arguments: {1}", args.Length, string.Join( "\r\n\t", args ) );

			CommandLineArgs extenderArgs = CommandLineArgs = new CommandLineArgs
							  {
								  AutoStart = false,
								  WorldName = string.Empty,
								  InstanceName = string.Empty,
								  NoGui = false,
								  NoConsole = false,
								  Debug = false,
								  GamePath = new DirectoryInfo( PathManager.BasePath ).Parent.FullName,
								  NoWcf = true,
								  Autosave = 0,
								  InstancePath = string.Empty,
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

			bool logPathSet = false;
			//Process the args
			foreach ( string arg in args )
			{
				string[ ] splitAtEquals = arg.Split( '=' );
				if ( splitAtEquals.Length > 1 )
				{
					string argName = splitAtEquals[ 0 ];
					string argValue = splitAtEquals[ 1 ];

					string lowerCaseArgument = argName.ToLower( );
					if ( lowerCaseArgument.Equals( "instance" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 1, argValue.Length - 2 );
						extenderArgs.InstanceName = argValue;

						//Only let this override log path if the log path wasn't already explicitly set
						if ( !logPathSet )
						{
							FileTarget baseLogTarget = LogManager.Configuration.FindTargetByName( "BaseLog" ) as FileTarget;
							if ( baseLogTarget != null )
							{
								baseLogTarget.FileName = baseLogTarget.FileName.Render( new LogEventInfo { TimeStamp = DateTime.Now } ).Replace( "NoInstance", argValue );
							}
							FileTarget chatLogTarget = LogManager.Configuration.FindTargetByName( "ChatLog" ) as FileTarget;
							if ( chatLogTarget != null )
							{
								chatLogTarget.FileName = chatLogTarget.FileName.Render( new LogEventInfo { TimeStamp = DateTime.Now } ).Replace( "NoInstance", argValue );
							}
							FileTarget pluginLogTarget = LogManager.Configuration.FindTargetByName( "PluginLog" ) as FileTarget;
							if ( pluginLogTarget != null )
							{
								pluginLogTarget.FileName = pluginLogTarget.FileName.Render( new LogEventInfo { TimeStamp = DateTime.Now } ).Replace( "NoInstance", argValue );
							}
						}
					}
					else if ( lowerCaseArgument.Equals( "gamepath" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 1, argValue.Length - 2 );
						extenderArgs.GamePath = argValue;
					}
					else if ( lowerCaseArgument.Equals( "autosave" ) )
					{
						if ( !int.TryParse( argValue, out extenderArgs.Autosave ) )
							BaseLog.Warn( "Autosave parameter was not a valid integer." );
					}
					else if ( lowerCaseArgument.Equals( "path" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 1, argValue.Length - 2 );
						extenderArgs.InstancePath = argValue;
					}
					else if ( lowerCaseArgument.Equals( "instancepath" ) )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 1, argValue.Length - 2 );
						extenderArgs.InstancePath = argValue;
					}
					else if ( lowerCaseArgument == "logpath" )
					{
						if ( argValue[ argValue.Length - 1 ] == '"' )
							argValue = argValue.Substring( 1, argValue.Length - 2 );

						//This argument always prevails.
						FileTarget baseLogTarget = LogManager.Configuration.FindTargetByName( "BaseLog" ) as FileTarget;
						if ( baseLogTarget != null )
						{
							Layout l = new SimpleLayout( Path.Combine( argValue, "SEServerExtenderLog-${shortdate}.log" ) );
							baseLogTarget.FileName = l.Render( new LogEventInfo { TimeStamp = DateTime.Now } );
							ApplicationLog.BaseLog = BaseLog;
						}
						FileTarget chatLogTarget = LogManager.Configuration.FindTargetByName( "ChatLog" ) as FileTarget;
						if ( chatLogTarget != null )
						{
							Layout l = new SimpleLayout( Path.Combine( argValue, "ChatLog-${shortdate}.log" ) );
							chatLogTarget.FileName = l.Render( new LogEventInfo { TimeStamp = DateTime.Now } );
							ApplicationLog.ChatLog = ChatLog;
						}
						FileTarget pluginLogTarget = LogManager.Configuration.FindTargetByName( "PluginLog" ) as FileTarget;
						if ( pluginLogTarget != null )
						{
							Layout l = new SimpleLayout( Path.Combine( argValue, "PluginLog-${shortdate}.log" ) );
							pluginLogTarget.FileName = l.Render( new LogEventInfo { TimeStamp = DateTime.Now } );
							logPathSet = true;
							ApplicationLog.PluginLog = PluginLog;
						}

					}
				}
				else
				{
					string lowerCaseArgument = arg.ToLower( );
					if ( lowerCaseArgument.Equals( "autostart" ) )
					{
						extenderArgs.AutoStart = true;
					}
					else if ( lowerCaseArgument.Equals( "nogui" ) )
					{
						extenderArgs.NoGui = true;

						//Implies autostart
						//extenderArgs.AutoStart = true;
					}
					else if ( lowerCaseArgument.Equals( "noconsole" ) )
					{
						extenderArgs.NoConsole = true;

						//Implies nogui and autostart
						extenderArgs.NoGui = true;
						extenderArgs.AutoStart = true;
					}
					else if ( lowerCaseArgument.Equals( "debug" ) )
					{
						extenderArgs.Debug = true;
					}
					else if ( lowerCaseArgument.Equals( "nowcf" ) )
					{
						extenderArgs.NoWcf = false;
					}
					else if ( lowerCaseArgument.Equals( "closeoncrash" ) )
					{
						extenderArgs.CloseOnCrash = true;
					}
					else if ( lowerCaseArgument.Equals( "autosaveasync" ) )
					{
						extenderArgs.AutoSaveSync = false;
					}
					else if ( lowerCaseArgument.Equals( "autosavesync" ) )
					{
						extenderArgs.AutoSaveSync = true;
					}
					else if ( lowerCaseArgument.Equals( "restartoncrash" ) )
					{
						extenderArgs.RestartOnCrash = true;
					}
					else if ( lowerCaseArgument.Equals( "wrr" ) )
					{
						extenderArgs.WorldRequestReplace = true;
					}
					else if ( lowerCaseArgument.Equals( "wrm" ) )
					{
						extenderArgs.WorldDataModify = true;
					}
                    else if (lowerCaseArgument.Equals("wvm"))
                    {
                        extenderArgs.WorldVoxelModify = true;
                    }
				}
			}

			if ( !Environment.UserInteractive )
			{
				extenderArgs.NoConsole = true;
				extenderArgs.NoGui = true;
				extenderArgs.AutoStart = true;
			}

			if ( extenderArgs.Debug )
				ExtenderOptions.IsDebugging = true;

			try
			{
				bool unitTestResult = BasicUnitTestManager.Instance.Run( );
				if ( !unitTestResult )
					ExtenderOptions.IsInSafeMode = true;

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
					string uriString = string.Format( "{0}{1}", ConfigurationManager.AppSettings[ "WCFServerServiceBaseAddress" ], CommandLineArgs.InstanceName );
					BaseLog.Info( "Opening up WCF service listener at {0}", uriString );
					ServerServiceHost = new ServiceHost( typeof( ServerService.ServerService ), new Uri( uriString, UriKind.Absolute ) );
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
					BaseLog.Info( ex, "Exception - {0}", ex );
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
			BaseLog.Error( e.Exception, "Application Thread Exception" );
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
