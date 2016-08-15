namespace SEModAPIExtensions.API
{
	using System;
	using NLog;
	using NLog.Targets;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
	using SEModAPIInternal.API.Server;
	using SEModAPIInternal.API.Utility;

	public class BasicUnitTestManager
	{
		private static BasicUnitTestManager _instance;
		public static readonly Logger BaseLog = LogManager.GetLogger( "BaseLog" );

		protected BasicUnitTestManager()
		{
			FileTarget baseLogTarget = LogManager.Configuration.FindTargetByName( "BaseLog" ) as FileTarget;
			if ( baseLogTarget != null )
			{
				baseLogTarget.FileName = baseLogTarget.FileName.Render( new LogEventInfo { TimeStamp = DateTime.Now } );
			}
			_instance = this;
		}

		public static BasicUnitTestManager Instance
		{
			get { return _instance ?? ( _instance = new BasicUnitTestManager( ) ); }
		}

		public bool Run()
		{
			bool result = true;
			result &= RunBaseReflectionUnitTests();
			//result &= RunEntityReflectionUnitTests();
			//result &= RunCubeBlockReflectionTests();

			return result;
		}

		protected bool RunBaseReflectionUnitTests()
		{
			bool result = true;

			if (!DedicatedServerAssemblyWrapper.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "DedicatedServerAssemblyWrapper reflection validation failed!" );
			}

			if (!ServerNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "ServerNetworkManager reflection validation failed!" );
			}

			if (!UtilityFunctions.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "UtilityFunctions reflection validation failed!" );
			}

			if (!ChatManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "ChatManager reflection validation failed!" );
			}

			if (!PlayerMap.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "PlayerMap reflection validation failed!" );
			}

			if (!PlayerManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "PlayerManager reflection validation failed!" );
			}

			if (!WorldManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "WorldManager reflection validation failed!" );
			}

            /*
			if (!RadioManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "RadioManager reflection validation failed!" );
			}

			if (!RadioManagerNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "RadioManagerNetworkManager reflection validation failed!" );
			}

            if (!FactionsManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "FactionsManager reflection validation failed!" );
			}

			if (!Faction.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "Faction reflection validation failed!" );
			}
            */

			if (!GameEntityManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn( "GameEntityManager reflection validation failed!" );
			}

			if (result)
			{
				BaseLog.Info( "All main types passed reflection unit tests!" );
			}

			return result;
		}

		protected bool RunEntityReflectionUnitTests()
		{
			bool result = true;

			if (!BaseObject.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("BaseObject reflection validation failed!");
			}

			if (!BaseEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("BaseEntity reflection validation failed!");
			}

			if (!BaseEntityNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("BaseEntityNetworkManager reflection validation failed!");
			}

			if (!CubeGridEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CubeGridEntity reflection validation failed!");
			}

			//if (!CubeGridManagerManager.ReflectionUnitTest())
			//{
			//	result = false;
			//	BaseLog.Warn("CubeGridManagerManager reflection validation failed!");
			//}

			if (!CubeGridNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CubeGridNetworkManager reflection validation failed!");
			}

			if (!CubeGridThrusterManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CubeGridThrusterManager reflection validation failed!");
			}

			if (!SectorObjectManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("SectorObjectManager reflection validation failed!");
			}

			if (!CharacterEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CharacterEntity reflection validation failed!");
			}

			if (!CharacterEntityNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CharacterEntityNetworkManager reflection validation failed!");
			}

			if (!FloatingObject.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("FloatingObject reflection validation failed!");
			}

			if (!FloatingObjectManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("FloatingObjectManager reflection validation failed!");
			}

			if (!InventoryEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("InventoryEntity reflection validation failed!");
			}

			if (!InventoryItemEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("InventoryItemEntity reflection validation failed!");
			}
/*
			if (!VoxelMap.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("VoxelMap reflection validation failed!");
			}

			if (!VoxelMapMaterialManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("VoxelMapMaterialManager reflection validation failed!");
			}
            */
			if (result)
			{
				BaseLog.Info( "All entity types passed reflection unit tests!" );
			}

			return result;
		}

		protected bool RunCubeBlockReflectionTests()
		{
			bool result = true;

			if (!CubeBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CubeBlockEntity reflection validation failed!");
			}

			if (!TerminalBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("TerminalBlockEntity reflection validation failed!");
			}

			if (!FunctionalBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("FunctionalBlockEntity reflection validation failed!");
			}

			if (!ProductionBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ProductionBlockEntity reflection validation failed!");
			}

			if (!LightEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("LightEntity reflection validation failed!");
			}

			if (!BatteryBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("BatteryBlockEntity reflection validation failed!");
			}

			if (!BatteryBlockNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("BatteryBlockNetworkManager reflection validation failed!");
			}

			if (!DoorEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("DoorEntity reflection validation failed!");
			}

			if (!GravityBaseEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("GravityBaseEntity reflection validation failed!");
			}

			if (!GravityGeneratorEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("GravityGeneratorEntity reflection validation failed!");
			}

			if (!GravitySphereEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("GravitySphereEntity reflection validation failed!");
			}

			if (!BeaconEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("BeaconEntity reflection validation failed!");
			}

			if (!AntennaEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("AntennaEntity reflection validation failed!");
			}

			if (!ThrustEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ThrustEntity reflection validation failed!");
			}

			if (!ThrustNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ThrustNetworkManager reflection validation failed!");
			}

			if (!GyroEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("GyroEntity reflection validation failed!");
			}

			if (!GyroNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("GyroNetworkManager reflection validation failed!");
			}

			if (!CockpitEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CockpitEntity reflection validation failed!");
			}

			if (!TurretBaseEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("TurretBaseEntity reflection validation failed!");
			}

			if (!TurretNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("TurretNetworkManager reflection validation failed!");
			}

			if (!LandingGearEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("LandingGearEntity reflection validation failed!");
			}

			if (!LandingGearNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("LandingGearNetworkManager reflection validation failed!");
			}

			if (!ReactorEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ReactorEntity reflection validation failed!");
			}

			if (!SolarPanelEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("SolarPanelEntity reflection validation failed!");
			}

			if (!SmallGatlingGunEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("SmallGatlingGunEntity reflection validation failed!");
			}

			if (!MergeBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("MergeBlockEntity reflection validation failed!");
			}

			if (!PistonEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("PistonEntity reflection validation failed!");
			}

			if (!PistonNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("PistonNetworkManager reflection validation failed!");
			}

			if (!RotorEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("RotorEntity reflection validation failed!");
			}

			if (!VirtualMassEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("VirtualMassEntity reflection validation failed!");
			}

			if (!CameraBlockEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("CameraBlockEntity reflection validation failed!");
			}

			if (!OreDetectorEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("OreDetectorEntity reflection validation failed!");
			}

			if (!ButtonPanelEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ButtonPanelEntity reflection validation failed!");
			}

			if (!ShipControllerEntity.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ShipControllerEntity reflection validation failed!");
			}

			if (!ShipControllerNetworkManager.ReflectionUnitTest())
			{
				result = false;
				BaseLog.Warn("ShipControllerNetworkManager reflection validation failed!");
			}

			if (result)
			{
				BaseLog.Info( "All block types passed reflection unit tests!" );
			}

			return result;
		}
	}
}
