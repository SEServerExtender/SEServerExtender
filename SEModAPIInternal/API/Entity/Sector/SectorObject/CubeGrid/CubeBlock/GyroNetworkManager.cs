namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;
	using VRageMath;

	public class GyroNetworkManager
	{
		#region "Attributes"

		private GyroEntity m_parent;
		private Object m_backingObject;

		public static string GyroNetworkManagerNamespace = "";
		public static string GyroNetworkManagerClass = "=oxJceWENGF516EExThNDB8OIOS=";

		//Packet ID 7587
		public static string GyroNetworkManagerBroadcastOverrideMethod = "SendGyroOverrideRequest";

		//Packet ID 7586
		public static string GyroNetworkManagerBroadcastPowerMethod = "SendChangeGyroPowerRequest";

		//Packet ID 7588
		public static string GyroNetworkManagerBroadcastTargetAngularVelocityMethod = "SendGyroTorqueRequest";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public GyroNetworkManager( GyroEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Initializers"



		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GyroNetworkManagerNamespace, GyroNetworkManagerClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GyroNetworkManager" );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastOverrideMethod );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastPowerMethod );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastTargetAngularVelocityMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void BroadcastOverride( )
		{
			Action action = InternalBroadcastOverride;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void BroadcastPower( )
		{
			Action action = InternalBroadcastPower;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void BroadcastTargetAngularVelocity( )
		{
			Action action = InternalBroadcastTargetAngularVelocity;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#region "Internal"

		protected void InternalBroadcastOverride( )
		{
			BaseObject.InvokeEntityMethod( m_backingObject, GyroNetworkManagerBroadcastOverrideMethod, new object[ ] { m_parent.GyroOverride } );
		}

		protected void InternalBroadcastPower( )
		{
			BaseObject.InvokeEntityMethod( m_backingObject, GyroNetworkManagerBroadcastPowerMethod, new object[ ] { m_parent.GyroPower } );
		}

		protected void InternalBroadcastTargetAngularVelocity( )
		{
			Vector3 newTarget = (Vector3)m_parent.TargetAngularVelocity;
			BaseObject.InvokeEntityMethod( m_backingObject, GyroNetworkManagerBroadcastTargetAngularVelocityMethod, new object[ ] { newTarget } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}