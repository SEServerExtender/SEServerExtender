using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

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
