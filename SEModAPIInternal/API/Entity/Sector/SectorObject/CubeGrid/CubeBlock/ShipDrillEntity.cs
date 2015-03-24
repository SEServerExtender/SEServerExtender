namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.Support;

	[DataContract]
	public class ShipDrillEntity : FunctionalBlockEntity
	{
		public static string ShipDrillNamespace = "Sandbox.Game.Weapons";
		public static string ShipDrillClass = "MyShipDrill";

		public static string ShipDrillGetInventoryMethod = "GetInventory";

		public ShipDrillEntity( CubeGridEntity parent, MyObjectBuilder_Drill definition )
			: base( parent, definition )
		{
			Inventory = new InventoryEntity( definition.Inventory );
		}

		public ShipDrillEntity( CubeGridEntity parent, MyObjectBuilder_Drill definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			Inventory = new InventoryEntity( definition.Inventory, InternalGetContainerInventory( ) );
		}

		[DataMember]
		[Category( "Drill" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public InventoryEntity Inventory { get; private set; }

		protected Object InternalGetContainerInventory( )
		{
			try
			{
				Object actualObject = GetActualObject( );
				Object inventory = InvokeEntityMethod( actualObject, ShipDrillGetInventoryMethod, new object[ ] { 0 } );

				return inventory;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}
	}
}