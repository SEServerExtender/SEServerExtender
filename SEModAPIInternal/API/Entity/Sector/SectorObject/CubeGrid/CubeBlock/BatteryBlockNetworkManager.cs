namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Reflection;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class BatteryBlockNetworkManager
	{
		#region "Attributes"

		private readonly BatteryBlockEntity _parent;
		private readonly Object _backingObject;

		private static bool _isRegistered;

		public static string BatteryBlockNetworkManagerNamespace = string.Format( "{0}.{1}", BatteryBlockEntity.BatteryBlockNamespace, BatteryBlockEntity.BatteryBlockClass );
		public static string BatteryBlockNetworkManagerClass = "6704740496C47C5FDE69887798D17883";

		public static string BatteryBlockNetManagerBroadcastProducerEnabledMethod = "280D7AE8C0F523FF089618970C13B55B";
		public static string BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod = "F512BA7EF29F6A8B7DE3D56BAAC0207B";
		public static string BatteryBlockNetManagerBroadcastSemiautoEnabledMethod = "72CE36DE9C0BAB6FEADA5D10CF5B867A";
		public static string BatteryBlockNetManagerCurrentPowerPacketReceiver = "F512BA7EF29F6A8B7DE3D56BAAC0207B";

		///////////////////////////////////////////////////////////////////////

		//Packets
		//1587 - CurrentStoredPower
		//1588 - ??
		//15870 - ProducerEnabled On/Off
		//15871 - SemiautoEnabled On/Off

		//public static string BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod = "300F0FF97B3FABBCEBB539E8935D6930";
		//public static string BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod = "12133389A918B17D9822AB1721C55497";

		public static string BatteryBlockNetManagerCurrentStoredPowerPacketClass = "59DE66D2ECADE0929A1C776D7FA907E2";
		public static string BatteryBlockNetManagerCurrentStoredPowerPacketValueField = "ADC3AB91A03B31875821D57B8B718AF5";


		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BatteryBlockNetworkManager( BatteryBlockEntity parent, Object backingObject )
		{
			_parent = parent;
			_backingObject = backingObject;

			Action action = RegisterPacketHandlers;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		internal Object BackingObject
		{
			get { return _backingObject; }
		}

		public static Type InternalType
		{
			get
			{
				Type type = BatteryBlockEntity.InternalType.GetNestedType( BatteryBlockNetworkManagerClass, BindingFlags.Public | BindingFlags.NonPublic );
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
					throw new TypeLoadException( "Could not find internal type for BatteryBlockNetworkManager" );
				result &= BaseObject.HasMethod( type, BatteryBlockNetManagerBroadcastProducerEnabledMethod );
				result &= BaseObject.HasMethod( type, BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod );
				result &= BaseObject.HasMethod( type, BatteryBlockNetManagerBroadcastSemiautoEnabledMethod );

				Type packetType = InternalType.GetNestedType( BatteryBlockNetManagerCurrentStoredPowerPacketClass, BindingFlags.Public | BindingFlags.NonPublic );
				//result &= BaseObject.HasMethod( packetType, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod );
				result &= BaseObject.HasField( packetType, BatteryBlockNetManagerCurrentStoredPowerPacketValueField );
				//				result &= BaseObject.HasField(packetType, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdField);

				Type refPacketType = packetType.MakeByRefType( );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				Console.WriteLine( ex );
				return false;
			}
			catch ( ArgumentNullException ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void BroadcastProducerEnabled( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, BatteryBlockNetManagerBroadcastProducerEnabledMethod, new object[ ] { _parent.ProducerEnabled } );
		}

		public void BroadcastCurrentStoredPower( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod, new object[ ] { _parent.CurrentStoredPower } );
		}

		public void BroadcastSemiautoEnabled( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, BatteryBlockNetManagerBroadcastSemiautoEnabledMethod, new object[ ] { _parent.SemiautoEnabled } );
		}

		protected static void RegisterPacketHandlers( )
		{
			try
			{
				if ( _isRegistered )
					return;

				Type packetType = InternalType.GetNestedType( BatteryBlockNetManagerCurrentStoredPowerPacketClass, BindingFlags.Public | BindingFlags.NonPublic );
				MethodInfo method = typeof( BatteryBlockNetworkManager ).GetMethod( "ReceiveCurrentPowerPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static );
				bool result = NetworkManager.RegisterCustomPacketHandler( PacketRegistrationType.Static, packetType, method, InternalType );
				if ( !result )
					return;

				_isRegistered = true;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		protected static void ReceiveCurrentPowerPacket<T>( ref T packet, Object netManager ) where T : struct
		{
			try
			{
				//object result = BaseObject.InvokeEntityMethod( packet, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod );
				//object result = BaseObject.GetEntityFieldValue(packet, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdField);
				object result = null;
				if ( result == null )
					return;
				long entityId = (long)result;
				BaseObject matchedEntity = GameEntityManager.GetEntity( entityId );
				if ( matchedEntity == null )
					return;
				if ( !( matchedEntity is BatteryBlockEntity ) )
					return;
				BatteryBlockEntity battery = (BatteryBlockEntity)matchedEntity;

				result = BaseObject.GetEntityFieldValue( packet, BatteryBlockNetManagerCurrentStoredPowerPacketValueField );
				if ( result == null )
					return;
				float packetPowerLevel = (float)result;
				if ( packetPowerLevel == 1.0f )
					return;

				BaseObject.SetEntityFieldValue( packet, BatteryBlockNetManagerCurrentStoredPowerPacketValueField, battery.CurrentStoredPower );

				Type refPacketType = packet.GetType( ).MakeByRefType( );
				MethodInfo basePacketHandlerMethod = BaseObject.GetStaticMethod( InternalType, BatteryBlockNetManagerCurrentPowerPacketReceiver, new[ ] { refPacketType, netManager.GetType( ) } );
				basePacketHandlerMethod.Invoke( null, new[ ] { packet, netManager } );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}