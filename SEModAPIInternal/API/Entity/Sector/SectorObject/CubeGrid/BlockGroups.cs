using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	internal class BlockGroup : BaseObject
	{
		public BlockGroup( CubeGridEntity parent, MyObjectBuilder_BlockGroup definition )
			: base( definition )
		{
		}
	}

	internal class BlockGroupManager
	{
	}
}