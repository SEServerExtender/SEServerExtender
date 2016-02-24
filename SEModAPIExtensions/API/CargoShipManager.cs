using VRage.Game;

namespace SEModAPIExtensions.API
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Definitions;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;
	using VRage;
	using VRageMath;

	public class CargoShipManager
	{
		private static CargoShipManager _instance;

		protected CargoShipManager( )
		{
			_instance = this;

			ApplicationLog.BaseLog.Info( "Finished loading CargoShipManager" );
		}

		public static CargoShipManager Instance
		{
			get { return _instance ?? ( _instance = new CargoShipManager( ) ); }
		}

		public void SpawnCargoShipGroup( ulong remoteUserId = 0 )
		{
			SpawnCargoShipGroup( true, remoteUserId );
		}

		public void SpawnCargoShipGroup( bool spawnAtAsteroids = true, ulong remoteUserId = 0 )
		{
			double worldSize = MySandboxGame.ConfigDedicated.SessionSettings.WorldSizeKm * 1000d;
			double spawnSize = 0.25d * worldSize;
			double destinationSize = 0.02d * spawnSize;

			if ( spawnAtAsteroids )
			{
				double farthestAsteroidDistance = 0;
				double nearestAsteroidDistance = double.MaxValue;
				/*foreach ( VoxelMap voxelMap in SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>( ) )
				{
					Vector3D asteroidPositon = voxelMap.Position;
					if ( asteroidPositon.Length( ) > farthestAsteroidDistance )
						farthestAsteroidDistance = asteroidPositon.Length( );
					if ( asteroidPositon.Length( ) < nearestAsteroidDistance )
						nearestAsteroidDistance = asteroidPositon.Length( );
				}*/

				spawnSize = farthestAsteroidDistance * 2d + 10000d;
				destinationSize = nearestAsteroidDistance * 2d + 2000d;
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
				double lowestFrequency = double.MaxValue;
				double highestFrequency = 0d;
				foreach ( MySpawnGroupDefinition entry in MyDefinitionManager.Static.GetSpawnGroupDefinitions( ) )
				{
					if ( entry.Frequency < lowestFrequency )
						lowestFrequency = entry.Frequency;
					if ( entry.Frequency > highestFrequency )
						highestFrequency = entry.Frequency;
				}
				if ( lowestFrequency <= 0d )
					lowestFrequency = 1d;

				//Get a list of which groups *could* spawn
				Random random = new Random( );
				double randomChance = random.NextDouble( );
				randomChance = randomChance * ( highestFrequency / lowestFrequency );
				List<MySpawnGroupDefinition> possibleGroups = MyDefinitionManager.Static.GetSpawnGroupDefinitions( ).Where( entry => entry.Frequency >= randomChance ).ToList( );

				//Determine which group *will* spawn
				randomChance = random.NextDouble( );
				int randomShipIndex = Math.Max( 0, Math.Min( (int) Math.Round( randomChance * possibleGroups.Count, 0 ), possibleGroups.Count - 1 ) );
				MySpawnGroupDefinition randomSpawnGroup = possibleGroups[ randomShipIndex ];

				ChatManager.Instance.SendPrivateChatMessage( remoteUserId, string.Format( "Spawning cargo group '{0}' ...", randomSpawnGroup.DisplayNameText ) );

				//Spawn the ships in the group
				Matrix orientation = Matrix.CreateLookAt( startPosition, stopPosition, new Vector3( 0, 1, 0 ) );
				foreach ( MySpawnGroupDefinition.SpawnGroupPrefab entry in randomSpawnGroup.Prefabs )
				{
					MyPrefabDefinition matchedPrefab =
						MyDefinitionManager.Static.GetPrefabDefinitions( ).Select( prefabEntry => prefabEntry.Value ).FirstOrDefault( prefabDefinition => prefabDefinition.Id.SubtypeId.ToString( ) == entry.SubtypeId );
					if ( matchedPrefab == null )
						continue;

					foreach ( MyObjectBuilder_CubeGrid objectBuilder in matchedPrefab.CubeGrids )
					{

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

						//cubeGrid.IsDampenersEnabled = false;

						foreach ( MyObjectBuilder_CubeBlock cubeBlock in cubeGrid.BaseCubeBlocks )
						{
							//Set the beacon names
							MyObjectBuilder_Beacon beacon = cubeBlock as MyObjectBuilder_Beacon;
							if ( beacon != null )
							{
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
				}

				ChatManager.Instance.SendPrivateChatMessage( remoteUserId,
				                                             string.Format( "Cargo group '{0}' spawned with {1} ships at {2}", randomSpawnGroup.DisplayNameText, randomSpawnGroup.Prefabs.Count, startPosition ) );
			}
			catch ( ArgumentOutOfRangeException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
			catch ( ArgumentNullException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
			catch ( FormatException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}
	}
}
