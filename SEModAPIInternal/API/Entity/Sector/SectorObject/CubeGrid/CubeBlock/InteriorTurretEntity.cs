namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class InteriorTurretEntity : TurretBaseEntity
	{
		#region "Attributes"

		public static string InteriorTurretNamespace = "";
		public static string InteriorTurretClass = "";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public InteriorTurretEntity( CubeGridEntity parent, MyObjectBuilder_InteriorTurret definition )
			: base( parent, definition )
		{
		}

		public InteriorTurretEntity( CubeGridEntity parent, MyObjectBuilder_InteriorTurret definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}