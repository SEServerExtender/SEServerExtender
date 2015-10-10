namespace SEModAPIInternal.Support
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using NLog;
	using SEModAPI.API.Sandbox;
	using VRage.FileSystem;

	public class ApplicationLog
	{
		private readonly bool _useInstancePath;
		private StringBuilder _appVersion;
		private FileInfo _filePath;
		private bool _instanceMode;
		private DirectoryInfo _libraryPath;
		private string _logFileName;

		public static Logger BaseLog = LogManager.GetLogger( "BaseLog" );
		public static Logger ChatLog = LogManager.GetLogger( "ChatLog" );
		public static Logger PluginLog = LogManager.GetLogger( "PluginLog" );

		public ApplicationLog( bool useGamePath = false )
		{
			_useInstancePath = useGamePath;

			if ( _useInstancePath && MySandboxGameWrapper.IsGameStarted && MyFileSystem.UserDataPath != null )
			{
				_libraryPath = new DirectoryInfo( MyFileSystem.UserDataPath );

				_instanceMode = true;
			}
			else
			{
				string codeBase = Assembly.GetExecutingAssembly( ).CodeBase;
				UriBuilder uri = new UriBuilder( codeBase );
				string path = Uri.UnescapeDataString( uri.Path );
				_libraryPath = new DirectoryInfo( Path.Combine( Path.GetDirectoryName( path ), "Logs" ) );
				if ( !_libraryPath.Exists )
				{
					Directory.CreateDirectory( _libraryPath.ToString( ) );
				}
			}
		}

		public static void Info( string text )
		{
			BaseLog.Info( text );
		}

		public static void Error( Exception ex, string text )
		{
			BaseLog.Error( ex, text, ex );
		}

		public static void Error( string text )
		{
			BaseLog.Error( text );
		}

		public bool LogEnabled { get { return _filePath != null; } }

		public string GetFilePath( )
		{
			if ( _filePath == null )
			{
				return "";
			}

			return _filePath.ToString( );
		}

		public void Init( string logFileName, StringBuilder appVersionString )
		{
			_logFileName = logFileName;
			_appVersion = appVersionString;

			_filePath = new FileInfo( Path.Combine( _libraryPath.ToString( ), _logFileName ) );

			//If the log file already exists then archive it
			if ( _filePath.Exists )
			{
				DateTime lastWriteTime = _filePath.LastWriteTime;
				string modifiedTimestamp = string.Format( "{0}_{1}_{2}_{3}_{4}_{5}", lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, lastWriteTime.Second );
				string fileNameWithoutExtension = _filePath.Name.Remove( _filePath.Name.Length - _filePath.Extension.Length );
				string newFileName = string.Format( "{0}_{1}{2}", fileNameWithoutExtension, modifiedTimestamp, _filePath.Extension );

				File.Move( _filePath.ToString( ), Path.Combine( _libraryPath.ToString( ), newFileName ) );
			}

			int num = (int)Math.Round( ( DateTime.Now - DateTime.UtcNow ).TotalHours );

			BaseLog.Info( "Log Started" );
			BaseLog.Info( "Timezone (local - UTC): " + num + "h" );
			BaseLog.Info( "App Version: " + _appVersion );
		}

		private static int GetThreadId( )
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
	}
}