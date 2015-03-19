using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "ThrustEntityProxy" )]
	public class ThrustEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private float m_thrustOverride;
		private ThrustNetworkManager m_networkManager;

		public static string ThrustNamespace = "";
		public static string ThrustClass = "=Avs8k7NfpEpYw1QyLX0wdzrIq9=";

		public static string ThrustGetOverrideMethod = "get_ThrustOverride";
		public static string ThrustSetOverrideMethod = "SetThrustOverride";
		public static string ThrustGetMaxThrustVectorMethod = "get_ThrustForce";
		public static string ThrustGetMaxPowerConsumptionMethod = "get_MaxPowerConsumption";
		public static string ThrustGetMinPowerConsumptionMethod = "get_MinPowerConsumption";

		public static string ThrustNetManagerField = "=gNAvpmtOwXYmmcG9ynRDGRitUw=";

		//Note: The following fields exist but are not broadcast and as such setting these on the server will do nothing client-side
		public static string ThrustFlameColorField = "=6OD8GKOc8H2eZeBzbaPpDc8wHza=";

		public static string ThrustLightField = "=SvdAPseZczZcMSnX1q9J0bFGki=";
		public static string ThrustFlameScaleCoefficientField = "=Ey8BUGNI8orxc8EnSAO8vkFNiB=";

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
					Action action = InternalUpdateOverride;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
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
				result &= HasMethod( type, ThrustGetOverrideMethod );
				result &= HasMethod( type, ThrustSetOverrideMethod );
				result &= HasMethod( type, ThrustGetMaxThrustVectorMethod );
				result &= HasMethod( type, ThrustGetMaxPowerConsumptionMethod );
				result &= HasMethod( type, ThrustGetMinPowerConsumptionMethod );
				result &= HasField( type, ThrustNetManagerField );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
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
				LogManager.ErrorLog.WriteLine( ex );
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

		public static string ThrustNetManagerNamespace = "";
		public static string ThrustNetManagerClass = "=JekhTXKmjFBlkT4vf3FViyIGcg=";

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
				result &= BaseObject.HasMethod( type, ThrustNetManagerBroadcastOverrideMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void BroadcastOverride( float overrideValue )
		{
			if ( m_backingObject == null )
				return;

			m_lastOverride = overrideValue;

			Action action = InternalBroadcastOverride;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		protected void InternalBroadcastOverride( )
		{
			BaseObject.InvokeEntityMethod( m_backingObject, ThrustNetManagerBroadcastOverrideMethod, new object[ ] { m_lastOverride } );
		}

		#endregion "Methods"
	}
}