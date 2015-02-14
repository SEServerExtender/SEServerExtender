namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	using System;
	using System.Collections.Generic;
	using Sandbox.Definitions;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;
	using VRage;

	public class VoxelMapMaterialManager
	{
		#region "Attributes"

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