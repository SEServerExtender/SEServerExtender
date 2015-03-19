using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIExtensions.API
{
	public class CargoShipManager
	{
		private static CargoShipManager m_instance;

		protected CargoShipManager( )
		{
			m_instance = this;

			Console.WriteLine( "Finished loading CargoShipManager" );
		}

		public static CargoShipManager Instance
		{
			get { return m_instance ?? ( m_instance = new CargoShipManager( ) ); }
		}

		public void SpawnCargoShipGroup( ulong remoteUserId = 0 )
		{
			SpawnCargoShipGroup( true, remoteUserId );
		}

		public void SpawnCargoShipGroup( bool spawnAtAsteroids = true, ulong remoteUserId = 0 )
		{
			double worldSize = SandboxGameAssemblyWrapper.Instance.GetServerConfig( ).SessionSettings.WorldSizeKm * 1000.0f;
			double spawnSize = 0.25f * worldSize;
			double destinationSize = 0.02f * spawnSize;

			if ( spawnAtAsteroids )
			{
				double farthestAsteroidDistance = 0;
				double nearestAsteroidDistance = 999999;
				foreach ( VoxelMap voxelMap in SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>( ) )
				{
					Vector3D asteroidPositon = voxelMap.Position;
					if ( asteroidPositon.Length( ) > farthestAsteroidDistance )
						farthestAsteroidDistance = asteroidPositon.Length( );
					if ( asteroidPositon.Length( ) < nearestAsteroidDistance )
						nearestAsteroidDistance = asteroidPositon.Length( );
				}

				spawnSize = farthestAsteroidDistance * 2 + 10000;
				destinationSize = nearestAsteroidDistance * 2 + 2000;
			}

			Vector3D groupPosition = UtilityFunctions.GenerateRandomBorderPosition( new Vector3D( -spawnSize, -spawnSize, -spawnSize ), new Vector3D( spawnSize, spawnSize, spawnSize ) );
			Vector3D destinationPosition = UtilityFunctions.GenerateRandomBorderPosition( new Vector3D( -destinationSize, -destinationSize, -destinationSize ), new Vector3D( destinationSize, destinationSize, destinationSize ) );

			SpawnCargoShipGroup( groupPosition, destinationPosition, remoteUserId );
		}

		public void SpawnCargoShipGroup( Vector3 startPosition, Vector3 stopPosition, ulong remoteUserId = 0 )
		{
			try
			{
				//Calculate lowest and highest frequencies
				float lowestFrequency = 999999;
				float highestFrequency = 0;
				foreach ( MySpawnGroupDefinition entry in MyDefinitionManager.Static.GetSpawnGroupDefinitions( ) )
				{
					if ( entry.Frequency < lowestFrequency )
						lowestFrequency = entry.Frequency;
					if ( entry.Frequency > highestFrequency )
						highestFrequency = entry.Frequency;
				}
				if ( lowestFrequency <= 0 )
					lowestFrequency = 1;

				//Get a list of which groups *could* spawn
				Random random = new Random( (int)DateTime.Now.ToBinary( ) );
				double randomChance = random.NextDouble( );
				randomChance = randomChance * ( highestFrequency / lowestFrequency );
				List<MySpawnGroupDefinition> possibleGroups = new List<MySpawnGroupDefinition>( );
				foreach ( MySpawnGroupDefinition entry in MyDefinitionManager.Static.GetSpawnGroupDefinitions( ) )
				{
					if ( entry.Frequency >= randomChance )
					{
						possibleGroups.Add( entry );
					}
				}

				//Determine which group *will* spawn
				randomChance = random.NextDouble( );
				int randomShipIndex = Math.Max( 0, Math.Min( (int)Math.Round( randomChance * possibleGroups.Count, 0 ), possibleGroups.Count - 1 ) );
				MySpawnGroupDefinition randomSpawnGroup = possibleGroups[ randomShipIndex ];

				ChatManager.Instance.SendPrivateChatMessage( remoteUserId, string.Format( "Spawning cargo group '{0}' ...", randomSpawnGroup.DisplayNameText ) );

				//Spawn the ships in the group
				Matrix orientation = Matrix.CreateLookAt( startPosition, stopPosition, new Vector3( 0, 1, 0 ) );
				foreach ( MySpawnGroupDefinition.SpawnGroupPrefab entry in randomSpawnGroup.Prefabs )
				{
					MyPrefabDefinition matchedPrefab = null;
					foreach ( var prefabEntry in MyDefinitionManager.Static.GetPrefabDefinitions( ) )
					{
						MyPrefabDefinition prefabDefinition = prefabEntry.Value;
						if ( prefabDefinition.Id.SubtypeId.ToString( ) == entry.SubtypeId )
						{
							matchedPrefab = prefabDefinition;
							break;
						}
					}
					if ( matchedPrefab == null )
						continue;

					//TODO - Build this to iterate through all cube grids in the prefab
					MyObjectBuilder_CubeGrid objectBuilder = matchedPrefab.CubeGrids[ 0 ];

					//Create the ship
					CubeGridEntity cubeGrid = new CubeGridEntity( objectBuilder );

					//Set the ship position and orientation
					Vector3 shipPosition = Vector3.Transform( entry.Position, orientation ) + startPosition;
					orientation.Translation = shipPosition;
					MyPositionAndOrientation newPositionOrientation = new MyPositionAndOrientation( orientation );
					cubeGrid.PositionAndOrientation = newPositionOrientation;

					//Set the ship velocity
					//Speed is clamped between 1.0f and the max cube grid speed
					Vector3 travelVector = stopPosition - startPosition;
					travelVector.Normalize( );
					Vector3 shipVelocity = travelVector * Math.Min( cubeGrid.MaxLinearVelocity, Math.Max( 1.0f, entry.Speed ) );
					cubeGrid.LinearVelocity = shipVelocity;

					cubeGrid.IsDampenersEnabled = false;

					foreach ( MyObjectBuilder_CubeBlock cubeBlock in cubeGrid.BaseCubeBlocks )
					{
						//Set the beacon names
						if ( cubeBlock.TypeId == typeof( MyObjectBuilder_Beacon ) )
						{
							MyObjectBuilder_Beacon beacon = (MyObjectBuilder_Beacon)cubeBlock;
							beacon.CustomName = entry.BeaconText;
						}

						//Set the owner of every block
						//TODO - Find out if setting to an arbitrary non-zero works for this
						//cubeBlock.Owner = PlayerMap.Instance.GetServerVirtualPlayerId();
						cubeBlock.Owner = 0;
						cubeBlock.ShareMode = MyOwnershipShareModeEnum.Faction;
					}

					//And add the ship to the world
					SectorObjectManager.Instance.AddEntity( cubeGrid );
				}

				ChatManager.Instance.SendPrivateChatMessage( remoteUserId, string.Format( "Cargo group '{0}' spawned with {1} ships at {2}", randomSpawnGroup.DisplayNameText, randomSpawnGroup.Prefabs.Count, startPosition ) );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}
	}
}
