using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "AssemblerEntityProxy" )]
	public class AssemblerEntity : ProductionBlockEntity
	{
		#region "Attributes"

		public static string AssemblerNamespace = "";
		public static string AssemblerClass = "=HEfaqb80qdXtP0YwG2EV0OQFs=";

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