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
	[DataContract(Name = "ShipConnectorEntity")]
	public class ShipConnectorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_inventory;
		public static string ShipConnectorGetInventoryMethod = "GetInventory";

		#endregion

		#region "Constructors and Intializers"

		public ShipConnectorEntity(CubeGridEntity parent, MyObjectBuilder_ShipConnector definition)
			: base(parent, definition)
		{
			m_inventory = new InventoryEntity(definition.Inventory);
		}

		public ShipConnectorEntity(CubeGridEntity parent, MyObjectBuilder_ShipConnector definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_inventory = new InventoryEntity(definition.Inventory, InternalGetContainerInventory());
		}

		#endregion

		#region "Properties"

		[DataMember]
		[Category("Connector")]
		[Browsable(false)]
		[ReadOnly(true)]
		public InventoryEntity Inventory
		{
			get
			{
				return m_inventory;
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		#region "Internal"

		protected Object InternalGetContainerInventory()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();
				Object inventory = InvokeEntityMethod(actualObject, ShipConnectorGetInventoryMethod, new object[] { 0 });

				return inventory;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		#endregion

		#endregion
	}
}
