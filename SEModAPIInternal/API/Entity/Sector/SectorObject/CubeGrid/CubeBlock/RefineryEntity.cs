using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "RefineryEntityProxy" )]
	public class RefineryEntity : ProductionBlockEntity
	{
		#region "Attributes"

		public static string RefineryNamespace = "";
		public static string RefineryClass = "=8SP9fGLhWKHDI8VQ7qCDMW9aEE=";

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