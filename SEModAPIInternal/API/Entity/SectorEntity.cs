using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class SectorEntity : BaseObject
	{
		#region "Attributes"

		//Sector Events
		private BaseObjectManager _eventManager;

		//Sector Objects
		private BaseObjectManager _cubeGridManager;

		private BaseObjectManager _voxelMapManager;
		private BaseObjectManager _floatingObjectManager;
		private BaseObjectManager _meteorManager;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorEntity( MyObjectBuilder_Sector definition )
			: base( definition )
		{
			_eventManager = new BaseObjectManager( );
			_cubeGridManager = new BaseObjectManager( );
			_voxelMapManager = new BaseObjectManager( );
			_floatingObjectManager = new BaseObjectManager( );
			_meteorManager = new BaseObjectManager( );

			List<Event> events = new List<Event>( );
			foreach ( MyObjectBuilder_GlobalEventBase sectorEvent in definition.SectorEvents.Events )
			{
				events.Add( new Event( sectorEvent ) );
			}

			List<CubeGridEntity> cubeGrids = new List<CubeGridEntity>( );
			List<VoxelMap> voxelMaps = new List<VoxelMap>( );
			List<FloatingObject> floatingObjects = new List<FloatingObject>( );
			List<Meteor> meteors = new List<Meteor>( );
			foreach ( MyObjectBuilder_EntityBase sectorObject in definition.SectorObjects )
			{
				if ( sectorObject.TypeId == typeof( MyObjectBuilder_CubeGrid ) )
				{
					cubeGrids.Add( new CubeGridEntity( (MyObjectBuilder_CubeGrid)sectorObject ) );
				}
				else if ( sectorObject.TypeId == typeof( MyObjectBuilder_VoxelMap ) )
				{
					voxelMaps.Add( new VoxelMap( (MyObjectBuilder_VoxelMap)sectorObject ) );
				}
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
			_eventManager.Load( events );
			_cubeGridManager.Load( cubeGrids );
			_voxelMapManager.Load( voxelMaps );
			_floatingObjectManager.Load( floatingObjects );
			_meteorManager.Load( meteors );
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
			get { return "SANDBOX_" + Position.X + "_" + Position.Y + "_" + Position.Z + "_"; }
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
					foreach ( Event item in _eventManager.GetTypedInternalData<Event>( ) )
					{
						baseSector.SectorEvents.Events.Add( item.ObjectBuilder );
					}

					//Update the sector objects in the base definition
					baseSector.SectorObjects.Clear( );
					foreach ( CubeGridEntity item in _cubeGridManager.GetTypedInternalData<CubeGridEntity>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( VoxelMap item in _voxelMapManager.GetTypedInternalData<VoxelMap>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( FloatingObject item in _floatingObjectManager.GetTypedInternalData<FloatingObject>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( Meteor item in _meteorManager.GetTypedInternalData<Meteor>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
				return baseSector;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category( "Sector" )]
		public VRageMath.Vector3I Position
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
				List<Event> newList = _eventManager.GetTypedInternalData<Event>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<CubeGridEntity> CubeGrids
		{
			get
			{
				List<CubeGridEntity> newList = _cubeGridManager.GetTypedInternalData<CubeGridEntity>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<VoxelMap> VoxelMaps
		{
			get
			{
				List<VoxelMap> newList = _voxelMapManager.GetTypedInternalData<VoxelMap>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<FloatingObject> FloatingObjects
		{
			get
			{
				List<FloatingObject> newList = _floatingObjectManager.GetTypedInternalData<FloatingObject>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<Meteor> Meteors
		{
			get
			{
				List<Meteor> newList = _meteorManager.GetTypedInternalData<Meteor>( );
				return newList;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public BaseObject NewEntry( Type newType )
		{
			if ( newType == typeof( CubeGridEntity ) )
				return _cubeGridManager.NewEntry<CubeGridEntity>( );
			if ( newType == typeof( VoxelMap ) )
				return _voxelMapManager.NewEntry<VoxelMap>( );
			if ( newType == typeof( FloatingObject ) )
				return _floatingObjectManager.NewEntry<FloatingObject>( );
			if ( newType == typeof( Meteor ) )
				return _meteorManager.NewEntry<Meteor>( );

			return null;
		}

		public bool DeleteEntry( Object source )
		{
			Type deleteType = source.GetType( );
			if ( deleteType == typeof( CubeGridEntity ) )
				return _cubeGridManager.DeleteEntry( (CubeGridEntity)source );
			if ( deleteType == typeof( VoxelMap ) )
				return _voxelMapManager.DeleteEntry( (VoxelMap)source );
			if ( deleteType == typeof( FloatingObject ) )
				return _floatingObjectManager.DeleteEntry( (FloatingObject)source );
			if ( deleteType == typeof( Meteor ) )
				return _meteorManager.DeleteEntry( (Meteor)source );

			return false;
		}

		#endregion "Methods"
	}
}