namespace SEModAPIInternal.API.Entity
{
	using System;
	using System.Collections.Generic;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;

	public class SectorObjectManager : BaseObjectManager
	{
		#region "Attributes"

		private static SectorObjectManager _instance;
		private static Queue<BaseEntity> _addEntityQueue = new Queue<BaseEntity>( );

		public static string ObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectManagerClass = "CAF1EB435F77C7B77580E2E16F988BED";
		public static string ObjectManagerGetEntityHashSet = "84C54760C0F0DDDA50B0BE27B7116ED8";
		public static string ObjectManagerAddEntity = "E5E18F5CAD1F62BB276DF991F20AE6AF";

		/////////////////////////////////////////////////////////////////

		public static string ObjectFactoryNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectFactoryClass = "E825333D6467D99DD83FB850C600395C";

		/////////////////////////////////////////////////////////////////

		//2 Packet Types
		public static string EntityBaseNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";

		public static string EntityBaseNetManagerClass = "8EFE49A46AB934472427B7D117FD3C64";
		public static string EntityBaseNetManagerSendEntity = "A6B585C993B43E72219511726BBB0649";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorObjectManager( )
		{
			IsDynamic = true;
			_instance = this;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static SectorObjectManager Instance
		{
			get { return _instance ?? ( _instance = new SectorObjectManager( ) ); }
		}

		public static Type InternalType
		{
			get
			{
				Type objectManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ObjectManagerNamespace, ObjectManagerClass );
				return objectManagerType;
			}
		}

		public static bool QueueFull
		{
			get
			{
				if ( _addEntityQueue.Count >= 25 )
					return true;

				return false;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for SectorObjectManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type, ObjectManagerGetEntityHashSet );
				result &= BaseObject.HasMethod( type, ObjectManagerAddEntity );

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ObjectFactoryNamespace, ObjectFactoryClass );
				if ( type2 == null )
					throw new Exception( "Could not find object factory type for SectorObjectManager" );

				Type type3 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( EntityBaseNetManagerNamespace, EntityBaseNetManagerClass );
				if ( type3 == null )
					throw new Exception( "Could not find entity base network manager type for SectorObjectManager" );
				result &= BaseObject.HasMethod( type3, EntityBaseNetManagerSendEntity );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		protected override bool IsValidEntity( Object entity )
		{
			try
			{
				if ( entity == null )
					return false;

				//Skip unknowns for now until we get the bugs sorted out with the other types
				Type entityType = entity.GetType( );
				if ( entityType != CharacterEntity.InternalType &&
				     entityType != CubeGridEntity.InternalType &&
				     entityType != VoxelMap.InternalType &&
				     entityType != FloatingObject.InternalType &&
				     entityType != Meteor.InternalType
					)
					return false;

				//Skip disposed entities
				bool isDisposed = (bool)BaseObject.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetIsDisposedMethod );
				if ( isDisposed )
					return false;

				//Skip entities that have invalid physics objects
				if ( BaseEntity.GetRigidBody( entity ) == null || BaseEntity.GetRigidBody( entity ).IsDisposed )
					return false;

				//Skip entities that don't have a position-orientation matrix defined
				if ( BaseObject.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetOrientationMatrixMethod ) == null )
					return false;

				return true;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		protected override void InternalRefreshBackingDataHashSet( )
		{
			try
			{
				if ( !CanRefresh )
					return;

				RawDataHashSetResourceLock.AcquireExclusive( );

				object rawValue = BaseObject.InvokeStaticMethod( InternalType, ObjectManagerGetEntityHashSet );
				if ( rawValue == null )
					return;

				//Create/Clear the hash set
				if ( RawDataHashSet == null )
					RawDataHashSet = new HashSet<object>( );
				else
					RawDataHashSet.Clear( );

				//Only allow valid entities in the hash set
				foreach ( object entry in UtilityFunctions.ConvertHashSet( rawValue ) )
				{
					if ( !IsValidEntity( entry ) )
						continue;

					RawDataHashSet.Add( entry );
				}

				RawDataHashSetResourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				if ( RawDataHashSetResourceLock.Owned )
					RawDataHashSetResourceLock.ReleaseExclusive( );
			}
		}

