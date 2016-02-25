using VRage.Game;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	using Sandbox.Common.ObjectBuilders;

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