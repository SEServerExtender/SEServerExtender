using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using VRage;

//using Sandbox.ModAPI.Interfaces;

namespace SEModAPIInternal.API.Entity
{
	[DataContract]
	[KnownType( typeof( InventoryItemEntity ) )]
	public class InventoryEntity : BaseObject
	{
		#region "Attributes"

		private InventoryItemManager m_itemManager;

		public static string InventoryNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryClass = "DE48496EE9812E665B802D5FE9E7AD77";

		public static string InventoryCalculateMassVolumeMethod = "166CC20258091AEA72B666F9EF9503F4";
		public static string InventoryGetTotalVolumeMethod = "C8CB569A2F9A58A24BAC40AB0817AD6A";
		public static string InventoryGetTotalMassMethod = "4E701A33F8803398A50F20D8BF2E5507";
		public static string InventorySetFromObjectBuilderMethod = "D85F2B547D9197E27D0DB9D5305D624F";
		public static string InventoryGetObjectBuilderMethod = "EFBD3CF8717682D7B59A5878FF97E0BB";
		public static string InventoryCleanUpMethod = "476A04917356C2C5FFE23B1CBFC11450";
		public static string InventoryGetItemListMethod = "C43E297C0F568726D4BDD5D71B901911";
		public static string InventoryAddItemAmountMethod = "FB009222ACFCEACDC546801B06DDACB6";
		public static string InventoryRemoveItemAmountMethod = "623B0AC0E7D9C30410680C76A55F0C6B";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public InventoryEntity( MyObjectBuilder_Inventory definition )
			: base( definition )
		{
			/*
			m_itemManager = new InventoryItemManager(this);

			List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
			foreach (MyObjectBuilder_InventoryItem item in definition.Items)
			{
				InventoryItemEntity newItem = new InventoryItemEntity(item);
				newItem.Container = this;
				itemList.Add(newItem);
			}
			m_itemManager.Load(itemList);
			 */
		}

		public InventoryEntity( MyObjectBuilder_Inventory definition, Object backingObject )
			: base( definition, backingObject )
		{
			//m_itemManager = new InventoryItemManager(this, backingObject, InventoryGetItemListMethod);
			//m_itemManager.Refresh();
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Container Inventory" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( InventoryNamespace, InventoryClass );
				return type;
			}
		}

