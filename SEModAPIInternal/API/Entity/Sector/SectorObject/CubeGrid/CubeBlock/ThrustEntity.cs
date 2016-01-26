namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.TypeConverters;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;
	using VRageMath;

	[DataContract]
	public class ThrustEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private float m_thrustOverride;
		private ThrustNetworkManager m_networkManager;

		public static string ThrustNamespace = "Sandbox.Game.Entities";
		public static string ThrustClass = "MyThrust";

		public static string ThrustGetOverrideMethod = "get_ThrustOverride";
		public static string ThrustSetOverrideMethod = "SetThrustOverride";
		public static string ThrustGetMaxThrustVectorMethod = "get_ThrustForce";
		public static string ThrustGetMaxPowerConsumptionMethod = "get_MaxPowerConsumption";
		public static string ThrustGetMinPowerConsumptionMethod = "get_MinPowerConsumption";

		public static string ThrustNetManagerField = "SyncObject";

		//Note: The following fields exist but are not broadcast and as such setting these on the server will do nothing client-side
		public static string ThrustFlameColorField = "m_thrustColor";

		public static string ThrustLightField = "m_light";
		public static string ThrustFlameScaleCoefficientField = "m_maxBillboardDistanceSquared";

		//Thrust flame scale coefficient values:
		//LargeShip-Large: 700
		//LargeShip-Small: 500
		//SmallShip-Large: 300
		//SmallShip-Small: 200

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ThrustEntity( CubeGridEntity parent, MyObjectBuilder_Thrust definition )
			: base( parent, definition )
		{
		}

		public ThrustEntity( CubeGridEntity parent, MyObjectBuilder_Thrust definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_thrustOverride = 0;
			m_networkManager = new ThrustNetworkManager( this, InternalGetThrustNetManager( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Thrust" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Thrust ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Thrust thrust = (MyObjectBuilder_Thrust)base.ObjectBuilder;

				return thrust;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Thrust" )]
		[Browsable( true )]
		[ReadOnly( false )]
		public float Override
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return m_thrustOverride;

				return InternalGetThrustOverride( );
			}
			set
			{
				m_thrustOverride = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateOverride );
				}
			}
		}

		[DataMember]
		[Category( "Thrust" )]
		[Browsable( true )]
		[ReadOnly( true )]
		public Vector3Wrapper MaxThrustVector
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return Vector3.Zero;

				return InternalGetMaxThrustVector( );
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ThrustNamespace, ThrustClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for ThrustEntity" );
				result &= Reflection.HasMethod( type, ThrustGetOverrideMethod );
				result &= Reflection.HasMethod( type, ThrustSetOverrideMethod );
				result &= Reflection.HasMethod( type, ThrustGetMaxThrustVectorMethod );
				result &= Reflection.HasMethod( type, ThrustGetMaxPowerConsumptionMethod );
				result &= Reflection.HasMethod( type, ThrustGetMinPowerConsumptionMethod );
				result &= Reflection.HasField( type, ThrustNetManagerField );

				return result;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetThrustNetManager( )
		{
			try
			{
				FieldInfo field = GetEntityField( ActualObject, ThrustNetManagerField );
				Object result = field.GetValue( ActualObject );

				return result;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected float InternalGetThrustOverride( )
		{
			float result = (float)InvokeEntityMethod( ActualObject, ThrustGetOverrideMethod );

			return result;
		}

		protected Vector3 InternalGetMaxThrustVector( )
		{
			Vector3 result = (Vector3)InvokeEntityMethod( ActualObject, ThrustGetMaxThrustVectorMethod );

			return result;
		}

		protected void InternalUpdateOverride( )
		{
			InvokeEntityMethod( ActualObject, ThrustSetOverrideMethod, new object[ ] { m_thrustOverride } );
			m_networkManager.BroadcastOverride( m_thrustOverride );
		}

		#endregion "Internal"

		#endregion "Methods"
	}

	public class ThrustNetworkManager
	{
		#region "Attributes"

		private ThrustEntity m_parent;
		private Object m_backingObject;

		private float m_lastOverride;

		public static string ThrustNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string ThrustNetManagerClass = "MySyncThruster";

		public static string ThrustNetManagerBroadcastOverrideMethod = "SendChangeThrustOverrideRequest";

		//Packets
		//7416 - Thrust override

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ThrustNetworkManager( ThrustEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Intializers"



		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ThrustNetManagerNamespace, ThrustNetManagerClass );
				if ( type == null )
					throw new Exception( "Could not find network manager type for ThrustEntity" );
				result &= Reflection.HasMethod( type, ThrustNetManagerBroadcastOverrideMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void BroadcastOverride( float overrideValue )
		{
			if ( m_backingObject == null )
				return;

			m_lastOverride = overrideValue;

			MySandboxGame.Static.Invoke( InternalBroadcastOverride );
		}

		protected void InternalBroadcastOverride( )
		{
			BaseObject.InvokeEntityMethod( m_backingObject, ThrustNetManagerBroadcastOverrideMethod, new object[ ] { m_lastOverride } );
		}

		#endregion "Methods"
	}
}