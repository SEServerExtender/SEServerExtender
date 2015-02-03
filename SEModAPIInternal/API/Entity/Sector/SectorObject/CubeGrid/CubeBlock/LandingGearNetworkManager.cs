namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Reflection;

	public class LandingGearNetworkManager
	{
		#region "Attributes"

		private LandingGearEntity m_parent;
		private Object m_backingObject;

		public static string LandingGearNetworkManagerNamespace = LandingGearEntity.LandingGearNamespace + "." + LandingGearEntity.LandingGearClass;
		public static string LandingGearNetworkManagerClass = "26556F6F0AE7CF1827348C8BE3041E52";

		public static string LandingGearNetworkManagerBroadcastIsLockedMethod = "486EB5B14ECC3CFCBB6A41DC47E8E457";
		public static string LandingGearNetworkManagerBroadcastAutoLockMethod = "EE7AB0648967FCDF7B20E1C359BC67E0";
		public static string LandingGearNetworkManagerBroadcastBrakeForceMethod = "78A3CD1FD04B6E57EB1053EE5E3F1CF7";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public LandingGearNetworkManager( LandingGearEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		internal Object BackingObject
		{
			get { return m_backingObject; }
		}

		public static Type InternalType
		{
			get
			{
				Type type = LandingGearEntity.InternalType.GetNestedType( LandingGearNetworkManagerClass, BindingFlags.Public | BindingFlags.NonPublic );
				return type;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for LandingGearNetworkManager" );
				result &= BaseObject.HasMethod( type, LandingGearNetworkManagerBroadcastIsLockedMethod );
				result &= BaseObject.HasMethod( type, LandingGearNetworkManagerBroadcastAutoLockMethod );
				result &= BaseObject.HasMethod( type, LandingGearNetworkManagerBroadcastBrakeForceMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void BroadcastIsLocked( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, LandingGearNetworkManagerBroadcastIsLockedMethod, new object[ ] { m_parent.IsLocked } );
		}

		public void BroadcastAutoLock( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, LandingGearNetworkManagerBroadcastAutoLockMethod, new object[ ] { m_parent.AutoLock } );
		}

		public void BroadcastBrakeForce( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, LandingGearNetworkManagerBroadcastBrakeForceMethod, new object[ ] { m_parent.BrakeForce } );
		}

		#endregion "Methods"
	}
}