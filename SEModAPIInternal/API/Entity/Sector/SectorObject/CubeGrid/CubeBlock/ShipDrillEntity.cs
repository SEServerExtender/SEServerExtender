using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "ShipDrillEntityProxy" )]
	public class ShipDrillEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_inventory;

		public static string ShipDrillNamespace = "Sandbox.Game.Weapons";
		public static string ShipDrillClass = "MyShipDrill";

		public static string ShipDrillGetInventoryMethod = "GetInventory";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ShipDrillEntity( CubeGridEntity parent, MyObjectBuilder_Drill definition )
			: base( parent, definition )
		{
			m_inventory = new InventoryEntity( definition.Inventory );
		}

		public ShipDrillEntity( CubeGridEntity parent, MyObjectBuilder_Drill definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_inventory = new InventoryEntity( definition.Inventory, InternalGetContainerInventory( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[DataMember]
		[Category( "Drill" )]
		[Browsable( false )]
		[ReadOnly( true )]
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

		#endregion "Properties"

		#region "Methods"

		#region "Internal"

		protected Object InternalGetContainerInventory( )
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject( );
				Object inventory = InvokeEntityMethod( actualObject, ShipDrillGetInventoryMethod, new object[ ] { 0 } );

				return inventory;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}