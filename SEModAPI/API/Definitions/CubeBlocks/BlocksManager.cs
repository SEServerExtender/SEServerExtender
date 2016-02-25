using VRage.Game;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	using System.Collections.Generic;
	using System.Linq;
	using global::Sandbox.Common.ObjectBuilders.Definitions;

	/// <summary>
	/// This class is intended to manage the modification and persistence of CubeBlocks.sbc
	/// </summary>
	public class BlocksManager
	{
		#region "Attributes"

		private readonly ConfigFileSerializer _configFileSerializer;
		private MyObjectBuilder_Definitions _definitions;


		private readonly List<CubeBlockDef> _cubeBlocks = new List<CubeBlockDef>();
		private readonly List<AssemblerDefinition> _assemblers = new List<AssemblerDefinition>();
		private readonly List<CargoContainerDefinition> _cargoContainers = new List<CargoContainerDefinition>();
		private readonly List<CockpitDefinition> _cockpits = new List<CockpitDefinition>();
		private readonly List<GravityGeneratorDefinition> _gravityGenerators = new List<GravityGeneratorDefinition>();
		private readonly List<GyroscopeDefinition> _gyroscopes = new List<GyroscopeDefinition>();
		private readonly List<MergeBlockDefinition> _mergeBlocks = new List<MergeBlockDefinition>();
		private readonly List<MotorStatorDefinition> _motorStators = new List<MotorStatorDefinition>();
		private readonly List<OreDetectorDefinition> _oreDetectors = new List<OreDetectorDefinition>();
		private readonly List<ReactorDefinition> _reactors = new List<ReactorDefinition>();
		private readonly List<RefineryDefinition> _refineries = new List<RefineryDefinition>();
		private readonly List<LightingBlockDefinition> _lightingBlocks = new List<LightingBlockDefinition>();
		private readonly List<ShipDrillDefinition> _shipDrills = new List<ShipDrillDefinition>();
		private readonly List<SolarPanelDefinition> _solarPanels = new List<SolarPanelDefinition>();
		private readonly List<ThrusterDefinition> _thrusters = new List<ThrusterDefinition>();
		private readonly List<VirtualMassDefinition> _virtualMasses = new List<VirtualMassDefinition>();

		#endregion
		
		#region "Properties"

		/// <summary>
		/// Get the container for Assemblers
		/// </summary>
		public List<AssemblerDefinition> Assemblers
		{
			get { return _assemblers; }
		}

		/// <summary>
		/// Get the container for CargoContainers
		/// </summary>
		public List<CargoContainerDefinition> CargoContainers
		{
			get { return _cargoContainers; }
		}

		/// <summary>
		/// Get the container for CubeBlocks
		/// </summary>
		public List<CubeBlockDef> CubeBlocks
		{
			get { return _cubeBlocks; }
		}

		/// <summary>
		/// Get the container for Cockpits
		/// </summary>
		public List<CockpitDefinition> Cockpits
		{
			get { return _cockpits; }
		}

		/// <summary>
		/// Get the container for GravityGenerators
		/// </summary>
		public List<GravityGeneratorDefinition> GravityGenerators
		{
			get { return _gravityGenerators; }
		}

		/// <summary>
		/// Get the container for Gyroscopes
		/// </summary>
		public List<GyroscopeDefinition> Gyroscopes
		{
			get { return _gyroscopes; }
		}

		/// <summary>
		/// Get the container for LightingBlocks
		/// </summary>
		public List<LightingBlockDefinition> LightingBlocks
		{
			get { return _lightingBlocks; }
		}

		/// <summary>
		/// Get the container for MergeBlocks
		/// </summary>
		public List<MergeBlockDefinition> MergeBlocks
		{
			get { return _mergeBlocks; }
		}

		/// <summary>
		/// Get the container for MotorStators
		/// </summary>
		public List<MotorStatorDefinition> MotorStators
		{
			get { return _motorStators; }
		}

		/// <summary>
		/// Get the container for OreDetectors
		/// </summary>
		public List<OreDetectorDefinition> OreDetectors
		{
			get { return _oreDetectors; }
		}

		/// <summary>
		/// Get the container for Reactors
		/// </summary>
		public List<ReactorDefinition> Reactors
		{
			get { return _reactors; }
		}

		/// <summary>
		/// Get the container for Refineries
		/// </summary>
		public List<RefineryDefinition> Refineries
		{
			get { return _refineries; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<ShipDrillDefinition> ShipDrills
		{
			get { return _shipDrills; }
		}

		/// <summary>
		/// Get the container for SolarPanels
		/// </summary>
		public List<SolarPanelDefinition> SolarPanels
		{
			get { return _solarPanels; }
		}

		/// <summary>
		/// Get the container for Thrusters
		/// </summary>
		public List<ThrusterDefinition> Thrusters
		{
			get { return _thrusters; }
		}

		/// <summary>
		/// Get the container for VirtualMasses
		/// </summary>
		public List<VirtualMassDefinition> VirtualMasses
		{
			get { return _virtualMasses; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method that scans definitions for changes
		/// </summary>
		/// <returns></returns>
		public bool FindChangesInDefinitions()
		{
			return ExtractDefinitionsFromContainers( ).Any( block => block.Changed );
		}

		/// <summary>
		/// Method to Serialize the current inner Configuration File
		/// </summary>
		public void Serialize()
		{
			_definitions.CubeBlocks = ExtractBaseDefinitionsFromContainers().ToArray();
			_configFileSerializer.Serialize(_definitions);
		}

		/// <summary>
		/// Method that Extract the base cubeblocks definitions from every container.
		/// </summary>
		public List<MyObjectBuilder_CubeBlockDefinition> ExtractBaseDefinitionsFromContainers()
		{
			List<MyObjectBuilder_CubeBlockDefinition> blocks = new List<MyObjectBuilder_CubeBlockDefinition>();

			foreach (CubeBlockDef item in _cubeBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (AssemblerDefinition item in _assemblers)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (CargoContainerDefinition item in _cargoContainers)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (CockpitDefinition item in _cockpits)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (GravityGeneratorDefinition item in _gravityGenerators)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (GyroscopeDefinition item in _gyroscopes)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (LightingBlockDefinition item in _lightingBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (MergeBlockDefinition item in _mergeBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (MotorStatorDefinition item in _motorStators)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (OreDetectorDefinition item in _oreDetectors)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (ReactorDefinition item in _reactors)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (RefineryDefinition item in _refineries)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (ShipDrillDefinition item in _shipDrills)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (SolarPanelDefinition item in _solarPanels)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (ThrusterDefinition item in _thrusters)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (VirtualMassDefinition item in _virtualMasses)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}

			return blocks;
		}

		/// <summary>
		/// Method that Extract base blocks definitions from container.
		/// </summary>
		public List<BlockDefinition> ExtractDefinitionsFromContainers()
		{
			List<BlockDefinition> blocks = new List<BlockDefinition>();

			foreach (CubeBlockDef item in _cubeBlocks)
			{
				blocks.Add(item);
			}
			foreach (CargoContainerDefinition item in _cargoContainers)
			{
				blocks.Add(item);
			}
			foreach (CockpitDefinition item in _cockpits)
			{
				blocks.Add(item);
			}
			foreach (GravityGeneratorDefinition item in _gravityGenerators)
			{
				blocks.Add(item);
			}
			foreach (GyroscopeDefinition item in _gyroscopes)
			{
				blocks.Add(item);
			}
			foreach (LightingBlockDefinition item in _lightingBlocks)
			{
				blocks.Add(item);
			}
			foreach (MergeBlockDefinition item in _mergeBlocks)
			{
				blocks.Add(item);
			}
			foreach (MotorStatorDefinition item in _motorStators)
			{
				blocks.Add(item);
			}
			foreach (OreDetectorDefinition item in _oreDetectors)
			{
				blocks.Add(item);
			}
			foreach (ReactorDefinition item in _reactors)
			{
				blocks.Add(item);
			}
			foreach (RefineryDefinition item in _refineries)
			{
				blocks.Add(item);
			}
			foreach (ShipDrillDefinition item in _shipDrills)
			{
				blocks.Add(item);
			}
			foreach (SolarPanelDefinition item in _solarPanels)
			{
				blocks.Add(item);
			}
			foreach (ThrusterDefinition item in _thrusters)
			{
				blocks.Add(item);
			}
			foreach (VirtualMassDefinition item in _virtualMasses)
			{
				blocks.Add(item);
			}

			return blocks;
		}

		#endregion
	}
}
