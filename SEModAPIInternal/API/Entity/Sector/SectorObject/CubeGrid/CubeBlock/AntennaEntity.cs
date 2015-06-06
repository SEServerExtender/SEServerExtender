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
	public class AntennaEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private RadioManager m_radioManager;

		public static string AntennaNamespace = "Sandbox.Game.Entities.Cube";
		public static string AntennaClass = "MyRadioAntenna";

		public static string AntennaGetRadioManagerMethod = "get_RadioBroadcaster";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public AntennaEntity( CubeGridEntity parent, MyObjectBuilder_RadioAntenna definition )
			: base( parent, definition )
		{
		}

		public AntennaEntity( CubeGridEntity parent, MyObjectBuilder_RadioAntenna definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			Object internalRadioManager = InternalGetRadioManager( );
			m_radioManager = new RadioManager( internalRadioManager );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Antenna" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_RadioAntenna ObjectBuilder
		{
			get
			{
				MyObjectBuilder_RadioAntenna antenna = (MyObjectBuilder_RadioAntenna)base.ObjectBuilder;

				return antenna;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Antenna" )]
		public float BroadcastRadius
		{
			get
			{
				float result = ObjectBuilder.BroadcastRadius;

				if ( m_radioManager != null )
					result = m_radioManager.BroadcastRadius;

				return result;
			}
			set
			{
				if ( ObjectBuilder.BroadcastRadius == value ) return;
				ObjectBuilder.BroadcastRadius = value;
				Changed = true;

				if ( m_radioManager != null )
					m_radioManager.BroadcastRadius = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Antenna" )]
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( AntennaNamespace, AntennaClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for AntennaEntity" );
				result &= Reflection.HasMethod( type, AntennaGetRadioManagerMethod );

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
				Object result = InvokeEntityMethod( ActualObject, AntennaGetRadioManagerMethod );
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