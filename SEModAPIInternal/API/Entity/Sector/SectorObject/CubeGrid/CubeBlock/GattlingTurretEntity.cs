using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "GatlingTurretEntityProxy" )]
	public class GatlingTurretEntity : TurretBaseEntity
	{
		#region "Attributes"

		public static string GatlingTurretNamespace = "";
		public static string GatlingTurretClass = "";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public GatlingTurretEntity( CubeGridEntity parent, MyObjectBuilder_LargeGatlingTurret definition )
			: base( parent, definition )
		{
		}

		public GatlingTurretEntity( CubeGridEntity parent, MyObjectBuilder_LargeGatlingTurret definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"
	}
}