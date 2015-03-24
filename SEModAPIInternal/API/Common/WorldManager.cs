namespace SEModAPIInternal.API.Common
{
	using System;
	using System.Threading;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;
	using VRage;

	public class WorldManager
	{
		#region "Attributes"

		private static WorldManager m_instance;
		private bool m_isSaving = false;

		public static string WorldManagerNamespace = "";
		public static string WorldManagerClass = "=GND6z8idGdIAcWcb2YBaSnRMp7=";

		public static string WorldManagerGetPlayerManagerMethod = "get_SyncLayer";
		public static string WorldManagerSaveWorldMethod = "Save";
		public static string WorldManagerGetCheckpointMethod = "GetCheckpoint";
		public static string WorldManagerGetSectorMethod = "GetSector";
		public static string WorldManagerGetSessionNameMethod = "get_Name";

		public static string WorldManagerInstanceField = "=iHp1wRxASKwluTuE6ZRcBr1tEt=";
		public static string WorldManagerFactionManagerField = "=CQQ5bEg8wsyXQIIb8tgJVAWlvQ=";
		public static string WorldManagerSessionSettingsField = "=h2we4UBWghQd1vXkt52UMKFEm0=";

		public static string WorldManagerSaveSnapshot = "Save";

		public static string WorldSnapshotNamespace = "";
		public static string WorldSnapshotStaticClass = "=BoBxxlO6NNIfr918q5l0NF4t5k=";
		public static string WorldSnapshotSaveMethod = "Start";

		////////////////////////////////////////////////////////////////////

		public static string WorldResourceManagerNamespace = "";
		public static string WorldResourceManagerClass = "=eVTMVlP4PfCfeF0vXNraUAYQr7=";

		public static string WorldResourceManagerResourceLockField = "=Xl7JjDRg5FKWoieR6JiWECpB59=";

		///////////////////////////////////////////////////////////////////

		public static string SandboxGameNamespace = "";
		public static string SandboxGameGameStatsClass = "=UnZ3XvpasoiU22GhaSLN7Oooqt=";
		public static string SandboxGameGetGameStatsInstance = "get_Static";
		public static string SandboxGameGetUpdatesPerSecondField = "=FcpSLAzswrKm3Y1htxRDcGezBo=";

		//////////////////////////////////////////////////////////////////

		public static string RespawnManager = "Sandbox.Game.World.MyRespawnComponent";
		public static string RespawnManagerDictionary = "m_globalRespawnTimesMs";
		public static string RespawnManagerList = "m_tmpRespawnTimes";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected WorldManager( )
		{
			m_instance = this;

			ApplicationLog.BaseLog.Info(  "Finished loading WorldManager" );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static WorldManager Instance
		{
			get
			{
				if ( m_instance == null )
					m_instance = new WorldManager( );

				return m_instance;
			}
		}

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( WorldManagerNamespace, WorldManagerClass );
				return type;
			}
		}

		public Object BackingObject
		{
			get
			{
				try
				{
					Object worldManager = BaseObject.GetStaticFieldValue( InternalType, WorldManagerInstanceField );

					return worldManager;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return null;
				}
			}
		}

		public string Name
		{
			get
			{
				string name = (string)BaseObject.InvokeEntityMethod( BackingObject, WorldManagerGetSessionNameMethod );

				return name;
			}
		}

		public bool IsWorldSaving
		{
			get
			{
				return m_isSaving;
			}
		}

		public MyObjectBuilder_SessionSettings SessionSettings
		{
			get
			{
				try
				{
					MyObjectBuilder_SessionSettings sessionSettings = (MyObjectBuilder_SessionSettings)BaseObject.GetEntityFieldValue( BackingObject, WorldManagerSessionSettingsField );

					return sessionSettings;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return new MyObjectBuilder_SessionSettings( );
				}
			}
		}

		public MyObjectBuilder_Checkpoint Checkpoint
		{
			get
			{
				MyObjectBuilder_Checkpoint checkpoint = (MyObjectBuilder_Checkpoint)BaseObject.InvokeEntityMethod( BackingObject, WorldManagerGetCheckpointMethod, new object[ ] { Name } );

				return checkpoint;
			}
		}

		public MyObjectBuilder_Sector Sector
		{
			get
			{
				MyObjectBuilder_Sector sector = (MyObjectBuilder_Sector)BaseObject.InvokeEntityMethod( BackingObject, WorldManagerGetSectorMethod );

				return sector;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( WorldManagerNamespace, WorldManagerClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for WorldManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, WorldManagerGetPlayerManagerMethod );
				Type[ ] argTypes = new Type[ 1 ];
				argTypes[ 0 ] = typeof( string );
				result &= BaseObject.HasMethod( type1, WorldManagerSaveWorldMethod, argTypes );
				result &= BaseObject.HasMethod( type1, WorldManagerGetCheckpointMethod );
				result &= BaseObject.HasMethod( type1, WorldManagerGetSectorMethod );
				result &= BaseObject.HasMethod( type1, WorldManagerGetSessionNameMethod );
				result &= BaseObject.HasField( type1, WorldManagerInstanceField );
				result &= BaseObject.HasField( type1, WorldManagerFactionManagerField );
				result &= BaseObject.HasField( type1, WorldManagerSessionSettingsField );

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( WorldResourceManagerNamespace, WorldResourceManagerClass );
				if ( type2 == null )
					throw new Exception( "Could not find world resource manager type for WorldManager" );
				result &= BaseObject.HasField( type2, WorldResourceManagerResourceLockField );

				Type type3 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( WorldSnapshotNamespace, WorldSnapshotStaticClass );
				if ( type3 == null )
					throw new Exception( "Could not find world snapshot type for WorldManager" );
				result &= BaseObject.HasMethod( type3, WorldSnapshotSaveMethod );

				Type type4 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( SandboxGameNamespace, SandboxGameGameStatsClass );
				if ( type4 == null )
					throw new Exception( "Count not find type for SandboxGameStats" );

				result &= BaseObject.HasMethod( type4, SandboxGameGetGameStatsInstance );
				result &= BaseObject.HasField( type4, SandboxGameGetUpdatesPerSecondField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error(  ex );
				return false;
			}
		}

		/*
		public static string SandboxGameNamespace = "";
		public static string SandboxGameGameStatsClass = "=UnZ3XvpasoiU22GhaSLN7Oooqt=";
		public static string SandboxGameGetGameStatsInstance = "get_Static";
		public static string SandboxGameGetUpdatesPerSecondField = "=FcpSLAzswrKm3Y1htxRDcGezBo=";
		 */

		public static long GetUpdatesPerSecond( )
		{
			long result = 0L;

			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( SandboxGameNamespace, SandboxGameGameStatsClass );

				object gameStatsObject = BaseObject.InvokeStaticMethod( type, SandboxGameGetGameStatsInstance );
				result = (long)BaseObject.GetEntityFieldValue( gameStatsObject, SandboxGameGetUpdatesPerSecondField );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			return result;
		}

		public void SaveWorld( )
		{
			if ( m_isSaving )
				return;

			m_isSaving = true;
			Action action = InternalSaveWorld;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void AsynchronousSaveWorld( )
		{
			if ( m_isSaving )
				return;

			m_isSaving = true;

			try
			{
				DateTime saveStartTime = DateTime.Now;

				ThreadPool.QueueUserWorkItem( new WaitCallback( ( object state ) =>
					{
						SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
							{
								Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( WorldSnapshotNamespace, WorldSnapshotStaticClass );
								BaseObject.InvokeStaticMethod( type, WorldSnapshotSaveMethod, new object[ ] { new Action(() =>
									{
										ApplicationLog.BaseLog.Info("Asynchronous Save Setup Started: {0}ms", (DateTime.Now - saveStartTime).TotalMilliseconds);
									})
								} );
							} );

						// Ugly -- Get rid of this?
						DateTime start = DateTime.Now;
						FastResourceLock saveLock = InternalGetResourceLock( );
						while ( !saveLock.Owned )
						{
							if ( DateTime.Now - start > TimeSpan.FromMilliseconds( 20000 ) )
								return;

							Thread.Sleep( 1 );
						}

						while ( saveLock.Owned )
						{
							if ( DateTime.Now - start > TimeSpan.FromMilliseconds( 60000 ) )
								return;

							Thread.Sleep( 1 );
						}

						ApplicationLog.BaseLog.Info( "Asynchronous Save Completed: {0}ms", ( DateTime.Now - saveStartTime ).TotalMilliseconds );
						EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
						newEvent.type = EntityEventManager.EntityEventType.OnSectorSaved;
						newEvent.timestamp = DateTime.Now;
						newEvent.entity = null;
						newEvent.priority = 0;
						EntityEventManager.Instance.AddEvent( newEvent );
					} ) );
			}
			catch ( Exception ex )
			{
			}
			finally
			{
				m_isSaving = false;
			}

			/*
			try
			{
				DateTime saveStartTime = DateTime.Now;

				// It looks like keen as an overloaded save function that returns the WorldResourceManager after setting up a save, and then
				// allows you to write to disk from a separate thread?  Why aren't they using this on normal saves?!
				bool result = false;
				String arg0 = null;
				Object[] parameters =
				{
					null,
					arg0,
				};

				Type[] paramTypes =
				{
					SandboxGameAssemblyWrapper.Instance.GetAssemblyType(WorldResourceManagerNamespace, WorldResourceManagerClass).MakeByRefType(),
					typeof(string),
				};

				// Run overloaded save function with extra an out parameter that is set to a WorldResourceManagerClass
				SandboxGameAssemblyWrapper.Instance.GameAction(() =>
				{
					result = (bool)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerSaveWorldMethod, parameters, paramTypes);
				});

			 *
				// Write to disk on a different thread using the WorldResourceManagerClass in the parameter
				ThreadPool.QueueUserWorkItem(new WaitCallback((object state) =>
				{
					if (result)
					{
						ApplicationLog.BaseLog.Info((string.Format("Asynchronous Save Setup Time: {0}ms", (DateTime.Now - saveStartTime).TotalMilliseconds));
						saveStartTime = DateTime.Now;
						result = (bool)BaseObject.InvokeEntityMethod(parameters[0], WorldManagerSaveSnapshot);
					}
					else
					{
						ApplicationLog.BaseLog.Error("Failed to save world (1)");
						return;
					}

					if (result)
					{
						ApplicationLog.BaseLog.Info((string.Format("Asynchronous Save Successful: {0}ms", (DateTime.Now - saveStartTime).TotalMilliseconds));
					}
					else
					{
						ApplicationLog.BaseLog.Error("Failed to save world (2)");
						return;
					}

					EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
					newEvent.type = EntityEventManager.EntityEventType.OnSectorSaved;
					newEvent.timestamp = DateTime.Now;
					newEvent.entity = null;
					newEvent.priority = 0;
					EntityEventManager.Instance.AddEvent(newEvent);
				}));
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
			}
			finally
			{
				m_isSaving = false;
			}
			 */
		}

		public void ClearSpawnTimers()
		{
			Type respawnManagerClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(WorldManagerNamespace, RespawnManager);
			
			object respawnDictionary = BaseObject.GetStaticFieldValue(respawnManagerClassType, RespawnManagerDictionary);
			respawnDictionary.GetType().GetMethod("Clear").Invoke(respawnDictionary, new object[] { });

			object respawnList = BaseObject.GetStaticFieldValue(respawnManagerClassType, RespawnManagerList);
			respawnList.GetType().GetMethod("Clear").Invoke(respawnList, new object[] { });
		}

		// Internals //

		internal Object InternalGetFactionManager( )
		{
			try
			{
				Object worldManager = BaseObject.GetEntityFieldValue( BackingObject, WorldManagerFactionManagerField );

				return worldManager;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		internal Object InternalGetPlayerManager( )
		{
			Object playerManager = BaseObject.InvokeEntityMethod( BackingObject, WorldManagerGetPlayerManagerMethod );

			return playerManager;
		}

		internal FastResourceLock InternalGetResourceLock( )
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( WorldResourceManagerNamespace, WorldResourceManagerClass );
				FastResourceLock result = (FastResourceLock)BaseObject.GetStaticFieldValue( type, WorldResourceManagerResourceLockField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		internal void InternalSaveWorld( )
		{
			try
			{
				DateTime saveStartTime = DateTime.Now;

				Type type = BackingObject.GetType( );
				Type[ ] argTypes = new Type[ 1 ];
				argTypes[ 0 ] = typeof( string );
				bool result = (bool)BaseObject.InvokeEntityMethod( BackingObject, WorldManagerSaveWorldMethod, new object[ ] { null }, argTypes );

				if ( result )
				{
					TimeSpan timeToSave = DateTime.Now - saveStartTime;
					ApplicationLog.BaseLog.Info( "Save complete and took {0} seconds", timeToSave.TotalSeconds );
					m_isSaving = false;

					EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
					newEvent.type = EntityEventManager.EntityEventType.OnSectorSaved;
					newEvent.timestamp = DateTime.Now;
					newEvent.entity = null;
					newEvent.priority = 0;
					EntityEventManager.Instance.AddEvent( newEvent );
				}
				else
				{
					ApplicationLog.BaseLog.Error( "Save failed!" );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
			finally
			{
				m_isSaving = false;
			}
		}

		#endregion "Methods"
	}
}