using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.ExceptionServices;
using System.Security;

using Sandbox.Common.ObjectBuilders;
using SEModAPI.API;
using SEModAPI.API.Definitions;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;
using SEModAPIExtensions.API.IPC;
using System.ServiceModel.Web;

namespace SEModAPIExtensions.API
{
	using System.Diagnostics;
	using System.Timers;

	[DataContract(Name = "ServerProxy",IsReference = true)]
	public class Server
	{
		#region "Attributes"

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
		private WebServiceHost _webHost;

		//Managers
		private PluginManager _pluginManager;
		private SandboxGameAssemblyWrapper _gameAssemblyWrapper;
		private FactionsManager _factionsManager;
		private ServerAssemblyWrapper _serverWrapper;
		private LogManager _logManager;
		private EntityEventManager _entityEventManager;
		private ChatManager _chatManager;
		private SessionManager _sessionManager;

		//Timers
		private readonly Timer _pluginMainLoop;
		private readonly Timer _autosaveTimer;

		#endregion

		#region "Constructors and Initializers"

		protected Server()
		{
			if ( _isInitialized )
				return;

			_restartLimit = 3;

			_pluginMainLoop = new Timer { Interval = 200 };
			_pluginMainLoop.Elapsed += PluginManagerMain;

			_autosaveTimer = new Timer { Interval = 300000 };
			_autosaveTimer.Elapsed += AutoSaveMain;

			_isWcfEnabled = true;

			Console.WriteLine("Finished creating server!");
		}

		private bool SetupServerService()
		{
			try
			{
				_serverHost = CreateServiceHost( typeof( ServerService ), typeof( IServerServiceContract ), "Server/", "ServerService" );
				_serverHost.Open( );
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				_serverHost.Abort( );
				return false;
			}

			return true;
		}

		private bool SetupMainService()
		{
			try
			{
				_baseHost = CreateServiceHost( typeof( InternalService ), typeof( IInternalServiceContract ), "", "InternalService" );
				_baseHost.Open( );
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				_baseHost.Abort( );
				return false;
			}

			return true;
		}

		private bool SetupWebService()
		{
			Uri webServiceAddress = new Uri( "http://localhost:" + WCFPort + "/SEServerExtender/Web/" );
			_webHost = new WebServiceHost( typeof( WebService ), webServiceAddress );
			try
			{
				//WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
				//binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
				//binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
				//ServiceEndpoint endpoint = _webHost.AddServiceEndpoint(typeof(IWebServiceContract), binding, "WebService");
				//_webHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
				//_webHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserNameValidator();
				EnableCorsBehavior ecb = new EnableCorsBehavior();
				_webHost.Description.Behaviors.Add( ecb );
				_webHost.Open( );
			}
			catch (CommunicationException ex)
			{
				Console.WriteLine("An exception occurred: {0}", ex.Message);
				_webHost.Abort( );
				return false;
			}

			return true;
		}

