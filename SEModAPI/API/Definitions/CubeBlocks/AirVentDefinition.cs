using VRage.Game;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	using System.ComponentModel;
	using global::Sandbox.Common.ObjectBuilders.Definitions;

	class AirVentDefinition : BlockDefinition
	{
		public AirVentDefinition( MyObjectBuilder_CubeBlockDefinition definition ) : base( definition )
		{
		}

		/// <summary>
		/// The current ventilation capacity per second.
		/// </summary>
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The current ventilation capacity per second." )]
		public float VentilationCapacityPerSecond
		{
			get { return GetSubTypeDefinition( ).VentilationCapacityPerSecond; }
		}

		/// <summary>
		/// The operational power consumption.
		/// </summary>
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The operational power consumption." )]
		public float OperationalPowerConsumption
		{
			get { return GetSubTypeDefinition( ).OperationalPowerConsumption; }
		}

		/// <summary>
		/// The standby power consumption.
		/// </summary>
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The standby power consumption." )]
		public float StandbyPowerConsumption
		{
			get { return GetSubTypeDefinition( ).StandbyPowerConsumption; }
		}

		public new MyObjectBuilder_AirVentDefinition GetSubTypeDefinition( )
		{
			return (MyObjectBuilder_AirVentDefinition) BaseDefinition;
		}
	}
}
