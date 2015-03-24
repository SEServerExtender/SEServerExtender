namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class ShipGrinderEntity : ShipToolBaseEntity
	{
		#region "Attributes"

		public static string ShipGrinderNamespace = "";
		public static string ShipGrinderClass = "";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ShipGrinderEntity( CubeGridEntity parent, MyObjectBuilder_ShipGrinder definition )
			: base( parent, definition )
		{
		}

		public ShipGrinderEntity( CubeGridEntity parent, MyObjectBuilder_ShipGrinder definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}