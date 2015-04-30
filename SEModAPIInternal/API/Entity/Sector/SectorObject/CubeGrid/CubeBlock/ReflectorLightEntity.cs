namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class ReflectorLightEntity : LightEntity
	{
		#region "Attributes"

		public static string ReflectorLightNamespace = "Sandbox.Game.Entities";
		public static string ReflectorLightClass = "MyReflectorLight";

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