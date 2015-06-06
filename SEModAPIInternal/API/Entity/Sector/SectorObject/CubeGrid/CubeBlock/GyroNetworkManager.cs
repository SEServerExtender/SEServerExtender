namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using Sandbox;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;
	using VRageMath;

	public class GyroNetworkManager
	{
		#region "Attributes"

		private GyroEntity m_parent;
		private Object m_backingObject;

		public static string GyroNetworkManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string GyroNetworkManagerClass = "MySyncGyro";

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
				result &= Reflection.HasMethod( type, GyroNetworkManagerBroadcastOverrideMethod );
				result &= Reflection.HasMethod( type, GyroNetworkManagerBroadcastPowerMethod );
				result &= Reflection.HasMethod( type, GyroNetworkManagerBroadcastTargetAngularVelocityMethod );

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
			MySandboxGame.Static.Invoke( InternalBroadcastOverride );
		}

		public void BroadcastPower( )
		{
			MySandboxGame.Static.Invoke( InternalBroadcastPower );
		}

		public void BroadcastTargetAngularVelocity( )
		{
			MySandboxGame.Static.Invoke( InternalBroadcastTargetAngularVelocity );
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