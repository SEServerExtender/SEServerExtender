using VRage.Game;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	using Sandbox.Common.ObjectBuilders;

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