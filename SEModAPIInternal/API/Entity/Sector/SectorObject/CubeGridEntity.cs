using VRage.Game;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using Sandbox;
    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Definitions;
    using Sandbox.Game.Entities;
    using Sandbox.Game.Entities.Cube;
    using Sandbox.ModAPI;
    using SEModAPI.API;
    using SEModAPI.API.Utility;
    using SEModAPIInternal.API.Common;
    using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
    using SEModAPIInternal.API.Utility;
    using SEModAPIInternal.Support;
    using VRage.ObjectBuilders;
    using VRageMath;

    [DataContract( Name = "CubeGridEntityProxy" )]
	[KnownType( "KnownTypes" )]
	public class CubeGridEntity : BaseEntity
	{
		#region "Attributes"

		private readonly CubeBlockManager _cubeBlockManager;
		private readonly CubeGridNetworkManager _networkManager;
		private readonly CubeGridManagerManager _managerManager;

		private static Type _internalType;
		private string _name;
		private DateTime _lastNameRefresh;

		private CubeBlockEntity _cubeBlockToAddRemove;

		public static string CubeGridNamespace = "";
		public static string CubeGridClass = "Sandbox.Game.Entities.MyCubeGrid";

		public static string CubeGridGetCubeBlocksHashSetMethod = "GetBlocks";
		public static string CubeGridAddCubeBlockMethod = "AddCubeBlock";
		public static string CubeGridRemoveCubeBlockMethod = "RemoveBlock";
		public static string CubeGridGetManagerManagerMethod = "get_GridSystems";

		//public static string CubeGridBlockGroupsField = "=Pzc3AyrJQWUIjBgew6TwHdil5t=";

		//////////////////////////////////////////////////////////////

		public static string CubeGridPackedCubeBlockClass = ".MyBlockBuildArea";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected CubeGridEntity( )
			: base( new MyObjectBuilder_CubeGrid( ) )
		{
			_cubeBlockManager = new CubeBlockManager( this );
			_lastNameRefresh = DateTime.Now;
			_name = "Cube Grid";
		}

		public CubeGridEntity( FileInfo prefabFile )
			: base( BaseObjectManager.LoadContentFile<MyObjectBuilder_CubeGrid>( prefabFile ) )
		{
			EntityId = 0;
			ObjectBuilder.EntityId = 0;
			if ( ObjectBuilder.PositionAndOrientation != null )
				PositionAndOrientation = ObjectBuilder.PositionAndOrientation.GetValueOrDefault( );

			_cubeBlockManager = new CubeBlockManager( this );
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>( );
			foreach ( MyObjectBuilder_CubeBlock cubeBlock in ObjectBuilder.CubeBlocks )
			{
				cubeBlock.EntityId = 0;
				cubeBlockList.Add( new CubeBlockEntity( this, cubeBlock ) );
			}
			_cubeBlockManager.Load( cubeBlockList );

			_lastNameRefresh = DateTime.Now;
			_name = "Cube Grid";
		}

		public CubeGridEntity( MyObjectBuilder_CubeGrid definition )
			: base( definition )
		{
			_cubeBlockManager = new CubeBlockManager( this );
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>( );
			foreach ( MyObjectBuilder_CubeBlock cubeBlock in definition.CubeBlocks )
			{
				cubeBlock.EntityId = 0;
				cubeBlockList.Add( new CubeBlockEntity( this, cubeBlock ) );
			}
			_cubeBlockManager.Load( cubeBlockList );

			_lastNameRefresh = DateTime.Now;
			_name = "Cube Grid";
		}

		public CubeGridEntity( MyObjectBuilder_CubeGrid definition, Object backingObject )
			: base( definition, backingObject )
		{
			_cubeBlockManager = new CubeBlockManager( this, backingObject, CubeGridGetCubeBlocksHashSetMethod );
			_cubeBlockManager.Refresh( );

			_networkManager = new CubeGridNetworkManager( this );
			_managerManager = new CubeGridManagerManager( this, GetManagerManager( ) );

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent
			{
				type = EntityEventManager.EntityEventType.OnCubeGridCreated,
				timestamp = DateTime.Now,
				entity = this,
				priority = 1
			};
			EntityEventManager.Instance.AddEvent( newEvent );

			_lastNameRefresh = DateTime.Now;
			_name = "Cube Grid";
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new internal static Type InternalType
		{
			get
			{
				if ( _internalType == null )
					_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CubeGridNamespace, CubeGridClass );
				return _internalType;
			}
		}

		[DataMember]
		[Category( "Cube Grid" )]
		[ReadOnly( true )]
		public override string Name
		{
			get
			{
				string name = string.Empty;
				TimeSpan timeSinceLastNameRefresh = DateTime.Now - _lastNameRefresh;
				if ( timeSinceLastNameRefresh.TotalSeconds < 2 )
				{
					name = _name;
				}
				else
				{
					_lastNameRefresh = DateTime.Now;

					List<MyObjectBuilder_CubeBlock> blocks = new List<MyObjectBuilder_CubeBlock>( ObjectBuilder.CubeBlocks );
					foreach ( MyObjectBuilder_CubeBlock cubeBlock in blocks )
					{
						try
						{
							if ( cubeBlock == null )
								continue;
							if ( cubeBlock.TypeId != typeof( MyObjectBuilder_Beacon ) )
							{
								continue;
							}
							if ( name.Length > 0 )
								name += "|";

							string customName = ( (MyObjectBuilder_Beacon)cubeBlock ).CustomName;
							if ( customName == string.Empty )
								customName = "Beacon";
							name += customName;
						}
						catch ( Exception ex )
						{
							ApplicationLog.BaseLog.Error( ex );
						}
					}
				}

				if ( name.Length == 0 )
					name = DisplayName;

				if ( name.Length == 0 )
					name = ObjectBuilder.EntityId.ToString( );

				_name = name;

				return name;
			}
		}

		[DataMember]
		[Category( "Cube Grid" )]
		[ReadOnly( true )]
		public override string DisplayName
		{
			get { return ObjectBuilder.DisplayName; }
			set
			{
				if ( ObjectBuilder.DisplayName == value ) return;
				ObjectBuilder.DisplayName = value;
				Changed = true;

				base.DisplayName = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_CubeGrid ObjectBuilder
		{
			get
			{
				MyObjectBuilder_CubeGrid objectBuilder = (MyObjectBuilder_CubeGrid)base.ObjectBuilder;
				if ( objectBuilder == null )
				{
					objectBuilder = new MyObjectBuilder_CubeGrid( );
					ObjectBuilder = objectBuilder;
				}

				objectBuilder.LinearVelocity = LinearVelocity;
				objectBuilder.AngularVelocity = AngularVelocity;

				return objectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Cube Grid" )]
		[ReadOnly( true )]
		public MyCubeSize GridSizeEnum
		{
			get { return ObjectBuilder.GridSizeEnum; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Cube Grid" )]
		[ReadOnly( true )]
		public bool IsStatic
		{
			get { return ObjectBuilder.IsStatic; }
			private set
			{
				//Do nothing!
			}
		}

		//[DataMember]
		//[Category( "Cube Grid" )]
		//public bool IsDampenersEnabled
		//{
		//	get { return ObjectBuilder.DampenersEnabled; }
		//	set
		//	{
		//		if ( ObjectBuilder.DampenersEnabled == value ) return;
		//		ObjectBuilder.DampenersEnabled = value;
		//		Changed = true;

		//		if ( ThrusterManager != null )
		//		{
		//			ThrusterManager.DampenersEnabled = value;
		//		}
		//	}
		//}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public List<CubeBlockEntity> CubeBlocks
		{
			get
			{
				List<CubeBlockEntity> cubeBlocks = _cubeBlockManager.GetTypedInternalData<CubeBlockEntity>( );
				return cubeBlocks;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		public List<MyObjectBuilder_CubeBlock> BaseCubeBlocks
		{
			get
			{
				List<MyObjectBuilder_CubeBlock> cubeBlocks = ObjectBuilder.CubeBlocks;
				return cubeBlocks;
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		public List<BoneInfo> Skeleton
		{
			get { return ObjectBuilder.Skeleton; }
		}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		public List<MyObjectBuilder_ConveyorLine> ConveyorLines
		{
			get { return ObjectBuilder.ConveyorLines; }
		}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		public List<MyObjectBuilder_BlockGroup> BlockGroups
		{
			get { return ObjectBuilder.BlockGroups; }
		}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public CubeGridNetworkManager NetworkManager
		{
			get { return _networkManager; }
			private set
			{
				//Do nothing!
			}
		}

		//[IgnoreDataMember]
		//[Category( "Cube Grid" )]
		//[Browsable( false )]
		//[ReadOnly( true )]
		//public CubeGridThrusterManager ThrusterManager
		//{
		//	get { return _managerManager.ThrusterManager; }
		//	private set
		//	{
		//		//Do nothing!
		//	}
		//}

		[IgnoreDataMember]
		[Category( "Cube Grid" )]
		[Browsable( false )]
		public bool IsLoading
		{
			get
			{
				bool isLoading = true;

				isLoading = isLoading && _cubeBlockManager.IsLoading;

				return isLoading;
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static List<Type> KnownTypes( )
		{
			return UtilityFunctions.GetObjectBuilderTypes( );
		}

		public override void Dispose( )
		{
			if ( ExtenderOptions.IsDebugging )
				ApplicationLog.BaseLog.Debug( "Disposing CubeGridEntity '" + Name + "' ..." );

			//Dispose the cube grid by disposing all of the blocks
			//This may be slow but it's reliable ... so far
			/*
			List<CubeBlockEntity> blocks = CubeBlocks;
			int blockCount = blocks.Count;
			foreach (CubeBlockEntity cubeBlock in blocks)
			{
				cubeBlock.Dispose();
			}
			if (ExtenderOptions.IsDebugging)
				ApplicationLog.BaseLog.Debug("Disposed " + blockCount.ToString() + " blocks on CubeGridEntity '" + Name + "'");
			*/
			//Broadcast the removal to the clients just to save processing time for the clients
			BaseNetworkManager.RemoveEntity( );

			m_isDisposed = true;

			if ( EntityId != 0 )
			{
				GameEntityManager.RemoveEntity( EntityId );
			}

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent
			{
				type = EntityEventManager.EntityEventType.OnCubeGridDeleted,
				timestamp = DateTime.Now,
				entity = this,
				priority = 1
			};
			EntityEventManager.Instance.AddEvent( newEvent );
		}

		public override void Export( FileInfo fileInfo )
		{
			RefreshBaseCubeBlocks( );

			MyObjectBuilderSerializer.SerializeXML( fileInfo.FullName, false, ObjectBuilder );
		}

		new public MyObjectBuilder_CubeGrid Export( )
		{
			RefreshBaseCubeBlocks( );

			return ObjectBuilder;
		}

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for CubeGridEntity" );
				bool result = true;
				result &= Reflection.HasMethod( type, CubeGridGetCubeBlocksHashSetMethod );
				result &= Reflection.HasMethod( type, CubeGridAddCubeBlockMethod );
				result &= Reflection.HasMethod( type, CubeGridRemoveCubeBlockMethod );
				result &= Reflection.HasMethod( type, CubeGridGetManagerManagerMethod );
				//result &= HasField( type, CubeGridBlockGroupsField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public CubeBlockEntity GetCubeBlock( Vector3I cubePosition )
		{
			try
			{
				long packedBlockCoordinates = (long)cubePosition.X + (long)cubePosition.Y * 10000 + (long)cubePosition.Z * 100000000;
				CubeBlockEntity cubeBlock = (CubeBlockEntity)_cubeBlockManager.GetEntry( packedBlockCoordinates );

				return cubeBlock;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		public void AddCubeBlock( CubeBlockEntity cubeBlock )
		{
			_cubeBlockToAddRemove = cubeBlock;

			MySandboxGame.Static.Invoke( InternalAddCubeBlock );
		}

		public void DeleteCubeBlock( CubeBlockEntity cubeBlock )
		{
			_cubeBlockToAddRemove = cubeBlock;

			MySandboxGame.Static.Invoke( InternalRemoveCubeBlock );
		}

		protected void RefreshBaseCubeBlocks( )
		{
			MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)ObjectBuilder;

			//Refresh the cube blocks content in the cube grid from the cube blocks manager
			cubeGrid.CubeBlocks.Clear( );
			foreach ( CubeBlockEntity item in CubeBlocks )
			{
				cubeGrid.CubeBlocks.Add( (MyObjectBuilder_CubeBlock)item.ObjectBuilder );
			}
		}

		/// <summary>
		/// Repairs all <see cref="CubeBlockEntity">CubeBlockEntities</see> in this <see cref="CubeGridEntity"/>
		/// </summary>
		public void Repair( )
		{
			foreach ( CubeBlockEntity block in CubeBlocks )
			{
				block.Repair( );
			}
            MyCubeGrid newEntity = (MyCubeGrid)Entity;
            foreach ( MySlimBlock slimBlock in newEntity.CubeBlocks )
                newEntity.ResetBlockSkeleton( slimBlock, true );            
		}

		#region "Internal"

		protected Object GetManagerManager( )
		{
			Object result = InvokeEntityMethod( BackingObject, CubeGridGetManagerManagerMethod );
			return result;
		}

		protected void InternalCubeGridMovedEvent( Object entity )
		{
			try
			{
				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent
				{
					type = EntityEventManager.EntityEventType.OnCubeGridMoved,
					timestamp = DateTime.Now,
					entity = this,
					priority = 9
				};
				EntityEventManager.Instance.AddEvent( newEvent );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalAddCubeBlock( )
		{
			if ( _cubeBlockToAddRemove == null )
				return;

			try
			{
				MyObjectBuilder_CubeBlock objectBuilder = _cubeBlockToAddRemove.ObjectBuilder;
				MyCubeBlockDefinition blockDef = MyDefinitionManager.Static.GetCubeBlockDefinition( objectBuilder );

				NetworkManager.BroadcastAddCubeBlock( _cubeBlockToAddRemove );

				Object result = InvokeEntityMethod( BackingObject, CubeGridAddCubeBlockMethod, new object[ ] { objectBuilder, true, blockDef } );
				_cubeBlockToAddRemove.BackingObject = result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			_cubeBlockToAddRemove = null;
		}

		protected void InternalRemoveCubeBlock( )
		{
			if ( _cubeBlockToAddRemove == null )
				return;

			//NOTE - We don't broadcast the removal because the game internals take care of that by broadcasting the removal delta lists every frame update

			InvokeEntityMethod( BackingObject, CubeGridRemoveCubeBlockMethod, new object[ ] { _cubeBlockToAddRemove.BackingObject, Type.Missing } );

			_cubeBlockToAddRemove = null;
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}