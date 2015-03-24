namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class ConveyorTubeEntity : CubeBlockEntity
	{
		#region "Attributes"

		public static string ConveyorTubeNamespace = "";
		public static string ConveyorTubeClass = "";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ConveyorTubeEntity( CubeGridEntity parent, MyObjectBuilder_ConveyorConnector definition )
			: base( parent, definition )
		{
		}

		public ConveyorTubeEntity( CubeGridEntity parent, MyObjectBuilder_ConveyorConnector definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}