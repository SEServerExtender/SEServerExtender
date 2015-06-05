namespace SEModAPI.API.Sandbox
{
	using System;
	using NLog;
	using SEModAPI.API.Utility;

	public class MySandboxGameWrapper
	{
		private static readonly Logger Log = LogManager.GetLogger( "BaseLog" );
		public static bool IsGameStarted
		{
			get
			{
				try
				{
					bool someValue = (bool) Reflection.GetEntityFieldValue( global::Sandbox.MySandboxGame.Static, "isFirstUpdateDone", true );
					return someValue;
				}
				catch ( Exception ex )
				{
					return false;
				}
			}
		}

	}
}
