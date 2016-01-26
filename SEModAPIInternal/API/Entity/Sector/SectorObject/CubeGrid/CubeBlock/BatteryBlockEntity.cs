namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Game.EntityComponents;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class BatteryBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private BatteryBlockNetworkManager m_batteryBlockNetManager;

		private float m_maxPowerOutput;
		private float m_maxStoredPower;

		//Internal class
		public static string BatteryBlockNamespace = "Sandbox.Game.Entities";

		public static string BatteryBlockClass = "MyBatteryBlock";

		//Internal methods
		public static string BatteryBlockGetCurrentStoredPowerMethod = "get_CurrentStoredPower";

		public static string BatteryBlockSetCurrentStoredPowerMethod = "set_CurrentStoredPower";
		public static string BatteryBlockGetMaxStoredPowerMethod = "get_MaxStoredPower";
		public static string BatteryBlockSetMaxStoredPowerMethod = "set_MaxStoredPower";
		public static string BatteryBlockGetProducerEnabledMethod = "get_ProductionEnabled";
		public static string BatteryBlockSetProducerEnabledMethod = "set_ProductionEnabled";
		public static string BatteryBlockGetSemiautoEnabledMethod = "get_SemiautoEnabled";
		public static string BatteryBlockSetSemiautoEnabledMethod = "set_SemiautoEnabled";
		public const string BatteryBlockSourceCompProperty = "SourceComp";

		//Internal fields
		public static string BatteryBlockCurrentStoredPowerField = "m_currentStoredPower";

		public static string BatteryBlockMaxStoredPowerField = "m_maxStoredPower";
		public static string BatteryBlockProducerEnabledField = "m_producerEnabled";
		public static string BatteryBlockSemiautoEnabledField = "m_semiautoEnabled";
		public static string BatteryBlockBatteryDefinitionField = "m_batteryBlockDefinition";
		public static string BatteryBlockNetManagerField = "SyncObject";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BatteryBlockEntity( CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition )
			: base( parent, definition )
		{
		}

		public BatteryBlockEntity( CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_maxPowerOutput = 0;
			m_maxStoredPower = definition.MaxStoredPower;

			m_batteryBlockNetManager = new BatteryBlockNetworkManager( this, InternalGetNetManager( ) );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Battery Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( BatteryBlockNamespace, BatteryBlockClass );
				return type;
			}
		}

		[IgnoreDataMember]
		[Category( "Battery Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_BatteryBlock ObjectBuilder
		{
			get
			{
				MyObjectBuilder_BatteryBlock batteryBlock = (MyObjectBuilder_BatteryBlock)base.ObjectBuilder;

				batteryBlock.MaxStoredPower = m_maxStoredPower;

				return batteryBlock;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Battery Block" )]
		[DisplayName("Current Stored Power (MWh)")]
		public float CurrentStoredPower
		{
			get { return ObjectBuilder.CurrentStoredPower; }
			set
			{
				if ( ObjectBuilder.CurrentStoredPower == value ) return;
				ObjectBuilder.CurrentStoredPower = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBatteryBlockCurrentStoredPower );
				}
			}
		}

		[DataMember]
		[Category( "Battery Block" )]
		[DisplayName("Maximum Stored Power (MWh)")]
		[ReadOnly(true)]
		public float MaxStoredPower
		{
			get
			{
				float maxStoredPower = 0;

				if ( ActualObject != null )
				{
					maxStoredPower = (float)InvokeEntityMethod( ActualObject, BatteryBlockGetMaxStoredPowerMethod );
				}
				else
				{
					maxStoredPower = m_maxStoredPower;
				}

				return maxStoredPower;
			}
			set
			{
				if ( m_maxStoredPower == value ) return;
				m_maxStoredPower = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBatteryBlockMaxStoredPower );
				}
			}
		}

		[DataMember]
		[Category( "Battery Block" )]
		[DisplayName("Discharge")]
		public bool ProducerEnabled
		{
			get { return ObjectBuilder.ProducerEnabled; }
			set
			{
				if ( ObjectBuilder.ProducerEnabled == value ) return;
				ObjectBuilder.ProducerEnabled = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBatteryBlockProducerEnabled );
				}
			}
		}

		[DataMember]
		[Category( "Battery Block" )]
		[DisplayName("Semi-Automatic")]
		public bool SemiautoEnabled
		{
			get { return ObjectBuilder.SemiautoEnabled; }
			set
			{
				if ( ObjectBuilder.SemiautoEnabled == value ) return;
				ObjectBuilder.SemiautoEnabled = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBatteryBlockSemiautoEnabled );
				}
			}
		}

		[DataMember]
		[Category( "Battery Block" )]
		[ReadOnly(true)]
		[DisplayName("Maximum Power Output")]
		public float MaxPowerOutput
		{
			get { return m_maxPowerOutput; }
			set
			{
				if ( m_maxPowerOutput == value ) return;
				m_maxPowerOutput = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBatteryBlockMaxPowerOutput );
				}
			}
		}

		[IgnoreDataMember]
		[Browsable( false )]
		[ReadOnly( true )]
		internal BatteryBlockNetworkManager BatteryNetManager
		{
			get { return m_batteryBlockNetManager; }
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( BatteryBlockNamespace, BatteryBlockClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for BatteryBlockEntity" );

				result &= Reflection.HasMethod( type, BatteryBlockGetCurrentStoredPowerMethod );
				result &= Reflection.HasProperty( type, "CurrentStoredPower" );
				result &= Reflection.HasMethod( type, BatteryBlockGetMaxStoredPowerMethod );
				result &= Reflection.HasMethod( type, BatteryBlockSetMaxStoredPowerMethod );
				result &= Reflection.HasMethod( type, BatteryBlockGetSemiautoEnabledMethod );
				result &= Reflection.HasMethod( type, BatteryBlockSetSemiautoEnabledMethod );

				result &= Reflection.HasField( type, BatteryBlockCurrentStoredPowerField );
				result &= Reflection.HasField( type, BatteryBlockMaxStoredPowerField );
				result &= Reflection.HasField( type, BatteryBlockSemiautoEnabledField );
				result &= Reflection.HasField( type, BatteryBlockBatteryDefinitionField );
				result &= Reflection.HasField( type, BatteryBlockNetManagerField );

				result &= Reflection.HasProperty( type, BatteryBlockSourceCompProperty );

				return result;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetNetManager( )
		{
			try
			{
				FieldInfo field = GetEntityField( ActualObject, BatteryBlockNetManagerField );
				Object result = field.GetValue( ActualObject );

				return result;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected void InternalUpdateBatteryBlockCurrentStoredPower( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, BatteryBlockSetCurrentStoredPowerMethod, new object[ ] { CurrentStoredPower } );
				BatteryNetManager.BroadcastCurrentStoredPower( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateBatteryBlockMaxStoredPower( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, BatteryBlockSetMaxStoredPowerMethod, new object[ ] { m_maxStoredPower } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateBatteryBlockProducerEnabled( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, BatteryBlockSetProducerEnabledMethod, new object[ ] { ProducerEnabled } );
				BatteryNetManager.BroadcastProducerEnabled( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateBatteryBlockSemiautoEnabled( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, BatteryBlockSetSemiautoEnabledMethod, new object[ ] { SemiautoEnabled } );
				BatteryNetManager.BroadcastSemiautoEnabled( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateBatteryBlockMaxPowerOutput( )
		{
			//TODO - Do stuff
		}

		#endregion "Internal"

		#endregion "Methods"
	}

	public class BatteryBlockNetworkManager
	{
		#region "Attributes"

		private BatteryBlockEntity m_parent;
		private Object m_backingObject;

		private static bool m_isRegistered;

		public static string BatteryBlockNetworkManagerNamespace = BatteryBlockEntity.BatteryBlockNamespace + "." + BatteryBlockEntity.BatteryBlockClass;
		public static string BatteryBlockNetworkManagerClass = "MySyncBatteryBlock";

		public static string BatteryBlockNetManagerBroadcastProducerEnabledMethod = "SendProducerEnableChange";
		public static string BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod = "CapacityChange";
		public static string BatteryBlockNetManagerBroadcastSemiautoEnabledMethod = "SendSemiautoEnableChange";
		public static string BatteryBlockNetManagerCurrentPowerPacketReceiver = "CapacityChange";

		///////////////////////////////////////////////////////////////////////

		//Packets
		//1587 - CurrentStoredPower
		//1588 - ??
		//15870 - ProducerEnabled On/Off
		//15871 - SemiautoEnabled On/Off

		//public static string BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod = "300F0FF97B3FABBCEBB539E8935D6930";
		//public static string BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod = "12133389A918B17D9822AB1721C55497";

		public static string BatteryBlockNetManagerCurrentStoredPowerPacketClass = "CapacityMsg";
		public static string BatteryBlockNetManagerCurrentStoredPowerPacketValueField = "capacity";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BatteryBlockNetworkManager( BatteryBlockEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;

			MySandboxGame.Static.Invoke( RegisterPacketHandlers );
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
					throw new Exception( "Could not find internal type for BatteryBlockNetworkManager" );
				result &= Reflection.HasMethod( type, BatteryBlockNetManagerBroadcastProducerEnabledMethod );
				result &= Reflection.HasMethod( type, BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod );
				result &= Reflection.HasMethod( type, BatteryBlockNetManagerBroadcastSemiautoEnabledMethod );

				Type packetType = InternalType.GetNestedType( BatteryBlockNetManagerCurrentStoredPowerPacketClass, BindingFlags.Public | BindingFlags.NonPublic );
				//result &= BaseObject.HasMethod( packetType, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod );
				result &= Reflection.HasField( packetType, BatteryBlockNetManagerCurrentStoredPowerPacketValueField );
				//				result &= BaseObject.HasField(packetType, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdField);

				Type refPacketType = packetType.MakeByRefType( );

				return result;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void BroadcastProducerEnabled( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, BatteryBlockNetManagerBroadcastProducerEnabledMethod, new object[ ] { m_parent.ProducerEnabled } );
		}

		public void BroadcastCurrentStoredPower( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod, new object[ ] { m_parent.CurrentStoredPower } );
		}

		public void BroadcastSemiautoEnabled( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, BatteryBlockNetManagerBroadcastSemiautoEnabledMethod, new object[ ] { m_parent.SemiautoEnabled } );
		}

		protected static void RegisterPacketHandlers( )
		{
			try
			{
				if ( m_isRegistered )
					return;

				Type packetType = InternalType.GetNestedType( BatteryBlockNetManagerCurrentStoredPowerPacketClass, BindingFlags.Public | BindingFlags.NonPublic );
				MethodInfo method = typeof( BatteryBlockNetworkManager ).GetMethod( "ReceiveCurrentPowerPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static );
				bool result = NetworkManager.RegisterCustomPacketHandler( PacketRegistrationType.Static, packetType, method, InternalType );
				if ( !result )
					return;

				m_isRegistered = true;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
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
				MethodInfo basePacketHandlerMethod = BaseObject.GetStaticMethod( InternalType, BatteryBlockNetManagerCurrentPowerPacketReceiver, new Type[ ] { refPacketType, netManager.GetType( ) } );
				basePacketHandlerMethod.Invoke( null, new object[ ] { packet, netManager } );
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}