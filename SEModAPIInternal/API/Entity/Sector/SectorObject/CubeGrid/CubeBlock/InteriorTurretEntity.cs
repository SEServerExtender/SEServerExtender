using System;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "InteriorTurretEntityProxy")]
	public class InteriorTurretEntity : TurretBaseEntity
	{
		#region "Attributes"

		public static string InteriorTurretNamespace = "";
		public static string InteriorTurretClass = "";

		#endregion

		#region "Constructors and Intializers"

		public InteriorTurretEntity(CubeGridEntity parent, MyObjectBuilder_InteriorTurret definition)
			: base(parent, definition)
		{
		}

		public InteriorTurretEntity(CubeGridEntity parent, MyObjectBuilder_InteriorTurret definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		#endregion

		#region "Methods"

		#endregion
	}
}
