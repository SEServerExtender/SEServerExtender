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
			public WindowsService()
			{
				ServiceName = "SEServerExtender";
				CanPauseAndContinue = false;
				CanStop = true;	
				AutoLog = true;
			}

			protected override void OnStart(string[] args)
			{
				LogManager.APILog.WriteLine(string.Format( "Starting SEServerExtender Service with {0} arguments ...", args.Length ));

				Start(args);
			}

			protected override void OnStop()
			{
				LogManager.APILog.WriteLine("Stopping SEServerExtender Service...");

				Program.Stop();
			}
		}

		static SEServerExtender m_serverExtenderForm;
		static Server m_server;

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		static void Main(string[] args)
		{
			if (!Environment.UserInteractive)
			{
				using (var service = new WindowsService())
				{
					ServiceBase.Run(service);
				}
			}
			else
			{
				Start(args);
			}
		}

		private static void Start(string[] args)
		{
			//Setup error handling for unmanaged exceptions
			AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

			//AppDomain.CurrentDomain.ClearEventInvocations("_unhandledException");

			LogManager.APILog.WriteLine(string.Format( "Starting SEServerExtender with {0} arguments ...", args.Length ));

			CommandLineArgs extenderArgs = new CommandLineArgs
			                               {
				                               autoStart = false,
				                               worldName = string.Empty,
				                               instanceName = string.Empty,
				                               noGUI = false,
				                               noConsole = false,
				                               debug = false,
				                               gamePath = string.Empty,
				                               noWCF = false,
				                               autosave = 0,
				                               wcfPort = 0,
				                               path = string.Empty,
				                               closeOnCrash = false,
				                               restartOnCrash = false,
				                               args = string.Join( " ", args.Select( x => string.Format( "\"{0}\"", x ) ) )
			                               };

			//Setup the default args

			//Process the args
			foreach (string arg in args)
			{
				if (arg.Split('=').Length > 1)
				{
					string argName = arg.Split('=')[0];
					string argValue = arg.Split('=')[1];

					if (argName.ToLower().Equals("instance"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.instanceName = argValue;
					}
					else if (argName.ToLower().Equals("gamepath"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.gamePath = argValue;
					}
					else if (argName.ToLower().Equals("autosave"))
					{
						try
						{
							extenderArgs.autosave = int.Parse(argValue);
						}
						catch
						{
							//Do nothing
						}
					}
					else if (argName.ToLower().Equals("wcfport"))
					{
						try
						{
							extenderArgs.wcfPort = ushort.Parse(argValue);
						}
						catch
						{
							//Do nothing
						}
					}
					else if (argName.ToLower().Equals("path"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.path = argValue;
					}
				}
				else
				{
					if (arg.ToLower().Equals("autostart"))
					{
						extenderArgs.autoStart = true;
					}
					if (arg.ToLower().Equals("nogui"))
					{
						extenderArgs.noGUI = true;

						//Implies autostart
						extenderArgs.autoStart = true;
					}
					if (arg.ToLower().Equals("noconsole"))
					{
						extenderArgs.noConsole = true;

						//Implies nogui and autostart
						extenderArgs.noGUI = true;
						extenderArgs.autoStart = true;
					}
					if (arg.ToLower().Equals("debug"))
					{
						extenderArgs.debug = true;
					}
					if (arg.ToLower().Equals("nowcf"))
					{
						extenderArgs.noWCF = true;
					}
					if (arg.ToLower().Equals("closeoncrash"))
					{
						extenderArgs.closeOnCrash = true;
					}
					if (arg.ToLower().Equals("autosaveasync"))
					{
						extenderArgs.autoSaveSync = false;
					}
					if (arg.ToLower().Equals("autosavesync"))
					{
						extenderArgs.autoSaveSync = true;
					}
					if (arg.ToLower().Equals("restartoncrash"))
					{
						extenderArgs.restartOnCrash = true;
					}
					if (arg.ToLower().Equals("wrr"))
					{
						extenderArgs.worldRequestReplace = true;
					}
					if (arg.ToLower().Equals("wrm"))
					{
						extenderArgs.worldDataModify = true;
					}
				}
			}

			if (extenderArgs.noWCF)
				extenderArgs.wcfPort = 0;

			if (!string.IsNullOrEmpty(extenderArgs.path))
			{
				extenderArgs.instanceName = string.Empty;
			}

			if (!Environment.UserInteractive)
			{
				extenderArgs.noConsole = true;
				extenderArgs.noGUI = true;
				extenderArgs.autoStart = true;
			}

			if (extenderArgs.debug)
				SandboxGameAssemblyWrapper.IsDebugging = true;

			try
			{
				bool unitTestResult = BasicUnitTestManager.Instance.Run();
				if (!unitTestResult)
					SandboxGameAssemblyWrapper.IsInSafeMode = true;

				m_server = Server.Instance;
				m_server.CommandLineArgs = extenderArgs;
				m_server.IsWCFEnabled = !extenderArgs.noWCF;
				m_server.WCFPort = extenderArgs.wcfPort;
				m_server.Init();

				ChatManager.ChatCommand guiCommand = new ChatManager.ChatCommand { Command = "gui", Callback = ChatCommand_GUI };
				ChatManager.Instance.RegisterChatCommand(guiCommand);

				if (extenderArgs.autoStart)
				{
					m_server.StartServer();
				}

				if (!extenderArgs.noGUI)
				{
					Thread uiThread = new Thread(StartGUI);
					uiThread.SetApartmentState(ApartmentState.STA);
					uiThread.Start();
				}
				else if(Environment.UserInteractive)
					Console.ReadLine();
			}
			catch (AutoException eEx)
			{
				if (!extenderArgs.noConsole)
					Console.WriteLine("AutoException - {0}\n\r{1}", eEx.AdditionnalInfo, eEx.GetDebugString() );
				if (!extenderArgs.noGUI)
					MessageBox.Show(string.Format( "{0}\n\r{1}", eEx.AdditionnalInfo, eEx.GetDebugString() ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (extenderArgs.noConsole && extenderArgs.noGUI)
					throw eEx.GetBaseException();
			}
			catch (TargetInvocationException ex)
			{
				if (!extenderArgs.noConsole)
					Console.WriteLine("TargetInvocationException - {0}\n\r{1}", ex, ex.InnerException );
				if (!extenderArgs.noGUI)
					MessageBox.Show(string.Format( "{0}\n\r{1}", ex, ex.InnerException ), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (extenderArgs.noConsole && extenderArgs.noGUI)
					throw;
			}
			catch (Exception ex)
			{
				if (!extenderArgs.noConsole)
					Console.WriteLine("Exception - {0}", ex );
				if (!extenderArgs.noGUI)
					MessageBox.Show(ex.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (extenderArgs.noConsole && extenderArgs.noGUI)
					throw;
			}
		}

		private static void Stop()
		{
			if(m_server != null && m_server.IsRunning)
				m_server.StopServer();
			if (m_serverExtenderForm != null && m_serverExtenderForm.Visible)
				m_serverExtenderForm.Close();

			if (m_server.ServerThread != null)
			{
				m_server.ServerThread.Join(20000);
			}
		}

		public static void Application_ThreadException(Object sender, ThreadExceptionEventArgs e)
		{
			Console.WriteLine("Application.ThreadException - {0}", e.Exception );

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

		public static void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine("AppDomain.UnhandledException - {0}", e.ExceptionObject );

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

		static void ChatCommand_GUI(ChatManager.ChatEvent chatEvent)
		{
			Thread uiThread = new Thread(StartGUI);
			uiThread.SetApartmentState(ApartmentState.STA);
			uiThread.Start();
		}

		[STAThread]
		static void StartGUI()
		{
			if (!Environment.UserInteractive)
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (m_serverExtenderForm == null || m_serverExtenderForm.IsDisposed)
				m_serverExtenderForm = new SEServerExtender(m_server);
			else if (m_serverExtenderForm.Visible)
				return;

			Application.Run(m_serverExtenderForm);
		}
	}
}
