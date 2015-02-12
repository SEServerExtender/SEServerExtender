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
		private readonly bool _useInstancePath;
		private bool _instanceMode;
		private string _logFileName;
		private StringBuilder _appVersion;
		private DirectoryInfo _libraryPath;
		private FileInfo _filePath;
		private readonly StringBuilder _stringBuilder;
		private static readonly object LogMutex = new object( );

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
					Directory.CreateDirectory( _libraryPath.ToString( ) );
			}

			_stringBuilder = new StringBuilder( );
		}

		public bool LogEnabled
		{
			get { return _filePath != null; }
		}

		public string GetFilePath( )
		{
			if ( _filePath == null )
				return "";

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
				string modifiedTimestamp = lastWriteTime.Year + "_" + lastWriteTime.Month + "_" + lastWriteTime.Day + "_" + lastWriteTime.Hour + "_" + lastWriteTime.Minute + "_" + lastWriteTime.Second;
				string fileNameWithoutExtension = _filePath.Name.Remove( _filePath.Name.Length - _filePath.Extension.Length );
				string newFileName = fileNameWithoutExtension + "_" + modifiedTimestamp + _filePath.Extension;

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
				return;

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
					AppendDateAndTime( _stringBuilder );
					_stringBuilder.Append( " - " );
					AppendThreadInfo( _stringBuilder );
					_stringBuilder.Append( " -> " );
					_stringBuilder.Append( msg );
					TextWriter m_Writer = new StreamWriter( _filePath.ToString( ), true );
					TextWriter.Synchronized( m_Writer ).WriteLine( _stringBuilder.ToString( ) );
					m_Writer.Close( );
					_stringBuilder.Clear( );
				}
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "Failed to write to log: " + ex );
			}
		}

		public void WriteLine( string message, LoggingOptions option )
		{
			WriteLine( message );
		}

		public void WriteLine( Exception ex )
		{
			if ( _filePath == null )
				return;

			if ( ex == null )
				return;

			WriteLine( ex.ToString( ) );

			if ( ex.InnerException == null )
				return;

			WriteLine( ex.InnerException );
		}

		public void WriteLineAndConsole( string msg )
		{
			if ( _filePath == null )
				return;

			WriteLine( msg );

			lock ( LogMutex )
			{
				_stringBuilder.Clear( );
				AppendDateAndTime( _stringBuilder );
				_stringBuilder.Append( " - " );
				_stringBuilder.Append( msg );
				Console.WriteLine( _stringBuilder.ToString( ) );
				_stringBuilder.Clear( );
			}
		}

		public void WriteLineAndConsole( Exception ex )
		{
			if ( _filePath == null )
				return;

			WriteLine( ex );

			lock ( LogMutex )
			{
				_stringBuilder.Clear( );
				AppendDateAndTime( _stringBuilder );
				_stringBuilder.Append( " - " );
				_stringBuilder.Append( ex );
				try
				{
					Console.WriteLine( _stringBuilder.ToString( ) );
				}
				catch ( IOException ioex )
				{
					WriteLine( ioex );
				}
				_stringBuilder.Clear( );
			}
		}

		private static int GetThreadId( )
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		private static void AppendDateAndTime( StringBuilder sb )
		{
			try
			{
				DateTimeOffset now = DateTimeOffset.Now;
				sb.Concat( now.Year, 4U, '0', 10U, false ).Append( '-' );
				sb.Concat( now.Month, 2U ).Append( '-' );
				sb.Concat( now.Day, 2U ).Append( ' ' );
				sb.Concat( now.Hour, 2U ).Append( ':' );
				sb.Concat( now.Minute, 2U ).Append( ':' );
				sb.Concat( now.Second, 2U ).Append( '.' );
				sb.Concat( now.Millisecond, 3U );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex.ToString( ) );
			}
		}

		private static void AppendThreadInfo( StringBuilder sb )
		{
			sb.Append( "Thread: " + GetThreadId( ) );
		}
	}
}