using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "ShipToolBaseEntityProxy" )]
	public class ShipToolBaseEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_inventory;

		public static string ShipToolBaseNamespace = "Sandbox.Game.Weapons";
		public static string ShipToolBaseClass = "MyShipToolBase";

		public static string ShipToolBaseGetInventoryMethod = "GetInventory";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ShipToolBaseEntity( CubeGridEntity parent, MyObjectBuilder_ShipToolBase definition )
			: base( parent, definition )
		{
			m_inventory = new InventoryEntity( definition.Inventory );
		}

		public ShipToolBaseEntity( CubeGridEntity parent, MyObjectBuilder_ShipToolBase definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_inventory = new InventoryEntity( definition.Inventory, InternalGetContainerInventory( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[DataMember]
		[Category( "Ship Tool Base" )]
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
				Object inventory = InvokeEntityMethod( actualObject, ShipToolBaseGetInventoryMethod, new object[ ] { 0 } );

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