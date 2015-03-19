using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "SolarPanelEntityProxy" )]
	public class SolarPanelEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private PowerProducer m_powerProducer;
		private float m_maxPowerOutput;

		public static string SolarPanelNamespace = "";
		public static string SolarPanelClass = "=eMM44GvCk02ICnSbvGDga6Az5t=";

		public static string SolarPanelSetMaxOutputMethod = "set_MaxPowerOutput";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public SolarPanelEntity( CubeGridEntity parent, MyObjectBuilder_SolarPanel definition )
			: base( parent, definition )
		{
		}

		public SolarPanelEntity( CubeGridEntity parent, MyObjectBuilder_SolarPanel definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_powerProducer = new PowerProducer( Parent.PowerManager, ActualObject );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Solar Panel" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_SolarPanel ObjectBuilder
		{
			get { return (MyObjectBuilder_SolarPanel)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Solar Panel" )]
		public float MaxPower
		{
			get { return PowerProducer.MaxPowerOutput; }
			set
			{
				m_maxPowerOutput = value;

				Action action = InternalUpdateMaxPowerOutput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
			}
		}

		[DataMember]
		[Category( "Solar Panel" )]
		public float Power
		{
			get { return PowerProducer.PowerOutput; }
			set { PowerProducer.PowerOutput = value; }
		}

		[IgnoreDataMember]
		[Category( "Solar Panel" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal PowerProducer PowerProducer
		{
			get { return m_powerProducer; }
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( SolarPanelNamespace, SolarPanelClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for SolarPanelEntity" );
				result &= HasMethod( type, SolarPanelSetMaxOutputMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		protected void InternalUpdateMaxPowerOutput( )
		{
			InvokeEntityMethod( ActualObject, SolarPanelSetMaxOutputMethod, new object[ ] { m_maxPowerOutput } );
		}

		#endregion "Methods"
	}
}