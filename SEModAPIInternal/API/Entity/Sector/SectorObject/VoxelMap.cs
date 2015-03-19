using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using VRage.Common.Voxels;
using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract]
	public class VoxelMap : BaseEntity
	{
		#region "Attributes"

		private static Type _internalType;

		private VoxelMapMaterialManager _materialManager;
		private MyStorageDataCache _cache;
		private readonly Dictionary<MyVoxelMaterialDefinition, float> _materialTotals;

		public static string VoxelMapNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string VoxelMapClass = "6EC806B54BA319767DA878841A56ECD8";

		public static string VoxelMapGetSizeMethod = "F7FC06F8DAF6ECC3F74F1D863DD65A36";

		//public static string VoxelMapGetVoxelMaterialManagerMethod = "1543B7CCAB7538E6877BA8CCC513A070";
		public static string VoxelMapGetVoxelMaterialManagerMethod = "61D7D905B19D162AF69D27DD9B2ADC58";

		public static string VoxelMapGetMaterialAtPositionMethod = "5F7E3213E519961F42617BC410B19346";

		/*
		 * Storage recompute for asteroids? CC67DA892A0C9277CC606E1B4C97A4F1.6922E99EC72C10627AA239B8167BF7DC.95CFB363D0BF8BBC6CBFC7248263FD6A
		 * Add a new voxel? 5BCAC68007431E61367F5B2CF24E2D6F.6EC806B54BA319767DA878841A56ECD8.01774E4E0A0FC967FD0C28D949278314
		 * Add a new voxel static? AAC05F537A6F0F6775339593FBDFC564.3F0C9546C1796109CAF2EB98B70C8049.40A5DC2FA0E0E2380A9DAB38B4953D0C
		 * Add a new voxel static by enum? AAC05F537A6F0F6775339593FBDFC564.3F0C9546C1796109CAF2EB98B70C8049.48D3319B987E8C194FDFEB0EFC41E8DF(MyMwcVoxelFilesEnum asteroidType, Vector3 position, string name, bool unknown)
		 * Generate voxel materials? 6B85614235D7D81095FD26C72DC7E1D1
		 * Possible creative voxel creation? 688215302A767A55ECDA0D653CFEED6F
		 * When user selects a roid in creative shift-f10 menu: 41312BD862D355BC5C4023FC22286611
		 *
		 */

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public VoxelMap( MyObjectBuilder_VoxelMap definition )
			: base( definition )
		{
		}

		public VoxelMap( MyObjectBuilder_VoxelMap definition, Object backingObject )
			: base( definition, backingObject )
		{
			//_materialManager = new VoxelMapMaterialManager(this, GetMaterialManager());
			_materialTotals = new Dictionary<MyVoxelMaterialDefinition, float>( );
			//RefreshCache();
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Voxel Map" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new internal static Type InternalType
		{
			get { return _internalType ?? ( _internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( VoxelMapNamespace, VoxelMapClass ) ); }
		}

		[DataMember]
		[Category( "Voxel Map" )]
		[Browsable( true )]
		[ReadOnly( true )]
		public override string Name
		{
			get { return Filename; }
		}

		[IgnoreDataMember]
		[Category( "Voxel Map" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_VoxelMap ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_VoxelMap)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Voxel Map" )]
		[ReadOnly( true )]
		public string Filename
		{
			get { return ObjectBuilder.StorageName; }
		}

		[DataMember]
		[Category( "Voxel Map" )]
		[ReadOnly( true )]
		public Vector3 Size
		{
			get
			{
				return BackingObject == null ? Vector3.Zero : GetVoxelMapSize( );
			}
		}

		[DataMember]
		[Category( "Voxel Map" )]
		[ReadOnly( true )]
		[Description( "Volume of the voxel map in cubic meters" )]
		public float Volume
		{
			get
			{
				return _cache == null ? 0 : _cache.Data.Length;
			}
		}

		[DataMember]
		[Category( "Voxel Map" )]
		[ReadOnly( true )]
		[Description( "Approximate mass of the voxel map in kg" )]
		new public float Mass
		{
			get
			{
				if ( BackingObject == null )
					return base.Mass;

				//Mass is estimated based on default ratio of 1kg = 0.37 cubic meters of ore
				//Note: This is not a realistic mass as this volume of silicate would be ~135kg
				return Volume * 19.727f;
			}
		}

		[IgnoreDataMember]
		[Category( "Voxel Map" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public Dictionary<MyVoxelMaterialDefinition, float> Materials
		{
			get
			{
				if ( BackingObject == null )
					return null;

				if ( _cache == null )
				{
					RefreshCache( );
				}

				return _materialTotals;
			}
		}

		[IgnoreDataMember]
		[Category( "Voxel Map" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal VoxelMapMaterialManager MaterialManager
		{
			get { return _materialManager; }
		}

		#endregion "Properties"

		#region "Methods"

		[Obsolete("Please file an issue if this method is used or it will be removed in SE version 1.70.", true)]
		public MyVoxelMaterialDefinition GetMaterial( Vector3I voxelPosition )
		{
			return GetMaterialAt( voxelPosition );
		}

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new TypeLoadException( "Could not find internal type for VoxelMap" );

				result &= HasMethod( type, VoxelMapGetSizeMethod );
				result &= HasMethod( type, VoxelMapGetVoxelMaterialManagerMethod );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected Vector3 GetVoxelMapSize( )
		{
			Object rawResult = InvokeEntityMethod( BackingObject, VoxelMapGetSizeMethod );
			if ( rawResult == null )
				return Vector3.Zero;
			Vector3 result = (Vector3)rawResult;
			return result;
		}

		protected Object GetMaterialManager( )
		{
			Object result = InvokeEntityMethod( BackingObject, VoxelMapGetVoxelMaterialManagerMethod );
			return result;
		}

		protected MyVoxelMaterialDefinition GetMaterialAt( Vector3I voxelPosition )
		{
			return InvokeEntityMethod( BackingObject, VoxelMapGetMaterialAtPositionMethod, new object[ ] { voxelPosition } ) as MyVoxelMaterialDefinition;
		}

		protected void RefreshCache( )
		{
			IMyVoxelMap voxelMap = (IMyVoxelMap)BackingObject;
			_cache = new MyStorageDataCache( );
			Vector3I size = voxelMap.Storage.Size;
			_cache.Resize( size );

			//			SandboxGameAssemblyWrapper.Instance.GameAction(() =>
			//			{
			voxelMap.Storage.ReadRange( _cache, MyStorageDataTypeFlags.Material, 0, Vector3I.Zero, size - 1 );
			//voxelMap.Storage.ReadRange(_cache, MyStorageDataTypeFlags.Material, Vector3I.Zero, size - 1);
			//			});

			foreach ( byte materialIndex in _cache.Data )
			{
				try
				{
					MyVoxelMaterialDefinition material = MyDefinitionManager.Static.GetVoxelMaterialDefinition( materialIndex );
					if ( material == null )
						continue;

					if ( !_materialTotals.ContainsKey( material ) )
						_materialTotals.Add( material, 1 );
					else
						_materialTotals[ material ]++;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}
		}

		#endregion "Methods"
	}
}