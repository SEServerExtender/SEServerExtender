namespace SEModAPIInternal.API.Common
{
	using System;
	using SEModAPIInternal.API.Entity;

	public class RadioManagerNetworkManager
	{
		#region "Attributes"

		private readonly RadioManager _parent;

		public static string RadioManagerNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string RadioManagerNetManagerClass = "1CE6F03E36FA552A8223DEBF0554411C";

		public static string RadioManagerNetManagerBroadcastRadiusMethod = "D98F28A89752B53473A94214E2D0E0EA";
		public static string RadioManagerNetManagerBroadcastEnabledMethod = "CCB8371C1B91E687E88786806386E87C";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public RadioManagerNetworkManager( RadioManager parent )
		{
			_parent = parent;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public object BackingObject
		{
			get
			{
				object result = _parent.GetNetworkManager( );
				return result;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( RadioManagerNetManagerNamespace, RadioManagerNetManagerClass );
				if ( type1 == null )
					throw new TypeLoadException( "Could not find internal type for RadioManagerNetworkManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, RadioManagerNetManagerBroadcastRadiusMethod );
				result &= BaseObject.HasMethod( type1, RadioManagerNetManagerBroadcastEnabledMethod );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void BroadcastRadius( )
		{
			Action action = InternalBroadcastRadius;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void BroadcastEnabled( )
		{
			Action action = InternalBroadcastEnabled;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#region "Internal"

		protected void InternalBroadcastRadius( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerNetManagerBroadcastRadiusMethod, new object[ ] { _parent.BroadcastRadius, true } );
		}

		protected void InternalBroadcastEnabled( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerNetManagerBroadcastEnabledMethod, new object[ ] { _parent.Enabled } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}