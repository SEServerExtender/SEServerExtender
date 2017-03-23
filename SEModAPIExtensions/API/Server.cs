using System.Reflection;
using System.Text;
using SteamSDK;
using VRage;
using VRage.Game;
using VRage.Plugins;

namespace SEModAPIExtensions.API
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.ExceptionServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.ServiceModel;
    using System.Threading;
    using System.Windows.Forms;
    using NLog;
    using NLog.Targets;
    using Sandbox;
    using Sandbox.Common.ObjectBuilders;
    using SEModAPI.API;
    using SEModAPI.API.Definitions;
    using SEModAPI.API.Sandbox;
    using SEModAPI.API.Utility;
    using SEModAPIInternal.API.Chat;
    using SEModAPIInternal.API.Common;
    using SEModAPIInternal.API.Server;
    using SEModAPIInternal.Support;
    using VRage.ObjectBuilders;
    using Timer = System.Timers.Timer;

    [DataContract]
	public class Server
    {
        public static bool DisableProfiler;
        public static bool IsStable;
		private static Server _instance;
		private static bool _isInitialized;
		private static Thread _runServerThread;
		private static bool _isServerRunning;
		private static bool _serverRan;
		private static int _restartLimit;
		private static bool _isWcfEnabled;
		private static FileSystemWatcher _cfgWatch;

		private CommandLineArgs _commandLineArgs;
		private DedicatedConfigDefinition _dedicatedConfigDefinition;
		private GameInstallationInfo _gameInstallationInfo;
		private ServiceHost _serverHost;
		private ServiceHost _baseHost;

		//Managers
		private PluginManager _pluginManager;
		//private FactionsManager _factionsManager;
		private DedicatedServerAssemblyWrapper _dedicatedServerWrapper;
		private LogManager _logManager;
		private EntityEventManager _entityEventManager;
		private ChatManager _chatManager;
		private SessionManager _sessionManager;

		//Timers
		private readonly Timer _pluginMainLoop;
		private readonly Timer _autosaveTimer;

		#region "Constructors and Initializers"

		protected Server( )
		{
			if ( _isInitialized )
				return;

			FileTarget baseLogTarget = LogManager.Configuration.FindTargetByName( "BaseLog" ) as FileTarget;
			if ( baseLogTarget != null )
			{
				baseLogTarget.FileName = baseLogTarget.FileName.Render( new LogEventInfo { TimeStamp = DateTime.Now } );
			}

			_restartLimit = 3;

			_pluginMainLoop = new Timer { Interval = 200 };
			_pluginMainLoop.Elapsed += PluginManagerMain;

			_autosaveTimer = new Timer { Interval = 300000 };
			_autosaveTimer.Elapsed += AutoSaveMain;

			_isWcfEnabled = true;

			ApplicationLog.BaseLog.Info( "Finished creating server!" );
		}


		//private bool SetupServerService()
		//{
		//	try
		//	{
		//		_serverHost = CreateServiceHost( typeof( ServerService ), typeof( IServerServiceContract ), "Server/", "ServerService" );
		//		_serverHost.Open( );
		//	}
		//	catch (CommunicationException ex)
		//	{
		//		ApplicationLog.BaseLog.Error("An exception occurred: " + ex.Message);
		//		_serverHost.Abort( );
		//		return false;
		//	}
		//	catch ( TimeoutException ex )
		//	{
		//		ApplicationLog.BaseLog.Error( "An exception occurred: " + ex.Message );
		//		_serverHost.Abort( );
		//		return false;
		//	}

		//	return true;
		//}

		//private bool SetupMainService()
		//{
		//	try
		//	{
		//		_baseHost = CreateServiceHost( typeof ( InternalService ), typeof ( IInternalServiceContract ), "", "InternalService" );
		//		_baseHost.Open( );
		//	}
		//	catch ( CommunicationException ex )
		//	{
		//		ApplicationLog.BaseLog.Error( "An exception occurred: " + ex.Message );
		//		_baseHost.Abort( );
		//		return false;
		//	}
		//	catch ( TimeoutException ex )
		//	{
		//		ApplicationLog.BaseLog.Error( "An exception occurred: " + ex.Message );
		//		_baseHost.Abort( );
		//		return false;
		//	}

		//	return true;
		//}

		//private bool SetupWebService()
		//{
		//Uri webServiceAddress = new Uri( "http://localhost:" + WCFPort + "/SEServerExtender/Web/" );
		//_webHost = new WebServiceHost( typeof( WebService ), webServiceAddress );
		//try
		//{
		//	//WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
		//	//binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
		//	//binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
		//	//ServiceEndpoint endpoint = _webHost.AddServiceEndpoint(typeof(IWebServiceContract), binding, "WebService");
		//	//_webHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
		//	//_webHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserNameValidator();
		//	//EnableCorsBehavior ecb = new EnableCorsBehavior();
		//	//_webHost.Description.Behaviors.Add( ecb );
		//	//_webHost.Open( );
		//}
		//catch (CommunicationException ex)
		//{
		//	ApplicationLog.BaseLog.Error( "An exception occurred: {0}", ex.Message);
		//	//_webHost.Abort( );
		//	return false;
		//}

		//return true;
		//}

		private bool SetupGameInstallation( )
		{
			try
			{
				string gamePath = _commandLineArgs.GamePath;
				if ( gamePath.Length > 0 )
				{
					if ( !GameInstallationInfo.IsValidGamePath( gamePath ) )
					{
						ApplicationLog.BaseLog.Fatal( "{0} is not a valid game path.", gamePath );

						return false;
					}
					_gameInstallationInfo = new GameInstallationInfo( gamePath );
				}
				else
				{
					_gameInstallationInfo = new GameInstallationInfo( );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			if ( _gameInstallationInfo != null )
				return true;
			else
				return false;
		}

		private bool SetupManagers( )
		{
			_dedicatedServerWrapper = DedicatedServerAssemblyWrapper.Instance;
			_pluginManager = PluginManager.Instance;
			//_factionsManager = FactionsManager.Instance;
			_entityEventManager = EntityEventManager.Instance;
			_chatManager = ChatManager.Instance;
			_sessionManager = SessionManager.Instance;

			return true;
		}

		private bool ProcessCommandLineArgs( )
		{
			try
			{
				if ( _commandLineArgs.AutoStart )
				{
					ApplicationLog.BaseLog.Info( "Auto-Start enabled" );
				}
				if ( _commandLineArgs.InstanceName.Length != 0 )
				{
					ApplicationLog.BaseLog.Info( "Common instance pre-selected: '" + _commandLineArgs.InstanceName + "'" );
				}
				if ( _commandLineArgs.NoGui )
				{
					ApplicationLog.BaseLog.Info( "GUI disabled" );
				}
				if ( _commandLineArgs.Debug )
				{
					ApplicationLog.BaseLog.Info( "Debugging enabled" );
					ExtenderOptions.IsDebugging = true;
				}
				if ( _commandLineArgs.NoWcf )
				{
					ApplicationLog.BaseLog.Info( "WCF disabled" );
				}
				if ( _commandLineArgs.Autosave > 0 )
				{
					ApplicationLog.BaseLog.Info( "Autosave interval: " + _commandLineArgs.Autosave );
				}
				if ( _commandLineArgs.CloseOnCrash )
				{
					ApplicationLog.BaseLog.Info( "Close On Crash: Enabled" );
				}
				if ( _commandLineArgs.AutoSaveSync )
				{
					ApplicationLog.BaseLog.Info( "Synchronous Save: Enabled" );
				}
				if ( _commandLineArgs.InstancePath.Length != 0 )
				{
					ApplicationLog.BaseLog.Info( "Full path pre-selected: '" + _commandLineArgs.InstancePath + "'" );
				}
				if ( _commandLineArgs.RestartOnCrash )
				{
					ApplicationLog.BaseLog.Info( "Restart On Crash: Enabled" );
				}
				if ( _commandLineArgs.WorldRequestReplace )
				{
					ApplicationLog.BaseLog.Info( "World Request Replace: Enabled" );
				}
                if (_commandLineArgs.ConsoleTitle.Length != 0 )
                {
                    ApplicationLog.BaseLog.Info("Console title set to '" + _commandLineArgs.ConsoleTitle + "'");
                }
                ApplicationLog.BaseLog.Info("end parsing");
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}

			return true;
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		public static Server Instance
		{
			get { return _instance ?? ( _instance = new Server( ) ); }
		}

		[DataMember]
		public bool IsRunning
		{
			get { return _isServerRunning; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		public bool ServerHasRan
		{
			get { return _serverRan; }
			set { _serverRan = value; }
		}

		[DataMember]
		public CommandLineArgs CommandLineArgs
		{
			get { return _commandLineArgs; }
			set { _commandLineArgs = value; }
		}

		[IgnoreDataMember]
		public DedicatedConfigDefinition Config
		{
			get { return _dedicatedConfigDefinition; }
			set { _dedicatedConfigDefinition = value; }
		}

		[DataMember]
		public string InstanceName
		{
			get { return _commandLineArgs.InstanceName; }
			set { _commandLineArgs.InstanceName = value; }
		}

		[DataMember]
		public double AutosaveInterval
		{
			get
			{
				_commandLineArgs.Autosave = (int)Math.Round( _autosaveTimer.Interval / 60000.0 );
				return _autosaveTimer.Interval;
			}
			set
			{
				_autosaveTimer.Interval = value;

				if ( _autosaveTimer.Interval <= 0 )
					_autosaveTimer.Interval = 300000;

				_commandLineArgs.Autosave = (int)Math.Round( _autosaveTimer.Interval / 60000.0 );
			}
		}

		[IgnoreDataMember]
		public bool IsWCFEnabled
		{
			get { return _isWcfEnabled; }
			set
			{
				if ( _isWcfEnabled == value ) return;
				_isWcfEnabled = value;

				if ( _isWcfEnabled )
				{
					if ( _serverHost != null )
						_serverHost.Open( );
					if ( _baseHost != null )
						_baseHost.Open( );
				}
				else
				{
					if ( _serverHost != null )
						_serverHost.Close( );
					if ( _baseHost != null )
						_baseHost.Close( );
				}
			}
		}

		[IgnoreDataMember]
		public string Path
		{
			get
			{
				string path = _commandLineArgs.InstancePath;
				if ( string.IsNullOrEmpty( path ) )
				{
					if ( InstanceName.Length != 0 )
					{
						ExtenderOptions.UseCommonProgramData = true;
						FileSystem.InitMyFileSystem( InstanceName, false );
					}
					path = FileSystem.GetUserDataPath( InstanceName );
				}

				return path;
			}
			set { _commandLineArgs.InstancePath = value; }
		}

        [IgnoreDataMember]
        public string Title
        {
            get
            {
                string title = _commandLineArgs.ConsoleTitle;
                return title;
            }
            set { _commandLineArgs.ConsoleTitle = value; }
        }

        [IgnoreDataMember]
		public Thread ServerThread
		{
			get { return _runServerThread; }
		}

		#endregion

		#region "Methods"

		public void Init( )
		{
			if ( _isInitialized )
				return;

			bool setupResult = true;
			setupResult &= SetupGameInstallation( );
			setupResult &= SetupManagers( );
			setupResult &= ProcessCommandLineArgs( );

			if ( _commandLineArgs.Autosave > 0 )
				_autosaveTimer.Interval = _commandLineArgs.Autosave * 60000;

			if ( !setupResult )
			{
				ApplicationLog.BaseLog.Error( "Failed to initialize server" );
				return;
			}

			_isInitialized = true;
			_serverRan = false;
		}

		private void PluginManagerMain( object sender, EventArgs e )
		{
			if ( !Instance.IsRunning )
			{
				_pluginMainLoop.Stop( );
				return;
			}

			if ( _pluginManager == null )
			{
				_pluginMainLoop.Stop( );
				return;
			}

			if ( !_pluginManager.Initialized && !_pluginManager.Loaded )
			{
				if ( MySandboxGameWrapper.IsGameStarted )
				{
					if ( CommandLineArgs.WorldRequestReplace )
						ServerNetworkManager.Instance.ReplaceWorldJoin( );

					if ( CommandLineArgs.WorldDataModify )
						ServerNetworkManager.Instance.ReplaceWorldData( );

                    if (CommandLineArgs.WorldVoxelModify)
                        ServerNetworkManager.WorldVoxelModify = true;

					//SandboxGameAssemblyWrapper.InitAPIGateway();
					_pluginManager.LoadPlugins( );
					_pluginManager.Init( );
                    ServerNetworkManager.Instance.InitNetworkIntercept();
                    //SandboxGameAssemblyWrapper.Instance.GameAction( ( ) => AppDomain.CurrentDomain.ClearEventInvocations( "_unhandledException" ) );

				    SteamServerAPI.Instance.GameServer.SetKeyValue("SM", "SE Server Extender");

                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
					//AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
					Application.ThreadException += Application_ThreadException;
					Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );
				}
			}
			else
			{
				//Force a refresh of the chat messages before running the plugin update
				List<string> messages = ChatManager.Instance.ChatMessages;

				//Run the plugin update
				_pluginManager.Update( );
			}
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		static void Application_ThreadException( object sender, ThreadExceptionEventArgs e )
		{
			ApplicationLog.BaseLog.Fatal( e.Exception,"Application.ThreadException - {0}", e.Exception );
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		static void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e )
		{
			ApplicationLog.BaseLog.Fatal( "AppDomain.UnhandledException - {0}", e.ExceptionObject );
		}

		private void AutoSaveMain( object sender, EventArgs e )
		{
			if ( !Instance.IsRunning )
			{
				_autosaveTimer.Stop( );
				return;
			}

			if ( CommandLineArgs.AutoSaveSync )
				WorldManager.Instance.SaveWorld( );
			else
				WorldManager.Instance.AsynchronousSaveWorld( );
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private void RunServer( )
		{
			if ( _restartLimit < 0 )
				return;

			try
			{
				ExtenderOptions.InstanceName = InstanceName;
				_dedicatedServerWrapper = DedicatedServerAssemblyWrapper.Instance;
                bool result = _dedicatedServerWrapper.StartServer( _commandLineArgs.InstanceName, _commandLineArgs.InstancePath, !_commandLineArgs.NoConsole );
                
                ApplicationLog.BaseLog.Info( "Server has stopped running" );

				_isServerRunning = false;

				_pluginMainLoop.Stop( );
				_autosaveTimer.Stop( );

				_pluginManager.Shutdown( );

				if ( !result && _commandLineArgs.CloseOnCrash )
				{
					Thread.Sleep( 5000 );
					Environment.Exit( 1 );
				}

				if ( !result && _commandLineArgs.RestartOnCrash )
				{
					Thread.Sleep( 5000 );

					string restartText = "timeout /t 20\r\n";
					restartText += string.Format( "cd /d \"{0}\"\r\n", System.IO.Path.GetDirectoryName( Application.ExecutablePath ) );
					restartText += string.Format( "{0} {1}\r\n", System.IO.Path.GetFileName( Application.ExecutablePath ), _commandLineArgs.Args );

					File.WriteAllText( "RestartApp.bat", restartText );
					Process.Start( "RestartApp.bat" );
					Environment.Exit( 1 );
				}

				/*
				if (!result)
				{
					LogManager.APILog.WriteLineAndConsole("Server crashed, attempting auto-restart ...");

					TimeSpan timeSinceLastRestart = DateTime.Now - m_lastRestart;

					//Reset the restart limit if the server has been running for more than 5 minutes before the crash
					if (timeSinceLastRestart.TotalMinutes > 5)
						_restartLimit = 3;

					_restartLimit--;

					_isServerRunning = true;
					SectorObjectManager.Instance.IsShutDown = false;

					_runServerThread = new Thread(new ThreadStart(this.RunServer));
					_runServerThread.Start();
				}*/
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
			finally
			{
				_dedicatedServerWrapper = null;
			}
		}

		public void StartServer( )
		{
			try
			{
				if ( !_isInitialized )
					return;
				if ( _isServerRunning )
					return;

			    if (DisableProfiler)
			    {
			        KillProfiler();
			    }
			    else
			    {
                    //profiler injection is timing critical
                    //create a fake plugin to trick the game into calling our injection init
                    //at MyPlugins.Init
			        var pluginslist = (List<IPlugin>)typeof(MyPlugins).GetField("m_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			        pluginslist.Add(new ProfilerFakePlugin());
			    }

			    if ( _dedicatedConfigDefinition == null )
					LoadServerConfig( );

			    if (Config == null)
			    {
                    ApplicationLog.BaseLog.Error("NULL CONFIG!");
			    }
			    else
			    {
			        if (Config.AutoSaveInMinutes > 0)
			        {
			            Config.AutoSaveInMinutes = 0;
			            SaveServerConfig();
			        }
			    }

			    _sessionManager.UpdateSessionSettings( );
				_pluginMainLoop.Start( );
                _autosaveTimer.Start( );

                _isServerRunning = true;
				_serverRan = true;
                
				_runServerThread = new Thread( RunServer ) { IsBackground = true };
                _runServerThread.Start( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				_isServerRunning = false;
			}
        }

        private void KillProfiler()
        {
            var keenStart = typeof(MySimpleProfiler).GetMethod("Begin");
            var keenEnd = typeof(MySimpleProfiler).GetMethod("End");
            var ourStart = typeof(Server).GetMethod("Begin");
            var ourEnd = typeof(Server).GetMethod("End");

            MethodUtil.ReplaceMethod(ourStart, keenStart);
            MethodUtil.ReplaceMethod(ourEnd, keenEnd);
        }

        public static void Begin(string key)
        { }

        public static void End(string Key)
        { }
        
		public void StopServer( )
		{
		    if (!_isServerRunning)
		        return; 

			ApplicationLog.BaseLog.Info( "Stopping server" );
            
            //cache for console output while we're waiting
		    StringBuilder sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
		    TextWriter tmp = Console.Out;

            //hijack the console so we can listen for the server stopped message
            Console.SetOut(tw);

            //ask the server nicely to stop
			MySandboxGame.ExitThreadSafe();

			_pluginMainLoop.Stop( );
			_autosaveTimer.Stop( );
			_pluginManager.Shutdown( );

            DateTime waitStart = DateTime.Now;

		    while (true)
		    {
                if (sb.ToString().Contains("Server stopped, press any key to close this window"))
		            break;
		        
                Thread.Sleep(100);

                //the server didn't listen, so kill it forcefully
		        if (DateTime.Now - waitStart > TimeSpan.FromMinutes(5))
		        {
                    ApplicationLog.BaseLog.Warn("Server failed to shut down correctly!");
		            break;
		        }
		    }

            //return control back to the console and write the cached log back
            Console.SetOut(tmp);
            Console.Write(sb);
            
			//_runServerThread.Interrupt();
			//_dedicatedServerWrapper.StopServer();
			//_runServerThread.Abort();
			//_runServerThread.Interrupt( );

			_isServerRunning = false;

			ApplicationLog.BaseLog.Info(  "Server has been stopped" );
		}

        public static bool registerResult = false;
        public static bool serializerResult = false;
        public static bool registered = false;
		public MyConfigDedicatedData<MyObjectBuilder_SessionSettings> LoadServerConfig( )
        {
            /*
            if ( !registered )
            {
                registered = true;
                MyObjectBuilderType.RegisterAssemblies( );
            }*/

            if ( File.Exists( System.IO.Path.Combine(Path,"SpaceEngineers-Dedicated.cfg.restart") ) )
			{
				File.Copy( System.IO.Path.Combine( Path, "SpaceEngineers-Dedicated.cfg.restart" ), System.IO.Path.Combine( Path, "SpaceEngineers-Dedicated.cfg" ), true );
				File.Delete( System.IO.Path.Combine( Path, "SpaceEngineers-Dedicated.cfg.restart" ) );
			}

            if ( File.Exists( System.IO.Path.Combine( Path, "SpaceEngineers-Dedicated.cfg" ) ) )
            {
                MyConfigDedicatedData<MyObjectBuilder_SessionSettings> config = DedicatedConfigDefinition.Load( new FileInfo( System.IO.Path.Combine( Path, "SpaceEngineers-Dedicated.cfg" ) ) );
                if(config==null)
                    throw new FileLoadException("Failed to load session settings: " + Path);
                _dedicatedConfigDefinition = new DedicatedConfigDefinition( config );
                _cfgWatch = new FileSystemWatcher( Path, "*.cfg" );
                _cfgWatch.Changed += Config_Changed;
                _cfgWatch.NotifyFilter = NotifyFilters.Size;
                _cfgWatch.EnableRaisingEvents = true;
                return config;
            }
            else
            {
                //if ( ExtenderOptions.IsDebugging )
                //{
                    ApplicationLog.BaseLog.Info( "Failed to load session settings" );
                    ApplicationLog.BaseLog.Info( Path );
                //}
                return null;
            }


			/*
			FileInfo fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg");
			if (fileInfo.Exists)
			{
				if (!File.Exists(Path + @"\SpaceEngineers-Dedicated.cfg.restart"))
					File.Copy(Path + @"\SpaceEngineers-Dedicated.cfg", Path + @"\SpaceEngineers-Dedicated.cfg.restart");

				MyConfigDedicatedData config = DedicatedConfigDefinition.Load(fileInfo);

				FileInfo restartFileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg.restart");
				
				if (restartFileInfo.Exists)				
					config = DedicatedConfigDefinition.Load(restartFileInfo);
	
				_dedicatedConfigDefinition = new DedicatedConfigDefinition(config);
				return config;
			}
			else
				return null;	
			 */
		}

		private void Config_Changed( object sender, FileSystemEventArgs e )
		{
			if ( !e.Name.Contains( "SpaceEngineers-Dedicated.cfg" ) || e.Name.Contains( "SpaceEngineers-Dedicated.cfg.restart" ) )
				return;

			if ( !_serverRan )
				return;

			if ( e.ChangeType == WatcherChangeTypes.Changed )
			{
				try
				{

					if ( !File.Exists( Path + @"\SpaceEngineers-Dedicated.cfg.restart" ) )
					{
						ApplicationLog.BaseLog.Info( "SpaceEngineers-Dedicated.cfg has changed updating configuration settings." );

						MyConfigDedicatedData<MyObjectBuilder_SessionSettings> changedConfig = DedicatedConfigDefinition.Load( new FileInfo( e.FullPath ) );
						Config = new DedicatedConfigDefinition( changedConfig );
					}
					else
					{
						ApplicationLog.BaseLog.Info( "SpaceEngineers-Dedicated.cfg has changed with existing restart file." );

						MyConfigDedicatedData<MyObjectBuilder_SessionSettings> restartConfig = DedicatedConfigDefinition.Load( new FileInfo( Path + @"\SpaceEngineers-Dedicated.cfg.restart" ) );
						MyConfigDedicatedData<MyObjectBuilder_SessionSettings> changedConfig = DedicatedConfigDefinition.Load( new FileInfo( e.FullPath ) );

						restartConfig.Mods = restartConfig.Mods.Union( changedConfig.Mods ).ToList( );
						restartConfig.Banned = changedConfig.Banned.Union( changedConfig.Banned ).ToList( );
						restartConfig.Administrators = changedConfig.Administrators.Union( changedConfig.Administrators ).ToList( );
						DedicatedConfigDefinition config = new DedicatedConfigDefinition( restartConfig );
						config.Save( new FileInfo( Path + @"\SpaceEngineers-Dedicated.cfg.restart" ) );
						Config = config;
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( "Error on configuration change ({1})", e.FullPath);
					ApplicationLog.BaseLog.Error( ex );
				}
			}
		}

		public void SaveServerConfig( )
		{
			FileInfo fileInfo = new FileInfo( Path + @"\SpaceEngineers-Dedicated.cfg" );

			if ( _serverRan )
				fileInfo = new FileInfo( Path + @"\SpaceEngineers-Dedicated.cfg.restart" );

			if ( _dedicatedConfigDefinition != null )
				_dedicatedConfigDefinition.Save( fileInfo );
		}

#endregion

        class ProfilerFakePlugin : IPlugin
        {
            public void Dispose()
            {
            }

            public void Init(object gameInstance)
            {
                ApplicationLog.BaseLog.Info("Initializing profiler injector");
                ProfilerInjection.Init();
            }

            public void Update()
            {
            }
        }
    }
}
