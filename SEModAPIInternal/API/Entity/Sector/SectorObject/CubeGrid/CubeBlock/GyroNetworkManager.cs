namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using SEModAPIInternal.API.Common;
	using VRageMath;

	public class GyroNetworkManager
	{
		#region "Attributes"

		private readonly GyroEntity _parent;
		private readonly Object _backingObject;

		public static string GyroNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string GyroNetworkManagerClass = "B4646791D7A57BE4E2EE21A1F22A4364";

		//Packet ID 7587
		public static string GyroNetworkManagerBroadcastOverrideMethod = "42AE4A855B87970C577E7D3B84925D3D";

		//Packet ID 7586
		public static string GyroNetworkManagerBroadcastPowerMethod = "4D6A36FFABF6AE6DA2E92DCA0ADBB5FD";

		//Packet ID 7588
		public static string GyroNetworkManagerBroadcastTargetAngularVelocityMethod = "8948F689D38487B17CF831F240CEEF08";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public GyroNetworkManager( GyroEntity parent, Object backingObject )
		{
			_parent = parent;
			_backingObject = backingObject;
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
					throw new TypeLoadException( "Could not find internal type for GyroNetworkManager" );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastOverrideMethod );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastPowerMethod );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastTargetAngularVelocityMethod );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				Console.WriteLine( ex );
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
			BaseObject.InvokeEntityMethod( _backingObject, GyroNetworkManagerBroadcastOverrideMethod, new object[ ] { _parent.GyroOverride } );
		}

		protected void InternalBroadcastPower( )
		{
			BaseObject.InvokeEntityMethod( _backingObject, GyroNetworkManagerBroadcastPowerMethod, new object[ ] { _parent.GyroPower } );
		}

		protected void InternalBroadcastTargetAngularVelocity( )
		{
			Vector3 newTarget = _parent.TargetAngularVelocity;
			BaseObject.InvokeEntityMethod( _backingObject, GyroNetworkManagerBroadcastTargetAngularVelocityMethod, new object[ ] { newTarget } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}