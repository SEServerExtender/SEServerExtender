using System;
using System.Reflection;
using System.Text;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SysUtils.Utils;

namespace SEModAPIInternal.Support
{
	public class LogManager
	{
		#region "Attributes"

		private static LogManager _instance;

		private static MyLog _gameLog;
		private static ApplicationLog _apiLog;
		private static ApplicationLog _chatLog;
		private static ApplicationLog _errorLog;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected LogManager( )
		{
			_instance = this;

			Console.WriteLine( "Finished loading LogManager" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static LogManager Instance
		{
			get { return _instance ?? ( _instance = new LogManager( ) ); }
		}

		public static ApplicationLog APILog
		{
			get
			{
				if ( _apiLog == null )
				{
					LogManager temp = Instance;

					try
					{
						_apiLog = new ApplicationLog( true );
						StringBuilder internalAPIAppVersion = new StringBuilder( typeof( LogManager ).Assembly.GetName( ).Version.ToString( ) );
						_apiLog.Init( "SEModAPIInternal.log", internalAPIAppVersion );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( _apiLog == null )
					throw new Exception( "Failed to create API log!" );

				return _apiLog;
			}
		}

		public static ApplicationLog ChatLog
		{
			get
			{
				if ( _chatLog == null )
				{
					LogManager temp = Instance;

					try
					{
						_chatLog = new ApplicationLog( true );
						StringBuilder internalAPIAppVersion = new StringBuilder( typeof( LogManager ).Assembly.GetName( ).Version.ToString( ) );
						_chatLog.Init( "SEModAPIInternal_Chat.log", internalAPIAppVersion );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( _chatLog == null )
					throw new Exception( "Failed to create chat log!" );

				return _chatLog;
			}
		}

		public static ApplicationLog ErrorLog
		{
			get
			{
				if ( _errorLog == null )
				{
					LogManager temp = Instance;

					try
					{
						_errorLog = new ApplicationLog( );
						StringBuilder internalAPIAppVersion = new StringBuilder( typeof( LogManager ).Assembly.GetName( ).Version.ToString( ) );
						_errorLog.Init( "SEModAPIInternal_Error.log", internalAPIAppVersion );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( _errorLog == null )
					throw new Exception( "Failed to create error log!" );

				return _errorLog;
			}
		}

		#endregion "Properties"
	}
}