using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using VRage;
using VRage.Common.Voxels;
using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract( Name = "VoxelMapProxy" )]
	public class VoxelMap : BaseEntity
	{
		#region "Attributes"

		private static Type m_internalType;

		private VoxelMapMaterialManager m_materialManager;
		private MyStorageDataCache m_cache;
		private Dictionary<MyVoxelMaterialDefinition, float> m_materialTotals;

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
			//m_materialManager = new VoxelMapMaterialManager(this, GetMaterialManager());
			m_materialTotals = new Dictionary<MyVoxelMaterialDefinition, float>( );
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
			get
			{
				if ( m_internalType == null )
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( VoxelMapNamespace, VoxelMapClass );
				return m_internalType;
			}
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
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Voxel Map" )]
		[ReadOnly( true )]
		public Vector3 Size
		{
			get
			{
				if ( BackingObject == null )
					return Vector3.Zero;

				return GetVoxelMapSize( );
			}
			private set
			{
				//Do nothing!
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
				if ( m_cache == null )
					return 0;

				return m_cache.Data.Length;
			}
			private set
			{
				//Do nothing!
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
			private set
			{
				//Do nothing!
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

				if ( m_cache == null )
				{
					RefreshCache( );
				}

				return m_materialTotals;
			}
		}

		[IgnoreDataMember]
		[Category( "Voxel Map" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal VoxelMapMaterialManager MaterialManager
		{
			get { return m_materialManager; }
		}

		#endregion "Properties"

		#region "Methods"

		public MyVoxelMaterialDefinition GetMaterial( Vector3I voxelPosition )
		{
			try
			{
				return GetMaterialAt( voxelPosition );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for VoxelMap" );

				result &= HasMethod( type, VoxelMapGetSizeMethod );
				result &= HasMethod( type, VoxelMapGetVoxelMaterialManagerMethod );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
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
			Object rawResult = InvokeEntityMethod( BackingObject, VoxelMapGetMaterialAtPositionMethod, new object[ ] { voxelPosition } );
			if ( rawResult == null )
				return null;
			MyVoxelMaterialDefinition result = (MyVoxelMaterialDefinition)rawResult;
			return result;
		}

		protected void RefreshCache( )
		{
			IMyVoxelMap voxelMap = (IMyVoxelMap)BackingObject;
			m_cache = new MyStorageDataCache( );
			Vector3I size = voxelMap.Storage.Size;
			m_cache.Resize( size );

			//			SandboxGameAssemblyWrapper.Instance.GameAction(() =>
			//			{
			voxelMap.Storage.ReadRange( m_cache, MyStorageDataTypeFlags.Material, 0, Vector3I.Zero, size - 1 );
			//voxelMap.Storage.ReadRange(m_cache, MyStorageDataTypeFlags.Material, Vector3I.Zero, size - 1);
			//			});

			foreach ( byte materialIndex in m_cache.Data )
			{
				try
				{
					MyVoxelMaterialDefinition material = MyDefinitionManager.Static.GetVoxelMaterialDefinition( materialIndex );
					if ( material == null )
						continue;

					if ( !m_materialTotals.ContainsKey( material ) )
						m_materialTotals.Add( material, 1 );
					else
						m_materialTotals[ material ]++;
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		#endregion "Methods"
	}

	public class VoxelMapMaterialManager
	{
		#region "Attributes"

		private VoxelMap m_parent;
		private Object m_backingObject;
		private int m_voxelCount;
		private FastResourceLock m_resourceLock;
		private Dictionary<MyVoxelMaterialDefinition, float> m_materialTotals;

		public static string VoxelMapMaterialManagerNamespace = "DC3F8F35BD18173B1D075139B475AD8E";
		public static string VoxelMapMaterialManagerClass = "119B0A83D4E9B352826763AD3746A162";

		public static string VoxelMapMaterialManagerGetVoxelsDictionaryMethod = "3B4214480FDA5B1811A72EEBB55B543C";

		public static string VoxelMapMaterialManagerVoxelsField = "4E39EA62F3374F5CCE29BA40FE62818C";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public VoxelMapMaterialManager( VoxelMap parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;

			m_resourceLock = new FastResourceLock( );
			m_materialTotals = new Dictionary<MyVoxelMaterialDefinition, float>( );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( VoxelMapMaterialManagerNamespace, VoxelMapMaterialManagerClass );
				return type;
			}
		}

		internal Object BackingObject
		{
			get { return m_backingObject; }
		}

		internal Dictionary<MyVoxelMaterialDefinition, float> Materials
		{
			get
			{
				if ( BackingObject == null )
					return m_materialTotals;

				object[ ] voxels = GetVoxels( );
				if ( voxels.Length == m_voxelCount )
					return m_materialTotals;

				m_resourceLock.AcquireExclusive( );

				m_voxelCount = voxels.Length;
				m_materialTotals.Clear( );

				foreach ( object entry in voxels )
				{
					try
					{
						if ( entry == null )
							continue;
						Object rawIndex = BaseObject.InvokeEntityMethod( entry, "42F2A6C72643C1E13B243E1B5A8E075B" );
						if ( rawIndex == null )
							continue;
						byte materialIndex = (byte)rawIndex;
						MyVoxelMaterialDefinition material = MyDefinitionManager.Static.GetVoxelMaterialDefinition( materialIndex );
						if ( material == null )
							continue;
						if ( !m_materialTotals.ContainsKey( material ) )
							m_materialTotals.Add( material, 1 );
						else
							m_materialTotals[ material ]++;
					}
					catch ( Exception ex )
					{
						LogManager.ErrorLog.WriteLine( ex );
					}
				}

				m_resourceLock.ReleaseExclusive( );

				return m_materialTotals;
			}
		}

		internal int VoxelCount
		{
			get { return m_voxelCount; }
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;
				/*
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for VoxelMapMaterialManager");

				result &= BaseObject.HasMethod(type, VoxelMapMaterialManagerGetVoxelsDictionaryMethod);

				result &= BaseObject.HasField(type, VoxelMapMaterialManagerVoxelsField);
				*/
				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		protected object[ ] GetVoxels( )
		{
			Object rawResult = BaseObject.GetEntityFieldValue( BackingObject, VoxelMapMaterialManagerVoxelsField );
			if ( rawResult == null )
				return null;
			object[ ] result = (object[ ])rawResult;
			return result;
		}

		#endregion "Methods"
	}
}