namespace SEModAPIInternal.API.Entity
{
	using System.IO;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.ObjectBuilders;

	public class SectorManager : BaseObjectManager
	{
		#region "Attributes"

		private SectorEntity _Sector;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorManager( )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public SectorEntity Sector
		{
			get { return _Sector; }
		}

		#endregion "Properties"

		#region "Methods"

		public void Load( FileInfo fileInfo )
		{
			//Save the file info to the property
			FileInfo = fileInfo;

			//Read in the sector data
			MyObjectBuilder_Sector data = ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>( FileInfo.FullName );

			//And instantiate the sector with the data
			_Sector = new SectorEntity( data );
		}

		new public bool Save( )
		{
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>( _Sector.ObjectBuilder, FileInfo.FullName );
		}

		#endregion "Methods"
	}
}