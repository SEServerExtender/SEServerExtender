namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class BeaconEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private RadioManager m_radioManager;

		public static string BeaconNamespace = "Sandbox.Game.Entities.Cube";
		public static string BeaconClass = "MyBeacon";

		public static string BeaconGetRadioManagerMethod = "get_RadioBroadcaster";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BeaconEntity( CubeGridEntity parent, MyObjectBuilder_Beacon definition )
			: base( parent, definition )
		{
		}

		public BeaconEntity( CubeGridEntity parent, MyObjectBuilder_Beacon definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			Object internalRadioManager = InternalGetRadioManager( );
			m_radioManager = new RadioManager( internalRadioManager );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Beacon" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Beacon ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Beacon beacon = (MyObjectBuilder_Beacon)base.ObjectBuilder;

				return beacon;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Beacon" )]
		[DisplayName("Broadcast Radius")]
		public float BroadcastRadius
		{
			get
			{
				float result = ObjectBuilder.BroadcastRadius;

				if ( RadioManager != null )
					result = RadioManager.BroadcastRadius;

				return result;
			}
			set
			{
				if ( ObjectBuilder.BroadcastRadius == value ) return;
				ObjectBuilder.BroadcastRadius = value;
				Changed = true;

				if ( RadioManager != null )
					RadioManager.BroadcastRadius = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Beacon" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal RadioManager RadioManager
		{
			get { return m_radioManager; }
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( BeaconNamespace, BeaconClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for BeaconEntity" );
				result &= Reflection.HasMethod( type, BeaconGetRadioManagerMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetRadioManager( )
		{
			try
			{
				Object result = InvokeEntityMethod( ActualObject, BeaconGetRadioManagerMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}