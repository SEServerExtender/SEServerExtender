namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class ConveyorBlockEntity : CubeBlockEntity
	{
		public static string ConveyorBlockNamespace = "Sandbox.Game.Entities";
		public static string ConveyorBlockClass = "MyConveyor";

		public ConveyorBlockEntity( CubeGridEntity parent, MyObjectBuilder_Conveyor definition )
			: base( parent, definition )
		{
		}

		public ConveyorBlockEntity( CubeGridEntity parent, MyObjectBuilder_Conveyor definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}
	}
}