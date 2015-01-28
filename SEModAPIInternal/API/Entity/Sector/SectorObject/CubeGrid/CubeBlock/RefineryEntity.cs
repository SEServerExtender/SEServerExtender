using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "RefineryEntityProxy" )]
	public class RefineryEntity : ProductionBlockEntity
	{
		#region "Attributes"

		public static string RefineryNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string RefineryClass = "D213D513B024AA8BF8DAC576FC59CB54";

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