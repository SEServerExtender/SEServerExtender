using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "AssemblerEntityProxy" )]
	public class AssemblerEntity : ProductionBlockEntity
	{
		#region "Attributes"

		public static string AssemblerNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string AssemblerClass = "B5257F4B8254F1D432BFCE1B5DCC1A5E";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public AssemblerEntity( CubeGridEntity parent, MyObjectBuilder_Assembler definition )
			: base( parent, definition )
		{
		}

		public AssemblerEntity( CubeGridEntity parent, MyObjectBuilder_Assembler definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}