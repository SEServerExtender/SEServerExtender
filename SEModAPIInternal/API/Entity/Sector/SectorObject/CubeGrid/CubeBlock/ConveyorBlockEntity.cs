namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class ConveyorBlockEntity : CubeBlockEntity
	{
		public static string ConveyorBlockNamespace = "";
		public static string ConveyorBlockClass = "=TOO9vEcUQ4GPDzJaGQ7GgjTGAt=";

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