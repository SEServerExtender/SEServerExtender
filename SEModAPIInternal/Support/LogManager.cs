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

		private static LogManager m_instance;

		private static MyLog m_gameLog;
		private static ApplicationLog m_apiLog;
		private static ApplicationLog m_chatLog;
		private static ApplicationLog m_errorLog;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected LogManager( )
		{
			m_instance = this;

			Console.WriteLine( "Finished loading LogManager" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static LogManager Instance
		{
			get { return m_instance ?? ( m_instance = new LogManager( ) ); }
		}

		[Obsolete]
		public static MyLog GameLog
		{
			get
			{
				if ( m_gameLog == null )
				{
					LogManager temp = Instance;

					try
					{
						FieldInfo myLogField = BaseObject.GetStaticField( SandboxGameAssemblyWrapper.MainGameType, SandboxGameAssemblyWrapper.MainGameMyLogField );
						m_gameLog = (MyLog)myLogField.GetValue( null );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( m_gameLog == null )
					throw new Exception( "Failed to load game log!" );

				return m_gameLog;
			}
		}

		public static ApplicationLog APILog
		{
			get
			{
				if ( m_apiLog == null )
				{
					LogManager temp = Instance;

					try
					{
						m_apiLog = new ApplicationLog( true );
						StringBuilder internalAPIAppVersion = new StringBuilder( typeof( LogManager ).Assembly.GetName( ).Version.ToString( ) );
						m_apiLog.Init( "SEModAPIInternal.log", internalAPIAppVersion );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( m_apiLog == null )
					throw new Exception( "Failed to create API log!" );

				return m_apiLog;
			}
		}

		public static ApplicationLog ChatLog
		{
			get
			{
				if ( m_chatLog == null )
				{
					LogManager temp = Instance;

					try
					{
						m_chatLog = new ApplicationLog( true );
						StringBuilder internalAPIAppVersion = new StringBuilder( typeof( LogManager ).Assembly.GetName( ).Version.ToString( ) );
						m_chatLog.Init( "SEModAPIInternal_Chat.log", internalAPIAppVersion );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( m_chatLog == null )
					throw new Exception( "Failed to create chat log!" );

				return m_chatLog;
			}
		}

		public static ApplicationLog ErrorLog
		{
			get
			{
				if ( m_errorLog == null )
				{
					LogManager temp = Instance;

					try
					{
						m_errorLog = new ApplicationLog( );
						StringBuilder internalAPIAppVersion = new StringBuilder( typeof( LogManager ).Assembly.GetName( ).Version.ToString( ) );
						m_errorLog.Init( "SEModAPIInternal_Error.log", internalAPIAppVersion );
					}
					catch ( Exception ex )
					{
						Console.WriteLine( ex );
					}
				}

				if ( m_errorLog == null )
					throw new Exception( "Failed to create error log!" );

				return m_errorLog;
			}
		}

		#endregion "Properties"
	}
}