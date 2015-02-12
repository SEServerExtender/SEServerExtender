namespace SEModAPIInternal.Support
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using SEModAPIInternal.API.Common;
	using SysUtils.Utils;
	using VRage.Common.Utils;

	public class ApplicationLog
	{
		private static readonly object LogMutex = new object( );
		private readonly StringBuilder _stringBuilder;
		private readonly bool _useInstancePath;
		private StringBuilder _appVersion;
		private FileInfo _filePath;
		private bool _instanceMode;
		private DirectoryInfo _libraryPath;
		private string _logFileName;

		public ApplicationLog( bool useGamePath = false )
		{
			_useInstancePath = useGamePath;

			if ( _useInstancePath && SandboxGameAssemblyWrapper.Instance.IsGameStarted && MyFileSystem.UserDataPath != null )
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

			_stringBuilder = new StringBuilder( );
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

			WriteLine( "Log Started" );
			WriteLine( "Timezone (local - UTC): " + num + "h" );
			WriteLine( "App Version: " + _appVersion );
		}

		public void WriteLine( string msg )
		{
			if ( _filePath == null )
			{
				return;
			}

			if ( _useInstancePath && !_instanceMode && SandboxGameAssemblyWrapper.Instance.IsGameStarted && MyFileSystem.UserDataPath != null )
			{
				_libraryPath = new DirectoryInfo( MyFileSystem.UserDataPath );

				_instanceMode = true;

				Init( _logFileName, _appVersion );
			}

			try
			{
				lock ( LogMutex )
				{
					_stringBuilder.Clear( );
					_stringBuilder.AppendFormat( "{0} - Thread: {1} -> {2}",
					                             DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),
					                             GetThreadId( ),
					                             msg );
					TextWriter m_Writer = new StreamWriter( _filePath.ToString( ), true );
					TextWriter.Synchronized( m_Writer ).WriteLine( _stringBuilder.ToString( ) );
					m_Writer.Close( );
					_stringBuilder.Clear( );
				}
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "Failed to write to log: {0}", ex );
			}
		}

		public void WriteLine( string message, LoggingOptions option )
		{
			WriteLine( message );
		}

		public void WriteLine( Exception ex )
		{
			if ( _filePath == null )
			{
				return;
			}

			if ( ex == null )
			{
				return;
			}

			WriteLine( ex.ToString( ) );

			if ( ex.InnerException == null )
			{
				return;
			}

			WriteLine( ex.InnerException );
		}

		public void WriteLineAndConsole( string msg )
		{
			if ( _filePath == null )
			{
				return;
			}

			WriteLine( msg );

			lock ( LogMutex )
			{
				Console.WriteLine( "{0} - {1}", DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ), msg );
			}
		}

		public void WriteLineAndConsole( Exception ex )
		{
			if ( _filePath == null )
			{
				return;
			}

			WriteLine( ex );

			lock ( LogMutex )
			{
				try
				{
					Console.WriteLine( "{0} - {1}", DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ), ex );
				}
				catch ( IOException ioex )
				{
					WriteLine( ioex );
				}
			}
		}

		private static int GetThreadId( )
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
	}
}