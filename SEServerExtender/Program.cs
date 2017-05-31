using System.Collections;
using System.ComponentModel;
using System.Management;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Definitions;
using Sandbox.Engine.Multiplayer;
using Sandbox.Engine.Platform;
using Sandbox.Engine.Utils;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using SEModAPI.API.Definitions;
using SEModAPI.API.Utility;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;
using SpaceEngineers.Game;
using VRage.FileSystem;
using VRage.Game;
using VRage.Game.ObjectBuilder;
using VRage.Plugins;
using VRage.Utils;
using Game = Sandbox.Engine.Platform.Game;

namespace SEServerExtender
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
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
    using VRage;
    using VRage.ObjectBuilders;
    public static class Program
	{
		private static int _maxChatHistoryMessageAge = 3600;
		private static int _maxChatHistoryMessageCount = 100;
		public static readonly Logger ChatLog = LogManager.GetLogger( "ChatLog" );
		public static readonly Logger BaseLog = LogManager.GetLogger( "BaseLog" );
		public static readonly Logger PluginLog = LogManager.GetLogger( "PluginLog" );
        public static Version SeVersion;
        public static bool IsStable;

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
        
        /// <summary>
        /// Manually initializes definitions and other stuff we need to load the DS config serializer.
        /// Touch this and die.
        /// </summary>
        private static void InitSandbox()
        {
            FileSystem.InitMyFileSystem();

            //Infinario should be disabled on DS
            MyFakes.ENABLE_INFINARIO = false;

            MyLog.Default = MySandboxGame.Log;
            MySandboxGame.Config = new MyConfig("SpaceEngineers.cfg");
            MySandboxGame.Config.Load();
            SpaceEngineersGame.SetupPerGameSettings();
            SpaceEngineersGame.SetupBasicGameInfo();

            //all this is basically copied from the MySandboxGame ctor
            //manually initializing this stuff prevents Steam from detecting SESE as an instance of the game
            MyPlugins.RegisterGameAssemblyFile(MyPerGameSettings.GameModAssembly);
            if (MyPerGameSettings.GameModBaseObjBuildersAssembly != null)
                MyPlugins.RegisterBaseGameObjectBuildersAssemblyFile(MyPerGameSettings.GameModBaseObjBuildersAssembly);
            MyPlugins.RegisterGameObjectBuildersAssemblyFile(MyPerGameSettings.GameModObjBuildersAssembly);
            MyPlugins.RegisterSandboxAssemblyFile(MyPerGameSettings.SandboxAssembly);
            MyPlugins.RegisterSandboxGameAssemblyFile(MyPerGameSettings.SandboxGameAssembly);
            MyPlugins.RegisterFromArgs(null);
            MyPlugins.Load();
            MyGlobalTypeMetadata.Static.Init();
            //MyDefinitionManager.Static.PrepareBaseDefinitions();
            //MyDefinitionManager.Static.PreloadDefinitions();
            //MyDefinitionManager.Static.LoadScenarios();
            //MyTutorialHelper.Init();
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(MyObjectBuilder_Base).TypeHandle);
        }

        private static void HideConfigs(params string[] names)
        {
            foreach (var name in names)
                SetBrowsable(name, false);
        }

        //Shamelessly stolen from StackOverflow
        private static void SetBrowsable(string name, bool value)
        {
            var desc = TypeDescriptor.GetProperties(typeof(DedicatedConfigDefinition))[name];
            var attrib = desc.Attributes[typeof(BrowsableAttribute)];
            var brows = attrib.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
            brows.SetValue(attrib, value);
        }

        private static void Start( string[ ] args )
        {
            // SE_VERSION is a private constant. Need to use reflection to get it. 
            FieldInfo field = typeof(SpaceEngineersGame).GetField("SE_VERSION", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            SeVersion = new Version(new MyVersion((int)field.GetValue(null)).FormattedText.ToString().Replace("_", "."));

		    //bool stableBuild = (bool)typeof(MyFinalBuildConstants).GetField("IS_STABLE").GetValue(null);

            ApplicationLog.BaseLog.Info($"SE version: {SeVersion}");

            string versionMessage = $"Extender version: {Assembly.GetExecutingAssembly().GetName().Version}";
#if DEBUG
            //mark debug builds as such
            versionMessage += " DEBUG";
#endif
            ApplicationLog.BaseLog.Info( versionMessage );


            //warn users about SESE EOL and Torch migration
            if (SystemInformation.UserInteractive)
            {
                Task.Run(() =>
                         {
                             AutoClosingMessageBox.Show("Support for SESE is ENDING. Ongoing compatability is not guaranteed. All servers should migrate to" +
                                             "Torch (SESE2) when it becomes available. Join our discord server to stay updated on Torch's status: " +
                                             "https://discord.gg/JPrcsWt or check regularly at https://github.com/TorchAPI/Torch",
                                             "SESE EOL",
                                             20000);
                         });
            }
            BaseLog.Warn("Support for SESE is ENDING. Ongoing compatability is not guaranteed. All servers should migrate to" +
                                             "Torch (SESE2) when it becomes available. Join our discord server to stay updated on Torch's status: " +
                                             "https://discord.gg/JPrcsWt or check regularly at https://github.com/TorchAPI/Torch");




            /*
            if (stableBuild)
            {
                BaseLog.Info("Detected \"Stable\" branch!");
                //BaseLog.Error("WARNING: THIS BUILD OF SESE IS NOT COMPATIBLE WITH STABLE BRANCH");
		        //if (SystemInformation.UserInteractive)
		        //{
		        //    var result = MessageBox.Show("This build of SESE is not compatible with stable branch!",
		        //                                 "Fatal Error",
		        //                                 MessageBoxButtons.OK,
		        //                                 MessageBoxIcon.Error);
		        //    if (result == DialogResult.OK)
		        //    {
		        //        Stop();
		        //        return;
		        //    }
		        //}
		        //else
		        //{
		        //    Stop();
		        //    return;
		        //}
		        IsStable = true;
		        PluginManager.IsStable = true;
		        Server.IsStable = true;
            }
            else
                BaseLog.Info("Detected \"Development\" branch!");
                */
            InitSandbox();

            //Setup error handling for unmanaged exceptions
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );

			//AppDomain.CurrentDomain.ClearEventInvocations("_unhandledException");
            
			BaseLog.Info( "Starting SEServerExtender with {0} arguments: {1}", args.Length, string.Join( "\r\n\t", args ) );

			CommandLineArgs extenderArgs = CommandLineArgs = new CommandLineArgs
							  {
                                  ConsoleTitle = string.Empty,
								  AutoStart = false,
								  WorldName = string.Empty,
								  InstanceName = string.Empty,
								  NoGui = false,
								  NoConsole = false,
								  Debug = false,
								  GamePath = new DirectoryInfo( PathManager.BasePath ).Parent.FullName,
                                  //TODO: turn noWFC back to off by default whenever WCF gets fixed
								  NoWcf = true,
								  Autosave = 0,
								  InstancePath = string.Empty,
								  CloseOnCrash = false,
								  RestartOnCrash = false,
                                  NoProfiler = false,
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
                        //sanitize input because stupid people put full paths for this argument
						extenderArgs.InstanceName = argValue.Replace( @"\", "-" ).Replace( @":", "-" );

						//Only let this override log path if the log path wasn't already explicitly set
						if ( !logPathSet )
						{
							FileTarget baseLogTarget = LogManager.Configuration.FindTargetByName( "BaseLog" ) as FileTarget;
							if ( baseLogTarget != null )
							{
                                baseLogTarget.FileName = baseLogTarget.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now }).Replace("NoInstance", argValue.Replace(@"\", "-").Replace(@":", "-"));
							}
							FileTarget chatLogTarget = LogManager.Configuration.FindTargetByName( "ChatLog" ) as FileTarget;
							if ( chatLogTarget != null )
							{
                                chatLogTarget.FileName = chatLogTarget.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now }).Replace("NoInstance", argValue.Replace(@"\", "-").Replace(@":", "-"));
							}
							FileTarget pluginLogTarget = LogManager.Configuration.FindTargetByName( "PluginLog" ) as FileTarget;
							if ( pluginLogTarget != null )
							{
                                pluginLogTarget.FileName = pluginLogTarget.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now }).Replace("NoInstance", argValue.Replace(@"\", "-").Replace(@":", "-"));
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
                    else if (lowerCaseArgument.Equals("title") )
                    {
                        if (argValue[argValue.Length - 1] == '"')
                            argValue = argValue.Substring(1, argValue.Length - 2);
                        extenderArgs.ConsoleTitle = argValue;
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
						extenderArgs.NoWcf = true;
					}
                    else if ( lowerCaseArgument.Equals( "wcfon" ) )
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
                    else if (lowerCaseArgument.Equals("noprofiler") && !IsStable)
                    {
                        extenderArgs.NoProfiler = true;
                        Server.DisableProfiler = true;
                    }
                    //these things are legacy and don't work anyway
                    /*
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
                    */
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

                //if(!DedicatedServerAssemblyWrapper.IsStable)
                //    InitSandbox(Path.Combine( GameInstallationInfo.GamePath, @"..\Content"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineers"));

                ChatManager.ChatCommand guiCommand = new ChatManager.ChatCommand( "gui", ChatCommand_GUI, false );
				ChatManager.Instance.RegisterChatCommand( guiCommand );

                if (!CommandLineArgs.NoConsole)
			    {
			        if (string.IsNullOrEmpty(extenderArgs.ConsoleTitle) || string.IsNullOrWhiteSpace(extenderArgs.ConsoleTitle))
			        {
			            Console.Title = "SESE";
			        }
			        else
			        {
			            Console.Title = extenderArgs.ConsoleTitle;
			        }
			    }

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

    //lifted verbatim from StackOverflow
    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
