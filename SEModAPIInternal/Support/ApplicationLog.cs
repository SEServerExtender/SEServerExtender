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

		/// <summary>
		/// Writes a friendly message and an optional exception to the log file only.
		/// </summary>
		/// <param name="ex">An exception to log. Will log a stack trace.</param>
		public void WriteLine( string msg, Exception ex = null )
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
					using ( TextWriter writer = new StreamWriter( _filePath.ToString( ), true ) )
					{
						if ( ex == null )
							TextWriter.Synchronized( writer ).WriteLine( "{0} - Thread: {1} -> {2}",
							                                             DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),
							                                             GetThreadId( ),
							                                             msg );
						else
							TextWriter.Synchronized( writer ).WriteLine( "{0} - Thread: {1} -> {2}\r\n\tException: {3}",
							                                             DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),
							                                             GetThreadId( ),
							                                             msg,
							                                             ex );
							
						writer.Close( );
					}
				}
			}
			catch ( Exception loggingException )
			{
				Console.WriteLine( "Failed to write to log: {0}", loggingException );
			}
		}

		/// <summary>
		/// Writes an exception to the log file only.
		/// </summary>
		/// <param name="ex">An exception to log. Will log a stack trace.</param>
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

		/// <summary>
		/// Writes an exception to the console and the log file.
		/// </summary>
		/// <param name="ex">An exception to log. Will log a stack trace.</param>
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

		/// <summary>
		/// Writes a friendly message and an optional exception to the console and the log file.
		/// </summary>
		/// <param name="msg">A friendly message describing what is happening.</param>
		/// <param name="ex">An optional exception to log. Will log a stack trace.</param>
		public void WriteLineAndConsole( string msg, Exception ex = null )
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
					if ( ex == null )
						Console.WriteLine( "{0} - {1}",
										   DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),
										   msg );
					else
						Console.WriteLine( "{0} - {1}\r\n\tException: {2}",
										   DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ),
										   msg,
										   ex );
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