		protected override void LoadDynamic( )
		{
			try
			{
				HashSet<Object> rawEntities = GetBackingDataHashSet( );
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>( GetInternalData( ) );

				//Update the main data mapping
				foreach ( Object entity in rawEntities )
				{
					try
					{
						long entityId = BaseEntity.GetEntityId( entity );
						if ( entityId == 0 )
							continue;

						if ( !IsValidEntity( entity ) )
							continue;

						MyObjectBuilder_EntityBase baseEntity = BaseEntity.GetObjectBuilder( entity );
						if ( baseEntity == null )
							continue;
						if ( !EntityRegistry.Instance.ContainsGameType( baseEntity.TypeId ) )
							continue;

						//If the original data already contains an entry for this, skip creation and just update values
						if ( GetInternalData( ).ContainsKey( entityId ) )
						{
							BaseEntity matchingEntity = (BaseEntity)GetEntry( entityId );
							if ( matchingEntity == null || matchingEntity.IsDisposed )
								continue;

							matchingEntity.BackingObject = entity;
							matchingEntity.ObjectBuilder = baseEntity;
						}
						else
						{
							BaseEntity newEntity = null;

							//Get the matching API type from the registry
							Type apiType = EntityRegistry.Instance.GetAPIType( baseEntity.TypeId );

							//Create a new API entity
							newEntity = (BaseEntity)Activator.CreateInstance( apiType, new object[ ] { baseEntity, entity } );

							if ( newEntity != null )
								AddEntry( newEntity.EntityId, newEntity );
						}
					}
					catch ( Exception ex )
					{
						LogManager.ErrorLog.WriteLine( ex );
					}
				}

				//Cleanup old entities
				foreach ( KeyValuePair<long, BaseObject> entry in internalDataCopy )
				{
					try
					{
						if ( !rawEntities.Contains( entry.Value.BackingObject ) )
							DeleteEntry( entry.Value );
					}
					catch ( Exception ex )
					{
						LogManager.ErrorLog.WriteLine( ex );
					}
				}
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		public void AddEntity( BaseEntity entity )
		{
			try
			{
				if ( _addEntityQueue.Count >= 25 )
				{
					throw new Exception( "AddEntity queue is full. Cannot add more entities yet" );
				}

				if ( SandboxGameAssemblyWrapper.IsDebugging )
					Console.WriteLine( entity.GetType( ).Name + " '" + entity.Name + "' is being added ..." );

				_addEntityQueue.Enqueue( entity );

				Action action = InternalAddEntity;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		protected void InternalAddEntity( )
		{
			try
			{
				if ( _addEntityQueue.Count == 0 )
					return;

				BaseEntity entityToAdd = _addEntityQueue.Dequeue( );

				if ( SandboxGameAssemblyWrapper.IsDebugging )
					Console.WriteLine( entityToAdd.GetType( ).Name + " '" + entityToAdd.GetType( ).Name + "': Adding to scene ..." );

				//Create the backing object
				Type entityType = entityToAdd.GetType( );
				Type internalType = (Type)BaseObject.InvokeStaticMethod( entityType, "get_InternalType" );
				if ( internalType == null )
					throw new Exception( "Could not get internal type of entity" );
				entityToAdd.BackingObject = Activator.CreateInstance( internalType );

				//Initialize the backing object
				BaseObject.InvokeEntityMethod( entityToAdd.BackingObject, "Init", new object[ ] { entityToAdd.ObjectBuilder } );

				//Add the backing object to the main game object manager
				BaseObject.InvokeStaticMethod( InternalType, ObjectManagerAddEntity, new object[ ] { entityToAdd.BackingObject, true } );

				if ( entityToAdd is FloatingObject )
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseObject.InvokeEntityMethod( entityToAdd.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[ ] { Type.Missing } );
						//TODO - Do stuff

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch ( Exception ex )
					{
						LogManager.APILog.WriteLineAndConsole( "Failed to broadcast new floating object" );
						LogManager.ErrorLog.WriteLine( ex );
					}
				}
				else
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseObject.InvokeEntityMethod( entityToAdd.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[ ] { Type.Missing } );
						Type someManager = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( EntityBaseNetManagerNamespace, EntityBaseNetManagerClass );
						BaseObject.InvokeStaticMethod( someManager, EntityBaseNetManagerSendEntity, new object[ ] { baseEntity } );

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch ( Exception ex )
					{
						LogManager.APILog.WriteLineAndConsole( "Failed to broadcast new entity" );
						LogManager.ErrorLog.WriteLine( ex );
					}
				}

				if ( SandboxGameAssemblyWrapper.IsDebugging )
				{
					Type type = entityToAdd.GetType( );
					Console.WriteLine( type.Name + " '" + entityToAdd.Name + "': Finished adding to scene" );
				}
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}