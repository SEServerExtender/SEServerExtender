namespace SEModAPIInternal.API.Entity
{
	using System.IO;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.ObjectBuilders;

	public class SectorManager : BaseObjectManager
	{
		#region "Attributes"

		private SectorEntity _sector;

		#endregion "Attributes"

		#region "Properties"

		public SectorEntity Sector
		{
			get { return _sector; }
		}

		#endregion "Properties"

		#region "Methods"

		/// <exception cref="FileNotFoundException">The file specified cannot be found.</exception>
		public void Load( FileInfo fileInfo )
		{
			//Save the file info to the property
			FileInfo = fileInfo;

			//Read in the sector data
			MyObjectBuilder_Sector data = ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>( FileInfo.FullName );

			//And instantiate the sector with the data
			_sector = new SectorEntity( data );
		}

		new public bool Save( )
		{
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>( _sector.ObjectBuilder, FileInfo.FullName );
		}

		#endregion "Methods"
	}
}