namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class RefineryEntity : ProductionBlockEntity
	{
		#region "Attributes"

		public static string RefineryNamespace = "Sandbox.Game.Entities.Cube";
		public static string RefineryClass = "MyRefinery";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public RefineryEntity( CubeGridEntity parent, MyObjectBuilder_Refinery definition )
			: base( parent, definition )
		{
		}

		public RefineryEntity( CubeGridEntity parent, MyObjectBuilder_Refinery definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}