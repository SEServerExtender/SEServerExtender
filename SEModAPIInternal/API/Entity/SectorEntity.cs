using VRage.Game;

namespace SEModAPIInternal.API.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Sandbox;
    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Game.Entities;
    using Sandbox.Game.Multiplayer;
    using Sandbox.ModAPI;
    using SEModAPI.API;
    using SEModAPI.API.Utility;
    using SEModAPIInternal.API.Common;
    using SEModAPIInternal.API.Entity.Sector;
    using SEModAPIInternal.API.Entity.Sector.SectorObject;
    using SEModAPIInternal.API.Utility;
    using SEModAPIInternal.Support;
    using VRage.ObjectBuilders;
    using VRageMath;
    using Sandbox.Engine.Multiplayer;
    using Sandbox.Game.Replication;
    using VRage.Game.Entity;
    public class SectorEntity : BaseObject
	{
		#region "Attributes"

		//Sector Events
		private BaseObjectManager m_eventManager;

		//Sector Objects
		private BaseObjectManager m_cubeGridManager;

		private BaseObjectManager m_voxelMapManager;
		private BaseObjectManager m_floatingObjectManager;
		private BaseObjectManager m_meteorManager;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorEntity( MyObjectBuilder_Sector definition )
			: base( definition )
		{
			m_eventManager = new BaseObjectManager( );
			m_cubeGridManager = new BaseObjectManager( );
			m_voxelMapManager = new BaseObjectManager( );
			m_floatingObjectManager = new BaseObjectManager( );
			m_meteorManager = new BaseObjectManager( );

			List<Event> events = new List<Event>( );
			foreach ( MyObjectBuilder_GlobalEventBase sectorEvent in definition.SectorEvents.Events )
			{
				events.Add( new Event( sectorEvent ) );
			}

			List<CubeGridEntity> cubeGrids = new List<CubeGridEntity>( );
			//List<VoxelMap> voxelMaps = new List<VoxelMap>( );
			List<FloatingObject> floatingObjects = new List<FloatingObject>( );
			List<Meteor> meteors = new List<Meteor>( );
			foreach ( MyObjectBuilder_EntityBase sectorObject in definition.SectorObjects )
			{
				if ( sectorObject.TypeId == typeof( MyObjectBuilder_CubeGrid ) )
				{
					cubeGrids.Add( new CubeGridEntity( (MyObjectBuilder_CubeGrid)sectorObject ) );
				}
				//else if ( sectorObject.TypeId == typeof( MyObjectBuilder_VoxelMap ) )
				//{
					//voxelMaps.Add( new VoxelMap( (MyObjectBuilder_VoxelMap)sectorObject ) );
				//}
				else if ( sectorObject.TypeId == typeof( MyObjectBuilder_FloatingObject ) )
				{
					floatingObjects.Add( new FloatingObject( (MyObjectBuilder_FloatingObject)sectorObject ) );
				}
				else if ( sectorObject.TypeId == typeof( MyObjectBuilder_Meteor ) )
				{
					meteors.Add( new Meteor( (MyObjectBuilder_Meteor)sectorObject ) );
				}
			}

			//Build the managers from the lists
			m_eventManager.Load( events );
			m_cubeGridManager.Load( cubeGrids );
			//m_voxelMapManager.Load( voxelMaps );
			m_floatingObjectManager.Load( floatingObjects );
			m_meteorManager.Load( meteors );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[Category( "Sector" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The formatted name of the object" )]
		public override string Name
		{
			get { return "SANDBOX_" + this.Position.X + "_" + this.Position.Y + "_" + this.Position.Z + "_"; }
		}

		[Category( "Sector" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Sector ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Sector baseSector = (MyObjectBuilder_Sector)base.ObjectBuilder;

				try
				{
					//Update the events in the base definition
					baseSector.SectorEvents.Events.Clear( );
					foreach ( Event item in m_eventManager.GetTypedInternalData<Event>( ) )
					{
						baseSector.SectorEvents.Events.Add( item.ObjectBuilder );
					}

					//Update the sector objects in the base definition
					baseSector.SectorObjects.Clear( );
					foreach ( CubeGridEntity item in m_cubeGridManager.GetTypedInternalData<CubeGridEntity>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					//foreach ( VoxelMap item in m_voxelMapManager.GetTypedInternalData<VoxelMap>( ) )
					//{
					//	baseSector.SectorObjects.Add( item.ObjectBuilder );
					//}
					foreach ( FloatingObject item in m_floatingObjectManager.GetTypedInternalData<FloatingObject>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( Meteor item in m_meteorManager.GetTypedInternalData<Meteor>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
				return baseSector;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category( "Sector" )]
		public Vector3I Position
		{
			get { return ObjectBuilder.Position; }
		}

		[Category( "Sector" )]
		public int AppVersion
		{
			get { return ObjectBuilder.AppVersion; }
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<Event> Events
		{
			get
			{
				List<Event> newList = m_eventManager.GetTypedInternalData<Event>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<CubeGridEntity> CubeGrids
		{
			get
			{
				List<CubeGridEntity> newList = m_cubeGridManager.GetTypedInternalData<CubeGridEntity>( );
				return newList;
			}
		}
        /*
		[Category( "Sector" )]
		[Browsable( false )]
		public List<VoxelMap> VoxelMaps
		{
			get
			{
				List<VoxelMap> newList = m_voxelMapManager.GetTypedInternalData<VoxelMap>( );
				return newList;
			}
		}
        */
		[Category( "Sector" )]
		[Browsable( false )]
		public List<FloatingObject> FloatingObjects
		{
			get
			{
				List<FloatingObject> newList = m_floatingObjectManager.GetTypedInternalData<FloatingObject>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<Meteor> Meteors
		{
			get
			{
				List<Meteor> newList = m_meteorManager.GetTypedInternalData<Meteor>( );
				return newList;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public BaseObject NewEntry( Type newType )
		{
			if ( newType == typeof( CubeGridEntity ) )
				return m_cubeGridManager.NewEntry<CubeGridEntity>( );
			//if ( newType == typeof( VoxelMap ) )
			//	return m_voxelMapManager.NewEntry<VoxelMap>( );
			if ( newType == typeof( FloatingObject ) )
				return m_floatingObjectManager.NewEntry<FloatingObject>( );
			if ( newType == typeof( Meteor ) )
				return m_meteorManager.NewEntry<Meteor>( );

			return null;
		}

		public bool DeleteEntry( Object source )
		{
			Type deleteType = source.GetType( );
			if ( deleteType == typeof( CubeGridEntity ) )
				return m_cubeGridManager.DeleteEntry( (CubeGridEntity)source );
			//if ( deleteType == typeof( VoxelMap ) )
			//	return m_voxelMapManager.DeleteEntry( (VoxelMap)source );
			if ( deleteType == typeof( FloatingObject ) )
				return m_floatingObjectManager.DeleteEntry( (FloatingObject)source );
			if ( deleteType == typeof( Meteor ) )
				return m_meteorManager.DeleteEntry( (Meteor)source );

			return false;
		}

		#endregion "Methods"
	}

	public class SectorObjectManager : BaseObjectManager
	{
		#region "Attributes"

		private static SectorObjectManager m_instance;
		private static readonly Queue<BaseEntity> AddEntityQueue = new Queue<BaseEntity>( );

		public static string ObjectManagerNamespace = "Sandbox.Game.Entities";
		public static string ObjectManagerClass = "MyEntities";
		public static string ObjectManagerAddEntity = "Add";

		/////////////////////////////////////////////////////////////////

		public static string ObjectFactoryNamespace = "Sandbox.Game.Entities";
		public static string ObjectFactoryClass = "MyEntityFactory";

		/////////////////////////////////////////////////////////////////

		//2 Packet Types
		public static string EntityBaseNetManagerNamespace = "Sandbox.Game.Multiplayer";

		public static string EntityBaseNetManagerClass = "MySyncCreate";
		public static string EntityBaseNetManagerSendEntity = "SendEntityCreated";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorObjectManager( )
		{
			IsDynamic = true;
			m_instance = this;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static SectorObjectManager Instance
		{
			get
			{
				if ( m_instance == null )
					m_instance = new SectorObjectManager( );

				return m_instance;
			}
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
				if ( AddEntityQueue.Count >= 25 )
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
				result &= Reflection.HasMethod( type, ObjectManagerAddEntity );

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ObjectFactoryNamespace, ObjectFactoryClass );
				if ( type2 == null )
					throw new Exception( "Could not find object factory type for SectorObjectManager" );

				Type type3 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( EntityBaseNetManagerNamespace, EntityBaseNetManagerClass );
				if ( type3 == null )
					throw new Exception( "Could not find entity base network manager type for SectorObjectManager" );
				result &= Reflection.HasMethod( type3, EntityBaseNetManagerSendEntity );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
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
					//entityType != VoxelMap.InternalType &&
					entityType != FloatingObject.InternalType &&
					entityType != Meteor.InternalType
					)
					return false;

				//Skip disposed entities
				bool isDisposed = (bool)BaseEntity.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetIsDisposedMethod );
				if ( isDisposed )
					return false;

				//Skip entities that have invalid physics objects
				if ( BaseEntity.GetRigidBody( entity ) == null || BaseEntity.GetRigidBody( entity ).IsDisposed )
					return false;

				//Skip entities that don't have a position-orientation matrix defined
				if ( BaseEntity.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetOrientationMatrixMethod ) == null )
					return false;

				return true;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected override void InternalRefreshBackingDataHashSet( )
		{
			try
			{
				if ( !CanRefresh )
					return;

				m_rawDataHashSetResourceLock.AcquireExclusive( );

				object rawValue = MyEntities.GetEntities( );
				if ( rawValue == null )
					return;

				//Create/Clear the hash set
				if ( m_rawDataHashSet == null )
					m_rawDataHashSet = new HashSet<object>( );
				else
					m_rawDataHashSet.Clear( );

				//Only allow valid entities in the hash set
				foreach ( object entry in UtilityFunctions.ConvertHashSet( rawValue ) )
				{
					if ( !IsValidEntity( entry ) )
						continue;

					m_rawDataHashSet.Add( entry );
				}

				m_rawDataHashSetResourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				if ( m_rawDataHashSetResourceLock.Owned )
					m_rawDataHashSetResourceLock.ReleaseExclusive( );
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
						//ApplicationLog.BaseLog.Error( ex );
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
						//ApplicationLog.BaseLog.Error( ex );
					}
				}
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
			}
		}

		public void AddEntity( BaseEntity entity )
		{
			try
			{
				if ( AddEntityQueue.Count >= 25 )
				{
					throw new Exception( "AddEntity queue is full. Cannot add more entities yet" );
				}

				if ( ExtenderOptions.IsDebugging )
					ApplicationLog.BaseLog.Debug(String.Format("{0} '{1}': Is being added...", entity.GetType().Name, entity.DisplayName));

				AddEntityQueue.Enqueue( entity );

				MySandboxGame.Static.Invoke( InternalAddEntity );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalAddEntity( )
		{
			try
			{
				if ( AddEntityQueue.Count == 0 )
					return;

				BaseEntity entityToAdd = AddEntityQueue.Dequeue( );

                if (ExtenderOptions.IsDebugging)
                    ApplicationLog.BaseLog.Debug(String.Format("{0} '{1}': Adding to scene...", entityToAdd.GetType().Name, entityToAdd.DisplayName));

				//Create the backing object
				Type entityType = entityToAdd.GetType( );
				Type internalType = (Type)BaseEntity.InvokeStaticMethod( entityType, "get_InternalType" );
				if ( internalType == null )
					throw new Exception( "Could not get internal type of entity" );
				entityToAdd.BackingObject = Activator.CreateInstance( internalType );

				//Add the backing object to the main game object manager
                //I don't think this is actually used anywhere?
				MyEntity backingObject = (MyEntity)entityToAdd.BackingObject;

                MyEntity newEntity = MyEntities.CreateFromObjectBuilderAndAdd(entityToAdd.ObjectBuilder);
                

                if ( entityToAdd is FloatingObject )
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = backingObject.GetObjectBuilder( );
						//TODO - Do stuff

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch ( Exception ex )
					{
						ApplicationLog.BaseLog.Error( "Failed to broadcast new floating object" );
						ApplicationLog.BaseLog.Error( ex );
					}
				}
				else
				{
					try
					{
						//Broadcast the new entity to the clients
                        ApplicationLog.BaseLog.Info("Broadcasted entity to clients.");
                        MyMultiplayer.ReplicateImmediatelly( MyExternalReplicable.FindByObject( newEntity ) );
                        //the misspelling in this function name is driving me  i n s a n e
                    }
					catch ( Exception ex )
					{
						ApplicationLog.BaseLog.Error( "Failed to broadcast new entity" );
						ApplicationLog.BaseLog.Error( ex );
					}
				}

				if ( ExtenderOptions.IsDebugging )
				{
					Type type = entityToAdd.GetType( );
					ApplicationLog.BaseLog.Debug(String.Format("{0} '{1}': Finished adding to scene", entityToAdd.GetType().Name, entityToAdd.DisplayName));
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}

	public class SectorManager : BaseObjectManager
	{
		#region "Attributes"

		private SectorEntity m_Sector;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorManager( )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public SectorEntity Sector
		{
			get { return m_Sector; }
		}

		#endregion "Properties"

		#region "Methods"

		public void Load( FileInfo fileInfo )
		{
			//Save the file info to the property
			FileInfo = fileInfo;

			//Read in the sector data
			MyObjectBuilder_Sector data;
			MyObjectBuilderSerializer.DeserializeXML(FileInfo.FullName,out data );

			//And instantiate the sector with the data
			m_Sector = new SectorEntity( data );
		}

		new public bool Save( )
		{
			return MyObjectBuilderSerializer.SerializeXML( FileInfo.FullName, false, m_Sector.ObjectBuilder );
		}

		#endregion "Methods"
	}
}