using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	internal class ConveyorLine : BaseObject
	{
		public ConveyorLine( CubeGridEntity parent, MyObjectBuilder_ConveyorLine definition )
			: base( definition )
		{
		}
	}

	internal class ConveyorLineManager
	{
	}
}