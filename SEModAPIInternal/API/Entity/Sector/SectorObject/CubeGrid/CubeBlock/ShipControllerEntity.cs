namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract( Name = "ShipControllerEntityProxy" )]
	public class ShipControllerEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private ShipControllerNetworkManager m_networkManager;
		private CharacterEntity m_pilot;
		private bool m_weaponStatus;

		public static string ShipControllerEntityNamespace = "Sandbox.Game.Entities";
		public static string ShipControllerEntityClass = "MyShipController";

		public static string ShipControllerEntityGetNetworkManager = "get_SyncObject";

		//public static string ShipControllerEntityGetPilotEntityMethod = "19CFD162A750443F856D37B6C946BFB0";
		public static string ShipControllerEntityGetPilotEntityMethod = "Sandbox.Game.GameSystems.Electricity.IMyRechargeSocketOwner.get_RechargeSocket";

		//public static string ShipControllerEntitySetPilotEntityMethod = "AC280CA879823319A66F3C71D6478297";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ShipControllerEntity( CubeGridEntity parent, MyObjectBuilder_ShipController definition )
			: base( parent, definition )
		{
		}

		public ShipControllerEntity( CubeGridEntity parent, MyObjectBuilder_ShipController definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_networkManager = new ShipControllerNetworkManager( GetShipControllerNetworkManager( ), this );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Ship Controller" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_ShipController ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_ShipController)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Ship Controller" )]
		[ReadOnly( true )]
		public bool ControlThrusters
		{
			get { return ObjectBuilder.ControlThrusters; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Ship Controller" )]
		[ReadOnly( true )]
		public bool ControlWheels
		{
			get { return ObjectBuilder.ControlWheels; }
			private set
			{
				//Do nothing!
			}
		}

		/*
			[IgnoreDataMember]
			[Category("Ship Controller")]
			[ReadOnly(true)]
			public MyObjectBuilder_AutopilotBase Autopilot
			{
				get
				{
					return null;
				}
				private set
				{
					//Do nothing!
				}
			}

			[IgnoreDataMember]
			[Category("Ship Controller")]
			[Browsable(false)]
			[ReadOnly(true)]
			[Obsolete]
			public MyObjectBuilder_Character Pilot
			{
				get {
					return null;
				}
				private set
				{
					//Do nothing!
				}
			}

			[IgnoreDataMember]
			[Category("Ship Controller")]
			[Browsable(false)]
			public CharacterEntity PilotEntity
			{
				get
				{
					if (BackingObject == null || ActualObject == null)
						return null;

					Object backingPilot = GetPilotEntity();
					if (backingPilot == null)
						return null;

					if (m_pilot == null)
					{
						try
						{
							MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder(backingPilot);
							m_pilot = new CharacterEntity(objectBuilder, backingPilot);
						}
						catch (Exception ex)
						{
							ApplicationLog.BaseLog.Error(ex);
						}
					}

					if (m_pilot != null)
					{
						try
						{
							if (m_pilot.BackingObject != backingPilot)
							{
								MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder(backingPilot);
								m_pilot.BackingObject = backingPilot;
								m_pilot.ObjectBuilder = objectBuilder;
							}
						}
						catch (Exception ex)
						{
							ApplicationLog.BaseLog.Error(ex);
						}
					}

					return m_pilot;
				}
				set
				{
					m_pilot = value;
					Changed = true;

					if (BackingObject != null && ActualObject != null)
					{
						Action action = InternalUpdatePilotEntity;
						MySandboxGame.Static.Invoke(action);
					}
				}
			}
		*/

		[IgnoreDataMember]
		[Category( "Ship Controller" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal ShipControllerNetworkManager NetworkManager
		{
			get { return m_networkManager; }
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ShipControllerEntityNamespace, ShipControllerEntityClass );
				if ( type == null )
					throw new Exception( "Could not find type for ShipControllerEntity" );

				result &= Reflection.HasMethod( type, ShipControllerEntityGetNetworkManager );
				//				result &= BaseObject.HasMethod(type, ShipControllerEntityGetPilotEntityMethod);
				//				result &= BaseObject.HasMethod(type, ShipControllerEntitySetPilotEntityMethod);

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected Object GetShipControllerNetworkManager( )
		{
			Object result = InvokeEntityMethod( ActualObject, ShipControllerEntityGetNetworkManager );
			return result;
		}

		/*
		protected Object GetPilotEntity()
		{
			Object result = InvokeEntityMethod(ActualObject, ShipControllerEntityGetPilotEntityMethod);
			return result;
		}

		protected void InternalUpdatePilotEntity()
		{
			if (m_pilot == null || m_pilot.BackingObject == null)
				return;

			BaseObject.InvokeEntityMethod(ActualObject, ShipControllerEntitySetPilotEntityMethod, new object[] { m_pilot.BackingObject, Type.Missing, Type.Missing });
		}
		*/

		#endregion "Methods"
	}

	public class ShipControllerNetworkManager
	{
		#region "Attributes"

		private Object m_networkManager;
		private ShipControllerEntity m_parent;

		private static bool m_isRegistered;

		//Packets
		//2480 - Pilot Relative World PositionOrientation
		//2481 - Dampeners On/Off
		//2487 - Autopilot Data
		//2488 - Thruster Power On/Off
		//2489 - Motor Handbrake On/Off

		public static string ShipControllerNetworkManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string ShipControllerNetworkManagerClass = "MySyncShipController";

		public static string ShipControllerNetworkManagerBroadcastDampenersStatus = "SendDampenersUpdate";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ShipControllerNetworkManager( Object networkManager, ShipControllerEntity parent )
		{
			m_networkManager = networkManager;
			m_parent = parent;

			MySandboxGame.Static.Invoke( RegisterPacketHandlers );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public Object BackingObject
		{
			get { return m_networkManager; }
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
					throw new Exception( "Could not find type for ShipControllerNetworkManager" );

				result &= Reflection.HasMethod( type, ShipControllerNetworkManagerBroadcastDampenersStatus );

				return result;
			}
			catch ( Exception ex )
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
			try
			{
				if ( m_isRegistered )
					return;
				/*
				Type packetType = InternalType.GetNestedType("8368ACD3E728CDA04FE741CDC05B1D16", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(ShipControllerNetworkManager).GetMethod("ReceivePositionOrientationPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				bool result = NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);
				if (!result)
					return;
				*/
				m_isRegistered = true;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected static void ReceivePositionOrientationPacket<T>( Object instanceNetManager, ref T packet, Object masterNetManager ) where T : struct
		{
			try
			{
				//For now we ignore any inbound packets that set the positionorientation
				//This prevents the clients from having any control over the actual ship position
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}