		private bool SetupGameInstallation()
		{
			try
			{
				string gamePath = _commandLineArgs.GamePath;
				if (gamePath.Length > 0)
				{
					if (!GameInstallationInfo.IsValidGamePath(gamePath))
						return false;
					_gameInstallationInfo = new GameInstallationInfo( gamePath );
				}
				else
				{
					_gameInstallationInfo = new GameInstallationInfo( );
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			if ( _gameInstallationInfo != null )
				return true;
			else
				return false;
		}

		private bool SetupManagers()
		{
			_serverWrapper = ServerAssemblyWrapper.Instance;
			_pluginManager = PluginManager.Instance;
			_gameAssemblyWrapper = SandboxGameAssemblyWrapper.Instance;
			_factionsManager = FactionsManager.Instance;
			_logManager = LogManager.Instance;
			_entityEventManager = EntityEventManager.Instance;
			_chatManager = ChatManager.Instance;
			_sessionManager = SessionManager.Instance;

			return true;
		}

		private bool ProcessCommandLineArgs()
		{
			try
			{
				if ( _commandLineArgs.AutoStart )
				{
					Console.WriteLine("Auto-Start enabled");
				}
				if ( _commandLineArgs.InstanceName.Length != 0 )
				{
					Console.WriteLine( "Common instance pre-selected: '" + _commandLineArgs.InstanceName + "'" );
				}
				if ( _commandLineArgs.NoGui )
				{
					Console.WriteLine("GUI disabled");
				}
				if ( _commandLineArgs.Debug )
				{
					Console.WriteLine("Debugging enabled");
					SandboxGameAssemblyWrapper.IsDebugging = true;
				}
				if ( _commandLineArgs.NoWcf )
				{
					Console.WriteLine("WCF disabled");
				}
				if ( _commandLineArgs.WcfPort > 0 )
				{
					Console.WriteLine( "WCF port: " + _commandLineArgs.WcfPort );
				}
				if ( _commandLineArgs.Autosave > 0 )
				{
					Console.WriteLine( "Autosave interval: " + _commandLineArgs.Autosave );
				}
				if ( _commandLineArgs.CloseOnCrash )
				{
					Console.WriteLine("Close On Crash: Enabled");
				}
				if ( _commandLineArgs.AutoSaveSync )
				{
					Console.WriteLine("Synchronous Save: Enabled");
				}
				if ( _commandLineArgs.Path.Length != 0 )
				{
					Console.WriteLine( "Full path pre-selected: '" + _commandLineArgs.Path + "'" );
				}
				if ( _commandLineArgs.RestartOnCrash )
				{
					Console.WriteLine("Restart On Crash: Enabled");
				}
				if ( _commandLineArgs.AutoSaveSync )
				{
					Console.WriteLine("Synchronous Autosave: Enabled");
				}
				if ( _commandLineArgs.WorldRequestReplace )
				{
					Console.WriteLine("World Request Replace: Enabled");
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
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
					if ( _webHost != null )
						_webHost.Open( );
					if ( _baseHost != null )
						_baseHost.Open( );
				}
				else
				{
					if ( _serverHost != null )
						_serverHost.Close( );
					if ( _webHost != null )
						_webHost.Close( );
					if ( _baseHost != null )
						_baseHost.Close( );
				}
			}
		}

		[IgnoreDataMember]
		public ushort WCFPort
		{
			get
			{
				ushort port = _commandLineArgs.WcfPort;
				if (port == 0)
					port = 8000;

				return port;
			}
			set { _commandLineArgs.WcfPort = value; }
		}

		[IgnoreDataMember]
		public string Path
		{
			get
			{
				string path = _commandLineArgs.Path;
				if ( string.IsNullOrEmpty( path ) )
				{
					if (InstanceName.Length != 0)
					{
						SandboxGameAssemblyWrapper.UseCommonProgramData = true;
						SandboxGameAssemblyWrapper.Instance.InitMyFileSystem(InstanceName, false);
					}
					path = _gameAssemblyWrapper.GetUserDataPath( InstanceName );
				}

				return path;
			}
			set { _commandLineArgs.Path = value; }
		}

		[IgnoreDataMember]
		public Thread ServerThread
		{
			get { return _runServerThread; }
		}

		#endregion

		#region "Methods"

		public static ServiceHost CreateServiceHost(Type serviceType, Type contractType, string urlExtension, string name)
		{
			try
			{
				Uri baseAddress = new Uri( string.Format( "http://localhost:{0}/SEServerExtender/{1}", Instance.WCFPort, urlExtension ) );
				ServiceHost selfHost = new ServiceHost(serviceType, baseAddress);

				WSHttpBinding binding = new WSHttpBinding { Security = { Mode = SecurityMode.Message, Message = { ClientCredentialType = MessageCredentialType.Windows } } };
				selfHost.AddServiceEndpoint(contractType, binding, name);

				ServiceMetadataBehavior smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
				selfHost.Description.Behaviors.Add(smb);

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Console.WriteLine( "Created WCF service at '{0}'", baseAddress );
				}

				return selfHost;
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole( string.Format( "An exception occurred: {0}", ex.Message ) );
				return null;
			}
		}

		public void Init()
		{
			if ( _isInitialized )
				return;

			bool setupResult = true;
			setupResult &= SetupGameInstallation();
			setupResult &= SetupManagers();
			setupResult &= ProcessCommandLineArgs();

			if (IsWCFEnabled)
			{
				SetupServerService();
				SetupMainService();
				SetupWebService();
			}

			if ( _commandLineArgs.Autosave > 0 )
				_autosaveTimer.Interval = _commandLineArgs.Autosave * 60000;

			if (!setupResult)
			{
				LogManager.ErrorLog.WriteLineAndConsole("Failed to initialize server");
				return;
			}

			_isInitialized = true;
			_serverRan = false;
		}

		private void PluginManagerMain(object sender, EventArgs e)
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
				if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				{
					if ( CommandLineArgs.WorldRequestReplace )
						ServerNetworkManager.Instance.ReplaceWorldJoin();

					if ( CommandLineArgs.WorldDataModify )
						ServerNetworkManager.Instance.ReplaceWorldData();

					SandboxGameAssemblyWrapper.InitAPIGateway();
					_pluginManager.LoadPlugins( );
					_pluginManager.Init( );

					SandboxGameAssemblyWrapper.Instance.GameAction( ( ) => AppDomain.CurrentDomain.ClearEventInvocations( "_unhandledException" ) );

					AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
					//AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
					Application.ThreadException += Application_ThreadException;
					Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
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

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Console.WriteLine( "Renewed Application.ThreadException - {0}", e.Exception );

			if (LogManager.APILog != null && LogManager.APILog.LogEnabled)
			{
				LogManager.APILog.WriteLine("Application.ThreadException");
				LogManager.APILog.WriteLine(e.Exception);
			}
			if (LogManager.ErrorLog != null && LogManager.ErrorLog.LogEnabled)
			{
				LogManager.ErrorLog.WriteLine("Application.ThreadException");
				LogManager.ErrorLog.WriteLine(e.Exception);
			}
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine( "Renewed - AppDomain.UnhandledException - {0}", e.ExceptionObject );

			if (LogManager.APILog != null && LogManager.APILog.LogEnabled)
			{
				LogManager.APILog.WriteLine("AppDomain.UnhandledException");
				LogManager.APILog.WriteLine((Exception)e.ExceptionObject);
			}
			if (LogManager.ErrorLog != null && LogManager.ErrorLog.LogEnabled)
			{
				LogManager.ErrorLog.WriteLine("AppDomain.UnhandledException");
				LogManager.ErrorLog.WriteLine((Exception)e.ExceptionObject);
			}			
		}

		private void AutoSaveMain(object sender, EventArgs e)
		{
			if ( !Instance.IsRunning )
			{
				_autosaveTimer.Stop( );
				return;
			}

			if ( CommandLineArgs.AutoSaveSync )
				WorldManager.Instance.SaveWorld();
			else
				WorldManager.Instance.AsynchronousSaveWorld();
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private void RunServer()
		{
			if ( _restartLimit < 0 )
				return;

			try
			{
				SandboxGameAssemblyWrapper.InstanceName = InstanceName;
				_serverWrapper = ServerAssemblyWrapper.Instance;
				bool result = _serverWrapper.StartServer( _commandLineArgs.InstanceName, _commandLineArgs.Path, !_commandLineArgs.NoConsole );
				Console.WriteLine("Server has stopped running");

				_isServerRunning = false;

				_pluginMainLoop.Stop( );
				_autosaveTimer.Stop( );

				_pluginManager.Shutdown( );

				if ( !result && _commandLineArgs.CloseOnCrash )
				{
					Thread.Sleep(5000);
					Environment.Exit(1);
				}

				if ( !result && _commandLineArgs.RestartOnCrash )
				{
					Thread.Sleep(5000);

					string restartText = "timeout /t 20\r\n";
					restartText += string.Format( "cd /d \"{0}\"\r\n", System.IO.Path.GetDirectoryName( Application.ExecutablePath ) );
					restartText += string.Format( "{0} {1}\r\n", System.IO.Path.GetFileName( Application.ExecutablePath ), _commandLineArgs.Args );

					File.WriteAllText("RestartApp.bat", restartText);
					Process.Start( "RestartApp.bat" );
					Environment.Exit(1);
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
			catch (Exception ex)
			{
				Console.WriteLine("Uncaught");
				LogManager.ErrorLog.WriteLine(ex);
			}
			finally
			{
				_serverWrapper = null;
			}
		}

		public void StartServer()
		{
			try
			{
				if ( !_isInitialized )
					return;
				if ( _isServerRunning )
					return;

				if ( _dedicatedConfigDefinition == null )
					LoadServerConfig();

				_sessionManager.UpdateSessionSettings( );
				_pluginMainLoop.Start( );
				_autosaveTimer.Start( );

				_isServerRunning = true;
				_serverRan = true;

				_runServerThread = new Thread( RunServer ) { IsBackground = true };
				_runServerThread.Start( );
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				_isServerRunning = false;
			}
		}

		public void StopServer()
		{
			LogManager.APILog.WriteLine("StopServer()");
			SandboxGameAssemblyWrapper.Instance.ExitGame();

			_pluginMainLoop.Stop( );
			_autosaveTimer.Stop( );
			_pluginManager.Shutdown( );

			//_runServerThread.Interrupt();
			//_serverWrapper.StopServer();
			//_runServerThread.Abort();
			_runServerThread.Interrupt( );

			_isServerRunning = false;

			Console.WriteLine("Server has been stopped");
		}

		public MyConfigDedicatedData LoadServerConfig()
		{
			FileInfo fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg.restart");
			if (fileInfo.Exists)
			{
				File.Copy(Path + @"\SpaceEngineers-Dedicated.cfg.restart", Path + @"\SpaceEngineers-Dedicated.cfg", true);
				File.Delete(Path + @"\SpaceEngineers-Dedicated.cfg.restart");
			}

			fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg");
			if (fileInfo.Exists)
			{
				MyConfigDedicatedData config = DedicatedConfigDefinition.Load(fileInfo);
				_dedicatedConfigDefinition = new DedicatedConfigDefinition( config );
				_cfgWatch = new FileSystemWatcher( Path, "*.cfg" );
				_cfgWatch.Changed += Config_Changed;
				_cfgWatch.NotifyFilter = NotifyFilters.Size;
				_cfgWatch.EnableRaisingEvents = true;
				return config;
			}
			else
				return null;


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

		private void Config_Changed(object sender, FileSystemEventArgs e)
		{
			if (!e.Name.Contains("SpaceEngineers-Dedicated.cfg") || e.Name.Contains("SpaceEngineers-Dedicated.cfg.restart"))
				return;

			if ( !_serverRan )
				return;

			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				try
				{

					if (!File.Exists(Path + @"\SpaceEngineers-Dedicated.cfg.restart"))
					{
						LogManager.APILog.WriteLineAndConsole(string.Format("SpaceEngineers-Dedicated.cfg has changed updating configuration settings."));

						MyConfigDedicatedData changedConfig = DedicatedConfigDefinition.Load(new FileInfo(e.FullPath));
						Config = new DedicatedConfigDefinition(changedConfig);
					}
					else
					{
						LogManager.APILog.WriteLineAndConsole(string.Format("SpaceEngineers-Dedicated.cfg has changed with existing restart file."));

						MyConfigDedicatedData restartConfig = DedicatedConfigDefinition.Load(new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg.restart"));
						MyConfigDedicatedData changedConfig = DedicatedConfigDefinition.Load(new FileInfo(e.FullPath));

						restartConfig.Mods = restartConfig.Mods.Union(changedConfig.Mods).ToList();
						restartConfig.Banned = changedConfig.Banned.Union(changedConfig.Banned).ToList();
						restartConfig.Administrators = changedConfig.Administrators.Union(changedConfig.Administrators).ToList();
						DedicatedConfigDefinition config = new DedicatedConfigDefinition(restartConfig);
						config.Save(new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg.restart"));
						Config = config;
					}
				}
				catch (Exception ex)
				{
					LogManager.APILog.WriteLineAndConsole( string.Format( "Error on configuration change ({1}): {0}", e.FullPath, ex ) );
				}
			}
		}

		public void SaveServerConfig()
		{
			FileInfo fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg");

			if ( _serverRan )
				fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg.restart");
			
			if ( _dedicatedConfigDefinition != null )
				_dedicatedConfigDefinition.Save( fileInfo );
		}

		#endregion
	}
}
