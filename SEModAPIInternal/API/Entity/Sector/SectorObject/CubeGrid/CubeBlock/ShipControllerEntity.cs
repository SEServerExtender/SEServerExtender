using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract]
	public class ShipControllerEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private readonly ShipControllerNetworkManager _networkManager;
		//private CharacterEntity _pilot;
		//private bool _weaponStatus;

		public static string ShipControllerEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ShipControllerEntityClass = "12BACAB3471C8707CE7420AE0465548C";

		public static string ShipControllerEntityGetNetworkManager = "4D19E6CD06284069B97E08353C984ABB";

		//public static string ShipControllerEntityGetPilotEntityMethod = "19CFD162A750443F856D37B6C946BFB0";
		public static string ShipControllerEntityGetPilotEntityMethod = "6DF6AE137CABD37D44B48CDD8802E82A";

		public static string ShipControllerEntitySetPilotEntityMethod = "AC280CA879823319A66F3C71D6478297";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ShipControllerEntity( CubeGridEntity parent, MyObjectBuilder_ShipController definition )
			: base( parent, definition )
		{
		}

		public ShipControllerEntity( CubeGridEntity parent, MyObjectBuilder_ShipController definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			_networkManager = new ShipControllerNetworkManager( GetShipControllerNetworkManager( ), this );
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

					if (_pilot == null)
					{
						try
						{
							MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder(backingPilot);
							_pilot = new CharacterEntity(objectBuilder, backingPilot);
						}
						catch (Exception ex)
						{
							ApplicationLog.BaseLog.Error(ex);
						}
					}

					if (_pilot != null)
					{
						try
						{
							if (_pilot.BackingObject != backingPilot)
							{
								MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder(backingPilot);
								_pilot.BackingObject = backingPilot;
								_pilot.ObjectBuilder = objectBuilder;
							}
						}
						catch (Exception ex)
						{
							ApplicationLog.BaseLog.Error(ex);
						}
					}

					return _pilot;
				}
				set
				{
					_pilot = value;
					Changed = true;

					if (BackingObject != null && ActualObject != null)
					{
						Action action = InternalUpdatePilotEntity;
						SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
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
			get { return _networkManager; }
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
					throw new TypeLoadException( "Could not find type for ShipControllerEntity" );

				result &= BaseObject.HasMethod( type, ShipControllerEntityGetNetworkManager );
				//				result &= BaseObject.HasMethod(type, ShipControllerEntityGetPilotEntityMethod);
				//				result &= BaseObject.HasMethod(type, ShipControllerEntitySetPilotEntityMethod);

				return result;
			}
			catch ( TypeLoadException ex )
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
			if (_pilot == null || _pilot.BackingObject == null)
				return;

			BaseObject.InvokeEntityMethod(ActualObject, ShipControllerEntitySetPilotEntityMethod, new object[] { _pilot.BackingObject, Type.Missing, Type.Missing });
		}
		*/

		#endregion "Methods"
	}
}