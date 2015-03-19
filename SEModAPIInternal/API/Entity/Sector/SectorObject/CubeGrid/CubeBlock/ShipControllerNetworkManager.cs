namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class ShipControllerNetworkManager
	{
		#region "Attributes"

		private readonly object _networkManager;
		//private ShipControllerEntity _parent;

		private static bool _isRegistered;

		//Packets
		//2480 - Pilot Relative World PositionOrientation
		//2481 - Dampeners On/Off
		//2487 - Autopilot Data
		//2488 - Thruster Power On/Off
		//2489 - Motor Handbrake On/Off

		public static string ShipControllerNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string ShipControllerNetworkManagerClass = "FC3A3372AD1F9E2E193FE3F7683D7DEF";

		public static string ShipControllerNetworkManagerBroadcastDampenersStatus = "7D17A6F76089A3756ED081F5CCB0E739";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ShipControllerNetworkManager( Object networkManager, ShipControllerEntity parent )
		{
			_networkManager = networkManager;
			//_parent = parent;

			Action action = RegisterPacketHandlers;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public Object BackingObject
		{
			get { return _networkManager; }
		}

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ShipControllerNetworkManagerNamespace, ShipControllerNetworkManagerClass );
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
					throw new TypeLoadException( "Could not find type for ShipControllerNetworkManager" );

				result &= BaseObject.HasMethod( type, ShipControllerNetworkManagerBroadcastDampenersStatus );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		internal void BroadcastDampenersStatus( bool status )
		{
			BaseObject.InvokeEntityMethod( BackingObject, ShipControllerNetworkManagerBroadcastDampenersStatus, new object[ ] { status } );
		}

		protected static void RegisterPacketHandlers( )
		{
			//try
			//{
				if ( _isRegistered )
					return;
				/*
				Type packetType = InternalType.GetNestedType("8368ACD3E728CDA04FE741CDC05B1D16", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(ShipControllerNetworkManager).GetMethod("ReceivePositionOrientationPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				bool result = NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);
				if (!result)
					return;
				*/
				_isRegistered = true;
			//}
			//catch ( Exception ex )
			//{
			//	ApplicationLog.BaseLog.Error( ex );
			//}
		}

		protected static void ReceivePositionOrientationPacket<T>( Object instanceNetManager, ref T packet, Object masterNetManager ) where T : struct
		{
			//try
			//{
			//	//For now we ignore any inbound packets that set the positionorientation
			//	//This prevents the clients from having any control over the actual ship position
			//}
			//catch ( Exception ex )
			//{
			//	ApplicationLog.BaseLog.Error( ex );
			//}
		}

		#endregion "Methods"
	}
}