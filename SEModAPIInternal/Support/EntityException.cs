using System;
using SEModAPI.Support;

namespace SEModAPIInternal.Support
{
	public enum EntityExceptionState
	{
		Invalid,
		NotFound,
		FieldNotFound,
		MethodNotFound,
	}

	public class EntityException : AutoException
	{
		public EntityException( EntityExceptionState state, string additionnalInfo = "", Exception original = null )
			: base( state, additionnalInfo )
		{
			LogManager.ErrorLog.WriteLine( original );
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