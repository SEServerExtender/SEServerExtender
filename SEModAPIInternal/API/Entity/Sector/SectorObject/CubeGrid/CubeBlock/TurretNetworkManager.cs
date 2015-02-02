namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using SEModAPIInternal.API.Common;

	public class TurretNetworkManager
	{
		#region "Attributes"

		private TurretBaseEntity m_parent;
		private Object m_backingObject;

		//Packets
		//686 - Target ID
		//687 - Range
		//688 - Target settings (meteor on/off, missile on/off, moving on/off)

		public static string TurretNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string TurretNetworkManagerClass = "D29C33B546169292854A4DECCDB53895";

		public static string TurretNetworkManagerBroadcastTargetIdMethod = "8E250BEF8500A92D96D13554C31D4EFA";
		public static string TurretNetworkManagerBroadcastRangeMethod = "8F298724F106679C0229CD9F9763EA90";
		public static string TurretNetworkManagerBroadcastTargettingFlagsMethod = "70D4690C2D4A199ABB3EB37364BE0595";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public TurretNetworkManager( TurretBaseEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		internal Object BackingObject
		{
			get { return m_backingObject; }
			set { m_backingObject = value; }
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( TurretNetworkManagerNamespace, TurretNetworkManagerClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for TurretNetworkManager" );

				result &= BaseObject.HasMethod( type, TurretNetworkManagerBroadcastTargetIdMethod );
				result &= BaseObject.HasMethod( type, TurretNetworkManagerBroadcastRangeMethod );
				result &= BaseObject.HasMethod( type, TurretNetworkManagerBroadcastTargettingFlagsMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void BroadcastTargetId( )
		{
			long entityId = BaseEntity.GetEntityId( m_parent.ActualObject );
			BaseObject.InvokeEntityMethod( BackingObject, TurretNetworkManagerBroadcastTargetIdMethod, new object[ ] { entityId, false } );
		}

		public void BroadcastRange( )
		{
			float range = m_parent.ShootingRange;
			BaseObject.InvokeEntityMethod( BackingObject, TurretNetworkManagerBroadcastRangeMethod, new object[ ] { range } );
		}

		public void BroadcastTargettingFlags( )
		{
			bool targetMeteors = m_parent.TargetMeteors;
			bool targetMoving = m_parent.TargetMoving;
			bool targetMissiles = m_parent.TargetMissiles;
			BaseObject.InvokeEntityMethod( BackingObject, TurretNetworkManagerBroadcastTargettingFlagsMethod, new object[ ] { targetMeteors, targetMoving, targetMissiles } );
		}

		#endregion "Methods"
	}
}