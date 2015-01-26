using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "InteriorLightEntityProxy" )]
	public class InteriorLightEntity : LightEntity
	{
		#region "Attributes"

		public static string InteriorLightNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string InteriorLightClass = "05750D6A5B237D5ABEB54E060707026B";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public InteriorLightEntity( CubeGridEntity parent, MyObjectBuilder_InteriorLight definition )
			: base( parent, definition )
		{
		}

		public InteriorLightEntity( CubeGridEntity parent, MyObjectBuilder_InteriorLight definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"
	}
}