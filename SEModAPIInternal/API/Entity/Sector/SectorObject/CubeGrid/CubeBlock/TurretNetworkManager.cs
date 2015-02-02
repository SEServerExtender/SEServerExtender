namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using SEModAPIInternal.API.Common;

	public class TurretNetworkManager
	{
		#region "Attributes"

		private readonly TurretBaseEntity _parent;

		//Packets
		//686 - Target ID
		//687 - Range
		//688 - Target settings (meteor on/off, missile on/off, moving on/off)

		public static string TurretNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string TurretNetworkManagerClass = "D29C33B546169292854A4DECCDB53895";

		public static string TurretNetworkManagerBroadcastTargetIdMethod = "8E250BEF8500A92D96D13554C31D4EFA";
		public static string TurretNetworkManagerBroadcastRangeMethod = "8F298724F106679C0229CD9F9763EA90";
		public static string TurretNetworkManagerBroadcastTargetingFlagsMethod = "70D4690C2D4A199ABB3EB37364BE0595";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public TurretNetworkManager( TurretBaseEntity parent, Object backingObject )
		{
			_parent = parent;
			BackingObject = backingObject;
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		internal Object BackingObject { get; set; }

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
				result &= BaseObject.HasMethod( type, TurretNetworkManagerBroadcastTargetingFlagsMethod );

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
			long entityId = BaseEntity.GetEntityId( _parent.ActualObject );
			BaseObject.InvokeEntityMethod( BackingObject, TurretNetworkManagerBroadcastTargetIdMethod, new object[ ] { entityId, false } );
		}

		public void BroadcastRange( )
		{
			float range = _parent.ShootingRange;
			BaseObject.InvokeEntityMethod( BackingObject, TurretNetworkManagerBroadcastRangeMethod, new object[ ] { range } );
		}

		public void BroadcastTargettingFlags( )
		{
			bool targetMeteors = _parent.TargetMeteors;
			bool targetMoving = _parent.TargetMoving;
			bool targetMissiles = _parent.TargetMissiles;
			BaseObject.InvokeEntityMethod( BackingObject, TurretNetworkManagerBroadcastTargetingFlagsMethod, new object[ ] { targetMeteors, targetMoving, targetMissiles } );
		}

		#endregion "Methods"
	}
}