using System.Collections.Generic;
using System.IO;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	/// <summary>
	/// This class is intended to manage the modification and persistente of CubeBlocks.sbc
	/// </summary>
	public class BlocksManager
	{
		#region "Attributes"

		private const string DefaultFileName = "CubeBlocks.sbc";
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
		

		#region "Constructor & Initializers"

		/// <summary>
		/// Default RAII constructor of Manager
		/// </summary>
		/// <param name="cubeBlocksFileInfo">The valid FileInfo that points to a valid CubeBlocks.sbc file</param>
		/// <param name="defaultName">Defines if the file has the defaultName: CubeBlocks.sbc</param>
		public BlocksManager(FileInfo cubeBlocksFileInfo, bool defaultName = true)
		{
			if (defaultName)
			{
				if (cubeBlocksFileInfo.Name != DefaultFileName)
				{
					throw new SEConfigurationException(SEConfigurationExceptionState.InvalidDefaultConfigFileName, "The given file name is not matching the default configuration name pattern: CubeBlocks.sbc");
				}
			}
			_configFileSerializer = new ConfigFileSerializer(cubeBlocksFileInfo, defaultName);
			if (cubeBlocksFileInfo.Exists)
			{
				Deserialize();
			}
		}

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
		/// Get the container for LightingBlocks
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
		/// Get the container for ShipDrills
		/// </summary>
		public List<SolarPanelDefinition> SolarPanels
		{
			get { return _solarPanels; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<ThrusterDefinition> Thrusters
		{
			get { return _thrusters; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<VirtualMassDefinition> VirtualMasses
		{
			get { return _virtualMasses; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method that scan definitions for changes
		/// </summary>
		/// <returns></returns>
		public bool FindChangesInDefinitions()
		{
			foreach (var block in ExtractDefinitionsFromContainers())
			{
				if (block.Changed)
				{
					return true;
				}
			}
			return false;
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
		/// Method to Deserialize the current inner Configuration File
		/// </summary>
		public void Deserialize()
		{
			_definitions = _configFileSerializer.Deserialize();
			FillOverLayerContainers(_definitions.CubeBlocks);
		}

		/// <summary>
		/// Method that fill the containers the underlayed definitions
		/// </summary>
		/// <param name="blocks">If an array is given, the containers will be filled with this array instead of the default underlayed one</param>
		public void FillOverLayerContainers(MyObjectBuilder_CubeBlockDefinition[] blocks)
		{
			_cubeBlocks.Clear();
			_assemblers.Clear();
			_cargoContainers.Clear();
			_cockpits.Clear();
			_gravityGenerators.Clear();
			_gyroscopes.Clear();
			_mergeBlocks.Clear();
			_motorStators.Clear();
			_oreDetectors.Clear();
			_reactors.Clear();
			_refineries.Clear();
			_lightingBlocks.Clear();
			_shipDrills.Clear();
			_solarPanels.Clear();
			_thrusters.Clear();
			_virtualMasses.Clear();
			foreach (var cubeBlock in blocks)
			{/*
				switch (cubeBlock.Id.TypeId)
				{
					case (MyObjectBuilderTypeEnum.CubeBlock):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Assembler):
					{
						_assemblers.Add(new AssemblerDefinition((MyObjectBuilder_AssemblerDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Beacon):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.CargoContainer):
					{
						_cargoContainers.Add(new CargoContainerDefinition((MyObjectBuilder_CargoContainerDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Cockpit):
					{
						_cockpits.Add(new CockpitDefinition((MyObjectBuilder_CockpitDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Collector):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Conveyor):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ConveyorConnector):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Decoy):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Door):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Drill):
					{
						_shipDrills.Add(new ShipDrillDefinition((MyObjectBuilder_ShipDrillDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.GravityGenerator):
					{
						_gravityGenerators.Add(new GravityGeneratorDefinition((MyObjectBuilder_GravityGeneratorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Gyro):
					{
						_gyroscopes.Add(new GyroscopeDefinition((MyObjectBuilder_GyroDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.LandingGear):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.LargeGatlingTurret):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.LightingBlock):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MedicalRoom):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MergeBlock):
					{
						_mergeBlocks.Add(new MergeBlockDefinition((MyObjectBuilder_MergeBlockDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MotorRotor):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MotorStator):
					{
						_motorStators.Add(new MotorStatorDefinition((MyObjectBuilder_MotorStatorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.OreDetector):
					{
						_oreDetectors.Add(new OreDetectorDefinition((MyObjectBuilder_OreDetectorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Passage):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.RadioAntenna):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Reactor):
					{
						_reactors.Add(new ReactorDefinition((MyObjectBuilder_ReactorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.RealWheel):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Refinery):
					{
						_refineries.Add(new RefineryDefinition((MyObjectBuilder_RefineryDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ReflectorLight):
					{
						_lightingBlocks.Add(new LightingBlockDefinition((MyObjectBuilder_LightingBlockDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ShipConnector):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ShipGrinder):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ShipWelder):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.SmallGatlingGun):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.SmallMissileLauncher):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.SolarPanel):
					{
						_solarPanels.Add(new SolarPanelDefinition((MyObjectBuilder_SolarPanelDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Thrust):
					{
						_thrusters.Add(new ThrusterDefinition((MyObjectBuilder_ThrustDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.VirtualMass):
					{
						_virtualMasses.Add(new VirtualMassDefinition((MyObjectBuilder_VirtualMassDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Warhead):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Wheel):
					{
						_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;
				}*/
			}
		}

		/// <summary>
		/// Method that Extract the base cubeblocks definitions from every container.
		/// </summary>
		public List<MyObjectBuilder_CubeBlockDefinition> ExtractBaseDefinitionsFromContainers()
		{
			List<MyObjectBuilder_CubeBlockDefinition> blocks = new List<MyObjectBuilder_CubeBlockDefinition>();

			foreach (var item in _cubeBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _assemblers)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _cargoContainers)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _cockpits)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _gravityGenerators)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _gyroscopes)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _lightingBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _mergeBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _motorStators)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _oreDetectors)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _reactors)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _refineries)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _shipDrills)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _solarPanels)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _thrusters)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in _virtualMasses)
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

			foreach (var item in _cubeBlocks)
			{
				blocks.Add(item);
			}
			foreach (var item in _cargoContainers)
			{
				blocks.Add(item);
			}
			foreach (var item in _cockpits)
			{
				blocks.Add(item);
			}
			foreach (var item in _gravityGenerators)
			{
				blocks.Add(item);
			}
			foreach (var item in _gyroscopes)
			{
				blocks.Add(item);
			}
			foreach (var item in _lightingBlocks)
			{
				blocks.Add(item);
			}
			foreach (var item in _mergeBlocks)
			{
				blocks.Add(item);
			}
			foreach (var item in _motorStators)
			{
				blocks.Add(item);
			}
			foreach (var item in _oreDetectors)
			{
				blocks.Add(item);
			}
			foreach (var item in _reactors)
			{
				blocks.Add(item);
			}
			foreach (var item in _refineries)
			{
				blocks.Add(item);
			}
			foreach (var item in _shipDrills)
			{
				blocks.Add(item);
			}
			foreach (var item in _solarPanels)
			{
				blocks.Add(item);
			}
			foreach (var item in _thrusters)
			{
				blocks.Add(item);
			}
			foreach (var item in _virtualMasses)
			{
				blocks.Add(item);
			}

			return blocks;
		}

		#endregion
	}
}
