using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace SEModAPI.API
{
	/// <summary>
	/// Class dedicated to handle of Space Engineer installation and information
	/// </summary>
	public class GameInstallationInfo
	{
		#region "Attributes"

		private static string _gamePath;

		internal static readonly string[] CoreSpaceEngineersFiles = 
		{
			"Sandbox.Common.dll",
			"Sandbox.Common.XmlSerializers.dll",
			"VRage.Common.dll",
			"VRage.Library.dll",
			"VRage.Math.dll"
		};

		#endregion

		#region "Constructor and Initializers"

		/// <summary>
		/// <para>Create a new instance of GameInstallationInfo with the automatically detected game path.</para>
		/// <para>
		/// It is not recommanded to purely rely on this function since if the game is not found,
		/// the API will not be able to work
		/// </para>
		/// </summary>
		public GameInstallationInfo()
		{
			_gamePath = GetGameRegistryPath();
			if (string.IsNullOrEmpty( _gamePath ))
			{
				_gamePath = GetGameSteamPath();
				if (string.IsNullOrEmpty( _gamePath ))
				{
					_gamePath = GetGameExePath();
					if (string.IsNullOrEmpty( _gamePath ))
					{
						throw new GameInstallationInfoException(GameInstallationInfoExceptionState.EmptyGamePath, "Can't find the game path");
					}
				}
					
			}

			if (!IsValidGamePath(_gamePath))
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.BrokenGameDirectory, "The game directory is broken");
		}

		/// <summary>
		/// Create a new instance of GameInstallationInfo with the specified game location
		/// </summary>
		/// <param name="gamePath">Location of the game executable</param>
		public GameInstallationInfo(string gamePath)
		{
			_gamePath = gamePath;
			if (string.IsNullOrEmpty( _gamePath ))
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.EmptyGamePath, "The gamePath given is empty");

			if (!IsValidGamePath(_gamePath))
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.BrokenGameDirectory, "The gamePath provided is invalid");
		}

		#endregion

		#region "Properties"

		public static string GamePath
		{
			get { return _gamePath; }
		}

		#endregion

		#region "Methods"

		public static bool IsValidGamePath(string gamePath)
		{
			if (string.IsNullOrEmpty(gamePath))
			{
				return false;
			}

			if (!Directory.Exists(gamePath))
			{
				return false;
			}

			if (!Directory.Exists(Path.Combine(gamePath, "Content")))
			{
				return false;
			}

			if (!Directory.Exists(Path.Combine(gamePath, "DedicatedServer64")))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Looks for the Space Engineers install location in the Registry, which should return the form:
		/// "C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers"
		/// </summary>
		/// <returns>The absolute path to the game installation</returns>
		public static string GetGameRegistryPath()
		{
			RegistryKey key;
			if (Environment.Is64BitProcess)
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850", false);
			else
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850", false);

			if ( key == null )
			{
				return null;
			}
			string path = key.GetValue("InstallLocation") as string;
			return path != null && Directory.Exists(path) ? path : null;
		}

		/// <summary>
		/// Looks for the Steam install location in the Registry, which should return the form:
		/// "C:\Program Files (x86)\Steam"
		/// </summary>
		/// <returns>Return the Steam install location, or null if not found</returns>
		public static string GetGameSteamPath()
		{
			RegistryKey key;

			if (Environment.Is64BitProcess)
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam", false);
			else
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam", false);

			if (key != null)
			{
				string steamBasePath = (string)key.GetValue("InstallPath");
				string path = Path.Combine(steamBasePath, "steamapps", "common", "spaceengineers");
				return Directory.Exists(path) ? path : null;
			}

			return null;
		}

		/// <summary>
		/// Looks for the game install by going to the parent directory of this application
		/// </summary>
		/// <returns>The parent path of this application</returns>
		public static string GetGameExePath()
		{
			try
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
				DirectoryInfo directory = new DirectoryInfo(path);
				string finalPath = directory.Parent.FullName;

				return finalPath;
			}
			catch (Exception)
			{
				return "";
			}
		}

		public bool IsBaseAssembliesChanged()
		{
			// We use the Bin64 Path, as these assemblies are marked "All CPU", and will work regardless of processor architecture.
			string baseFilePath = Path.Combine(GamePath, "DedicatedServer64");
			string appFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			foreach (string filename in CoreSpaceEngineersFiles)
			{
				if (DoFilesDiffer(baseFilePath, appFilePath, filename))
					return true;
			}

			return false;
		}

		public bool UpdateBaseFiles()
		{
			// We use the Bin64 Path, as these assemblies are marked "All CPU", and will work regardless of processor architecture.
			string baseFilePath = Path.Combine(GamePath, "DedicatedServer64");

			foreach (string filename in CoreSpaceEngineersFiles)
			{
				string sourceFile = Path.Combine(baseFilePath, filename);

				if (File.Exists(sourceFile))
				{
					//File.Copy(sourceFile, Path.Combine(appFilePath, filename), true);
				}
			}

			return true;
		}

		public static bool DoFilesDiffer(string directory1, string directory2, string filename)
		{
			return DoFilesDiffer(Path.Combine(directory1, filename), Path.Combine(directory2, filename));
		}

		public static bool DoFilesDiffer(string file1, string file2)
		{
			if (File.Exists(file1) != File.Exists(file2))
				return false;

			byte[ ] buffer1 = File.ReadAllBytes(file1);
			byte[ ] buffer2 = File.ReadAllBytes(file2);

			if (buffer1.Length != buffer2.Length)
				return true;

			Assembly ass1 = Assembly.Load(buffer1);
			Guid guid1 = ass1.ManifestModule.ModuleVersionId;

			Assembly ass2 = Assembly.Load(buffer2);
			Guid guid2 = ass2.ManifestModule.ModuleVersionId;

			return guid1 != guid2;
		}

		internal static bool CheckIsRuningElevated()
		{
			WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		internal static int? RunElevated(string fileName, string arguments, bool elevate, bool waitForExit)
		{
			ProcessStartInfo processInfo = new ProcessStartInfo(fileName, arguments);

			if (elevate)
			{
				processInfo.Verb = "runas";
			}

			try
			{
				Process process = Process.Start(processInfo);

				if (waitForExit)
				{
					if (process != null)
					{
						process.WaitForExit();

						return process.ExitCode;
					}
				}

				return 0;
			}
			catch (Win32Exception)
			{
				// Do nothing. Probably the user canceled the UAC window
				return null;
			}
		}

		#endregion
	}
}
