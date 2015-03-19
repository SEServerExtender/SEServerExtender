using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "ConveyorBlockEntityProxy" )]
	public class ConveyorBlockEntity : CubeBlockEntity
	{
		#region "Attributes"

		public static string ConveyorBlockNamespace = "";
		public static string ConveyorBlockClass = "=TOO9vEcUQ4GPDzJaGQ7GgjTGAt=";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ConveyorBlockEntity( CubeGridEntity parent, MyObjectBuilder_Conveyor definition )
			: base( parent, definition )
		{
		}

		public ConveyorBlockEntity( CubeGridEntity parent, MyObjectBuilder_Conveyor definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}