namespace SEModAPIInternal.Support
{
	using System;
	using NLog;
	using SEModAPI.Support;

	public enum EntityExceptionState
	{
		Invalid,
		NotFound,
		FieldNotFound,
		MethodNotFound,
	}

	public class EntityException : AutoException
	{
		private static readonly Logger Log = LogManager.GetLogger( "BaseLog" );
		public EntityException( EntityExceptionState state, string additionnalInfo = "", Exception original = null )
			: base( state, additionnalInfo )
		{
			Log.Error( original );
		}

		public new string[ ] StateRepresentation =
        {
            "Invalid",
			"NotFound",
			"FieldNotFound",
			"MethodNotFound",
        };
	}
}