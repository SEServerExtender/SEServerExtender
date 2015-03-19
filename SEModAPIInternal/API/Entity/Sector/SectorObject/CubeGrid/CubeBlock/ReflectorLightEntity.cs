using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "ReflectorLightEntityProxy" )]
	public class ReflectorLightEntity : LightEntity
	{
		#region "Attributes"

		public static string ReflectorLightNamespace = "";
		public static string ReflectorLightClass = "=LI5taOob2o87FserNhrJciGlxO=";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ReflectorLightEntity( CubeGridEntity parent, MyObjectBuilder_ReflectorLight definition )
			: base( parent, definition )
		{
		}

		public ReflectorLightEntity( CubeGridEntity parent, MyObjectBuilder_ReflectorLight definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"
	}
}