		[IgnoreDataMember]
		[Category( "Container Inventory" )]
		[ReadOnly( true )]
		public override string Name
		{
			get { return "Inventory"; }
		}

		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[Description( "Object builder data of the object" )]
		internal new MyObjectBuilder_Inventory ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)base.ObjectBuilder;

				return inventory;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Container Inventory" )]
		[ReadOnly( true )]
		public uint NextItemId
		{
			get
			{
				uint result = 0;
				if ( BackingObject != null )
				{
					SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
					                                                {
						                                                IMyInventory inventory = (IMyInventory) BackingObject;
						                                                result = (uint) inventory.GetItems( ).Count;
					                                                } );
				}

				return result;
				/*
                MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)InvokeEntityMethod(BackingObject, InventoryGetObjectBuilderMethod);
                ObjectBuilder = inventory;
                return ObjectBuilder.nextItemId;
				 */
			}
		}

		[IgnoreDataMember]
		[Category( "Container Inventory" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public List<InventoryItemEntity> Items
		{
			get
			{
				try
				{
					List<InventoryItemEntity> newList = new List<InventoryItemEntity>( );

					SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
					                                                {
						                                                if ( BackingObject != null )
						                                                {
							                                                IMyInventory myInventory = (IMyInventory) BackingObject;
							                                                foreach ( Sandbox.ModAPI.Interfaces.IMyInventoryItem item in myInventory.GetItems( ) )
							                                                {
								                                                InventoryItemEntity newItem = new InventoryItemEntity( item, this );
								                                                newList.Add( newItem );
							                                                }
						                                                }
					                                                } );

					return newList;

					/*
					List<InventoryItemEntity> newList = m_itemManager.GetTypedInternalData<InventoryItemEntity>();
					return newList;
					 */
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
					return new List<InventoryItemEntity>( );
				}
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new NotSupportedException( "Could not find internal type for InventoryEntity" );
				bool result = true;
				result &= HasMethod( type, InventoryCalculateMassVolumeMethod );
				result &= HasMethod( type, InventoryGetTotalVolumeMethod );
				result &= HasMethod( type, InventoryGetTotalMassMethod );
				result &= HasMethod( type, InventorySetFromObjectBuilderMethod );
				result &= HasMethod( type, InventoryGetObjectBuilderMethod );
				result &= HasMethod( type, InventoryCleanUpMethod );
				result &= HasMethod( type, InventoryGetItemListMethod );
				result &= HasMethod( type, InventoryAddItemAmountMethod );

				Type[ ] argTypes = new Type[ 3 ];
				argTypes[ 0 ] = typeof( MyFixedPoint );
				argTypes[ 1 ] = typeof( MyObjectBuilder_PhysicalObject );
				argTypes[ 2 ] = typeof( bool );
				result &= HasMethod( type, InventoryRemoveItemAmountMethod, argTypes );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		public InventoryItemEntity NewEntry( )
		{
			/*
			MyObjectBuilder_InventoryItem defaults = new MyObjectBuilder_InventoryItem();
			SerializableDefinitionId itemTypeId = new SerializableDefinitionId(typeof(MyObjectBuilder_Ore), "Stone");

			//defaults.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(itemTypeId);
            defaults.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(itemTypeId);
			defaults.Amount = 1;

			InventoryItemEntity newItem = new InventoryItemEntity(defaults);
			newItem.ItemId = NextItemId;
			m_itemManager.NewEntry<InventoryItemEntity>(newItem);

			NextItemId = NextItemId + 1;

			RefreshInventory();

			return newItem;
			 */

			return null;
		}

		public bool NewEntry( InventoryItemEntity source )
		{
			/*
			m_itemManager.AddEntry<InventoryItemEntity>(NextItemId, source);

            NextItemId = NextItemId + 1;
			//TODO - Figure out the right way to add new items
			//Just updating an item amount doesn't seem to work right
			UpdateItemAmount(source, source.Amount * 2);

			RefreshInventory();
			*/

			if ( BackingObject != null )
			{
				SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
				                                                {
					                                                IMyInventory inventory = (IMyInventory) BackingObject;
					                                                inventory.AddItems( (MyFixedPoint) source.Amount, source.PhysicalContent );
				                                                } );
			}

			return true;
		}

		public bool DeleteEntry( InventoryItemEntity source )
		{
			if ( BackingObject != null )
			{
				SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
				                                                {
					                                                IMyInventory myInventory = (IMyInventory) BackingObject;
					                                                myInventory.RemoveItems( source.ItemId );
				                                                } );
			}

			return true;

			/*
			bool result = m_itemManager.DeleteEntry(source);
			RefreshInventory();
			return result;
			 */
		}

		public void RefreshInventory( )
		{
			try
			{
				/*
				if (BackingObject != null)
				{
					//Update the base entity
					MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)InvokeEntityMethod(BackingObject, InventoryGetObjectBuilderMethod);
					ObjectBuilder = inventory;
				}
				else
				{
					//Update the item manager
					MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)ObjectBuilder;
					List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
					foreach (MyObjectBuilder_InventoryItem item in inventory.Items)
					{
						InventoryItemEntity newItem = new InventoryItemEntity(item);
						newItem.Container = this;
						itemList.Add(newItem);
					}
					m_itemManager.Load(itemList);
				}
				 */
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		[Obsolete]
		public void UpdateItemAmount( InventoryItemEntity item, Decimal newAmount )
		{
			UpdateItemAmount( item, (float)newAmount );
		}

		public void UpdateItemAmount( InventoryItemEntity item, float newAmount )
		{
			if ( BackingObject != null )
			{
				SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
				                                                {
					                                                IMyInventory myInventory = (IMyInventory) BackingObject;
					                                                if ( newAmount == 0 )
					                                                {
						                                                myInventory.RemoveItems( item.ItemId );
					                                                }
					                                                else if ( newAmount > item.Amount )
					                                                {
						                                                myInventory.AddItems( (MyFixedPoint) ( newAmount - item.Amount ), item.PhysicalContent, (int) item.ItemId );
					                                                }
					                                                else if ( newAmount < item.Amount )
					                                                {
						                                                myInventory.RemoveItemsAt( (int) item.ItemId, (MyFixedPoint) ( item.Amount - newAmount ), true );
					                                                }
				                                                } );
			}

			/*
			InventoryDelta delta = new InventoryDelta();
			delta.item = item;
			delta.oldAmount = item.Amount;
			delta.newAmount = newAmount;

			m_itemDeltaQueue.Enqueue(delta);

			Action action = InternalUpdateItemAmount;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			 */
		}

		#region "Internal"

		protected void InternalUpdateItemAmount( )
		{
			try
			{
				/*
				if (m_itemDeltaQueue.Count == 0)
					return;

				InventoryDelta itemDelta = m_itemDeltaQueue.Dequeue();

				float delta = itemDelta.newAmount - itemDelta.oldAmount;

				MyObjectBuilder_PhysicalObject physicalContent = itemDelta.item.ObjectBuilder.PhysicalContent;

				if (delta > 0)
				{
					Object[] parameters = new object[] {
						(MyFixedPoint)delta,
						physicalContent,
						-1
					};

					InvokeEntityMethod(BackingObject, InventoryAddItemAmountMethod, parameters);
				}
				else
				{
					Type[] argTypes = new Type[3];
					argTypes[0] = typeof(MyFixedPoint);
					argTypes[1] = typeof(MyObjectBuilder_PhysicalObject);
					argTypes[2] = typeof(bool);

					Object[] parameters = new object[] {
						(MyFixedPoint)(-delta),
						physicalContent,
						Type.Missing
					};

					InvokeEntityMethod(BackingObject, InventoryRemoveItemAmountMethod, parameters, argTypes);
				}
				 */
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}