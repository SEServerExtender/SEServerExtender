namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;

	[DataContract]
	public class MissileTurretEntity : TurretBaseEntity
	{
		#region "Attributes"

		public static string MissileTurretNamespace = "";
		public static string MissileTurretClass = "";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public MissileTurretEntity( CubeGridEntity parent, MyObjectBuilder_LargeMissileTurret definition )
			: base( parent, definition )
		{
		}

		public MissileTurretEntity( CubeGridEntity parent, MyObjectBuilder_LargeMissileTurret definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}