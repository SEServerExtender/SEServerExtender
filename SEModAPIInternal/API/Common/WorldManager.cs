using System;
using System.Threading;
using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;
using VRage;

namespace SEModAPIInternal.API.Common
{
	public class WorldManager
	{
		#region "Attributes"

		private static WorldManager m_instance;
		private bool m_isSaving = false;

		public static string WorldManagerNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string WorldManagerClass = "D580AE7552E79DAB03A3D64B1F7B67F9";

		public static string WorldManagerGetPlayerManagerMethod = "4C1B66FF099503DCB589BBFFC4976633";
		public static string WorldManagerSaveWorldMethod = "50092B623574C842837BD09CE21A96D6";
		public static string WorldManagerGetCheckpointMethod = "6CA03E6E730B7881842157B90C864031";
		public static string WorldManagerGetSectorMethod = "B2DFAD1262F75849DA03F64C5E3535B7";
		public static string WorldManagerGetSessionNameMethod = "193678BC97A6081A8AA344BF44620BC5";

		public static string WorldManagerInstanceField = "AE8262481750DAB9C8D416E4DBB9BA04";
		public static string WorldManagerFactionManagerField = "0A481A0F72FB8D956A8E00BB2563E605";
		public static string WorldManagerSessionSettingsField = "3D4D3F0E4E3582FF30FD014D9BB1E504";

		public static string WorldManagerSaveSnapshot = "C0CFAF4B58402DABBB39F4A4694795D0";

		public static string WorldSnapshotNamespace = "6D7C9F7F9CFF9877B430DBAFB54F1802";
		public static string WorldSnapshotStaticClass = "8DEBD6C63930F8C065956AC979F27488";
		public static string WorldSnapshotSaveMethod = "0E05B81B936D03329E9F49031001FE33";

		////////////////////////////////////////////////////////////////////

		public static string WorldResourceManagerNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string WorldResourceManagerClass = "15B6B94DB5BE105E7B58A34D4DC11412";

		public static string WorldResourceManagerResourceLockField = "5378A366A1927C9686ABCFD6316F5AE6";

		///////////////////////////////////////////////////////////////////

		public static string SandboxGameNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string SandboxGameGameStatsClass = "8EE387052960DB6076920F525374285D";
		public static string SandboxGameGetGameStatsInstance = "9FADFDED60BE5D614F2CA2DB13B753DE";
		public static string SandboxGameGetUpdatesPerSecondField = "87D39FAEA45F9E56D966C7580B2E42B7";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected WorldManager( )
		{
			m_instance = this;

			Console.WriteLine( "Finished loading WorldManager" );
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
					LogManager.ErrorLog.WriteLine( ex );
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
					LogManager.ErrorLog.WriteLine( ex );
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
				Console.WriteLine( ex );
				return false;
			}
		}

		/*
		public static string SandboxGameNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string SandboxGameGameStatsClass = "8EE387052960DB6076920F525374285D";
		public static string SandboxGameGetGameStatsInstance = "9FADFDED60BE5D614F2CA2DB13B753DE";
		public static string SandboxGameGetUpdatesPerSecondField = "87D39FAEA45F9E56D966C7580B2E42B7";
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
				LogManager.ErrorLog.WriteLine( ex );
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
										LogManager.APILog.WriteLineAndConsole(string.Format("Asynchronous Save Setup Started: {0}ms", (DateTime.Now - saveStartTime).TotalMilliseconds));
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

						LogManager.APILog.WriteLineAndConsole( string.Format( "Asynchronous Save Completed: {0}ms", ( DateTime.Now - saveStartTime ).TotalMilliseconds ) );
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
						LogManager.APILog.WriteLineAndConsole(string.Format("Asynchronous Save Setup Time: {0}ms", (DateTime.Now - saveStartTime).TotalMilliseconds));
						saveStartTime = DateTime.Now;
						result = (bool)BaseObject.InvokeEntityMethod(parameters[0], WorldManagerSaveSnapshot);
					}
					else
					{
						LogManager.ErrorLog.WriteLine("Failed to save world (1)");
						return;
					}

					if (result)
					{
						LogManager.APILog.WriteLineAndConsole(string.Format("Asynchronous Save Successful: {0}ms", (DateTime.Now - saveStartTime).TotalMilliseconds));
					}
					else
					{
						LogManager.ErrorLog.WriteLine("Failed to save world (2)");
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
				LogManager.ErrorLog.WriteLine(ex);
			}
			finally
			{
				m_isSaving = false;
			}
			 */
		}

		internal Object InternalGetFactionManager( )
		{
			try
			{
				Object worldManager = BaseObject.GetEntityFieldValue( BackingObject, WorldManagerFactionManagerField );

				return worldManager;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
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
				LogManager.ErrorLog.WriteLine( ex );
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
					LogManager.APILog.WriteLineAndConsole( "Save complete and took " + timeToSave.TotalSeconds + " seconds" );
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
					LogManager.APILog.WriteLineAndConsole( "Save failed!" );
				}
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
			finally
			{
				m_isSaving = false;
			}
		}

		#endregion "Methods"
	}
}