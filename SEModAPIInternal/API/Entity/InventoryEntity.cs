using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Serializer;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using VRage;

//using Sandbox.ModAPI.Interfaces;

namespace SEModAPIInternal.API.Entity
{
	public struct InventoryDelta
	{
		public InventoryItemEntity item;
		public float oldAmount;
		public float newAmount;
	}

	// IMyInventory
	[DataContract( Name = "InventoryEntityProxy" )]
	[KnownType( typeof( InventoryItemEntity ) )]
	public class InventoryEntity : BaseObject
	{
		#region "Attributes"

		private InventoryItemManager m_itemManager;
		private Queue<InventoryDelta> m_itemDeltaQueue;

		public static string InventoryNamespace = "";
		public static string InventoryClass = "=mazFMfE1HhxL19l3plK4hvEmGA=";

		public static string InventoryCalculateMassVolumeMethod = "RefreshVolumeAndMass";
		public static string InventoryGetTotalVolumeMethod = "get_CurrentVolume";
		public static string InventoryGetTotalMassMethod = "get_CurrentMass";
		public static string InventorySetFromObjectBuilderMethod = "Init";
		public static string InventoryGetObjectBuilderMethod = "GetObjectBuilder";
		public static string InventoryCleanUpMethod = "Clear";
		public static string InventoryGetItemListMethod = "GetItems";
		public static string InventoryAddItemAmountMethod = "AddItems";
		public static string InventoryRemoveItemAmountMethod = "RemoveItemsOfType";

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
			m_itemDeltaQueue = new Queue<InventoryDelta>( );
		}

		public InventoryEntity( MyObjectBuilder_Inventory definition, Object backingObject )
			: base( definition, backingObject )
		{
			//m_itemManager = new InventoryItemManager(this, backingObject, InventoryGetItemListMethod);
			//m_itemManager.Refresh();
			m_itemDeltaQueue = new Queue<InventoryDelta>( );
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
					SandboxGameAssemblyWrapper.Instance.GameAction( new Action( delegate( )
					{
						IMyInventory inventory = (IMyInventory)BackingObject;
						result = (uint)inventory.GetItems( ).Count;
					} ) );
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

					SandboxGameAssemblyWrapper.Instance.GameAction( new Action( delegate( )
						{
							if ( BackingObject != null )
							{
								IMyInventory myInventory = (IMyInventory)BackingObject;
								foreach ( Sandbox.ModAPI.Interfaces.IMyInventoryItem item in myInventory.GetItems( ) )
								{
									InventoryItemEntity newItem = new InventoryItemEntity( item, this );
									newList.Add( newItem );
								}
							}
						} ) );

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
					throw new Exception( "Could not find internal type for InventoryEntity" );
				bool result = true;
				result &= BaseObject.HasMethod( type, InventoryCalculateMassVolumeMethod );
				result &= BaseObject.HasMethod( type, InventoryGetTotalVolumeMethod );
				result &= BaseObject.HasMethod( type, InventoryGetTotalMassMethod );
				result &= BaseObject.HasMethod( type, InventorySetFromObjectBuilderMethod );
				result &= BaseObject.HasMethod( type, InventoryGetObjectBuilderMethod );
				result &= BaseObject.HasMethod( type, InventoryCleanUpMethod );
				result &= BaseObject.HasMethod( type, InventoryGetItemListMethod );
				result &= BaseObject.HasMethod( type, InventoryAddItemAmountMethod );

				Type[ ] argTypes = new Type[ 3 ];
				argTypes[ 0 ] = typeof( MyFixedPoint );
				argTypes[ 1 ] = typeof( MyObjectBuilder_PhysicalObject );
				argTypes[ 2 ] = typeof( bool );
				result &= BaseObject.HasMethod( type, InventoryRemoveItemAmountMethod, argTypes );

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
				SandboxGameAssemblyWrapper.Instance.GameAction( new Action( delegate( )
					{
						IMyInventory inventory = (IMyInventory)BackingObject;
						inventory.AddItems( (MyFixedPoint)source.Amount, source.PhysicalContent );
					} ) );
			}

			return true;
		}

		public bool DeleteEntry( InventoryItemEntity source )
		{
			if ( BackingObject != null )
			{
				SandboxGameAssemblyWrapper.Instance.GameAction( new Action( delegate( )
				{
					IMyInventory myInventory = (IMyInventory)BackingObject;
					myInventory.RemoveItems( source.ItemId );
				} ) );
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
				SandboxGameAssemblyWrapper.Instance.GameAction( new Action( delegate( )
				{
					IMyInventory myInventory = (IMyInventory)BackingObject;
					if ( newAmount == 0 )
						myInventory.RemoveItems( item.ItemId );
					else if ( newAmount > item.Amount )
						myInventory.AddItems( (MyFixedPoint)( newAmount - item.Amount ), item.PhysicalContent, (int)item.ItemId );
					else if ( newAmount < item.Amount )
						myInventory.RemoveItemsAt( (int)item.ItemId, (MyFixedPoint)( item.Amount - newAmount ), true );
				} ) );
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

	// IMyInventoryItem
	[DataContract( Name = "InventoryItemEntityProxy" )]
	public class InventoryItemEntity : BaseObject
	{
		#region "Attributes"

		private InventoryEntity m_parentContainer;

		public static string InventoryItemNamespace = "";
		public static string InventoryItemClass = "=Jm6LVWsHj1NFGuqtqTheDghSPX=";

		public static string InventoryItemGetObjectBuilderMethod = "GetObjectBuilder";

		public static string InventoryItemItemIdField = "=4E6roGfagvQcqDT8xP531wSsud=";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public InventoryItemEntity( MyObjectBuilder_InventoryItem definition )
			: base( definition )
		{
			m_definition = MyDefinitionManager.Static.GetPhysicalItemDefinition( PhysicalContent );
			m_definitionId = m_definition.Id;
		}

		public InventoryItemEntity( MyObjectBuilder_InventoryItem definition, Object backingObject )
			: base( definition, backingObject )
		{
			m_definition = MyDefinitionManager.Static.GetPhysicalItemDefinition( PhysicalContent );
			m_definitionId = m_definition.Id;
		}

		public InventoryItemEntity( Object backingObject, InventoryEntity parent )
		{
			m_backingObject = backingObject;
			m_parentContainer = parent;

			Sandbox.ModAPI.Interfaces.IMyInventoryItem item = (Sandbox.ModAPI.Interfaces.IMyInventoryItem)backingObject;
			MyObjectBuilder_InventoryItem newItem = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_InventoryItem>( );
			newItem.Amount = item.Amount;
			newItem.Content = item.Content;
			newItem.ItemId = item.ItemId;
			m_objectBuilder = newItem;

			m_definition = MyDefinitionManager.Static.GetPhysicalItemDefinition( item.Content.GetId( ) );
			m_definitionId = m_definition.Id;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		internal Sandbox.ModAPI.Interfaces.IMyInventoryItem InventoryInterface
		{
			get
			{
				Sandbox.ModAPI.Interfaces.IMyInventoryItem item = null;
				if ( BackingObject == null )
				{
					if ( m_parentContainer != null )
					{
						SandboxGameAssemblyWrapper.Instance.GameAction( new Action( delegate( )
						{
							IMyInventory inventory = (IMyInventory)m_parentContainer.BackingObject;
							item = inventory.GetItemByID( ObjectBuilder.ItemId );
							BackingObject = item;
						} ) );
					}
				}
				else
				{
					item = (Sandbox.ModAPI.Interfaces.IMyInventoryItem)BackingObject;
				}

				return item;
			}
		}

		[IgnoreDataMember]
		[Browsable( false )]
		[ReadOnly( true )]
		internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( InventoryItemNamespace, InventoryItemClass );
				return type;
			}
		}

		[DataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		public override string Name
		{
			get
			{
				MyPhysicalItemDefinition def = Definition;
				if ( def == null )
					return base.Name;

				return def.Id.SubtypeName;
			}
		}

		[IgnoreDataMember]
		[Category( "Container Item" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_InventoryItem ObjectBuilder
		//internal MyObjectBuilder_PhysicalObject ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_InventoryItem)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		new public MyPhysicalItemDefinition Definition
		{
			get
			{
				return (MyPhysicalItemDefinition)base.Definition;
			}
		}

		[IgnoreDataMember]
		[Category( "Container Item" )]
		[Browsable( false )]
		public InventoryEntity Container
		{
			get { return m_parentContainer; }
			set { m_parentContainer = value; }
		}

		[DataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		public uint ItemId
		{
			get
			{
				if ( InventoryInterface != null )
					return InventoryInterface.ItemId;

				return ObjectBuilder.ItemId;
			}
			/*set
			{
				if (ObjectBuilder.ItemId == value) return;
				ObjectBuilder.ItemId = value;
				Changed = true;
			}*/
		}

		[DataMember]
		[Category( "Container Item" )]
		public float Amount
		{
			get
			{
				if ( InventoryInterface != null )
					return (float)InventoryInterface.Amount;

				return (float)ObjectBuilder.Amount;
			}
			set
			{
				if ( Container != null )
					Container.UpdateItemAmount( this, value );

				m_backingObject = null;
				/*
				var baseEntity = ObjectBuilder;
				if ((float)baseEntity.Amount == value) return;

				if(Container != null)
					Container.UpdateItemAmount(this, value);

				baseEntity.Amount = (MyFixedPoint)value;
				Changed = true;
				 */
			}
		}

		[IgnoreDataMember]
		[Category( "Container Item" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public MyObjectBuilder_PhysicalObject PhysicalContent
		{
			get
			{
				if ( InventoryInterface != null )
					return InventoryInterface.Content;

				return ObjectBuilder.PhysicalContent;
			}

			/*set
			{
				if (ObjectBuilder.PhysicalContent == value) return;
				ObjectBuilder.PhysicalContent = value;
				Changed = true;
			}*/
		}

		[IgnoreDataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		public float TotalMass
		{
			get
			{
				if ( InventoryInterface != null )
					return (float)InventoryInterface.Amount * Mass;

				return (float)ObjectBuilder.Amount * Mass;
			}
		}

		[IgnoreDataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		public float TotalVolume
		{
			get
			{
				if ( InventoryInterface != null )
					return (float)InventoryInterface.Amount * Volume;

				return (float)ObjectBuilder.Amount * Volume;
			}
		}

		[DataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		public float Mass
		{
			get
			{
				if ( Definition == null )
					return 0;

				return Definition.Mass;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Container Item" )]
		[ReadOnly( true )]
		public float Volume
		{
			get
			{
				if ( Definition == null )
					return 0;

				return Definition.Volume;
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
					throw new Exception( "Could not find internal type for InventoryItemEntity" );
				bool result = true;
				result &= BaseObject.HasMethod( type, InventoryItemGetObjectBuilderMethod );
				result &= BaseObject.HasField( type, InventoryItemItemIdField );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		public static uint GetInventoryItemId( object item )
		{
			try
			{
				uint result = (uint)GetEntityFieldValue( item, InventoryItemItemIdField );
				return result;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return 0;
			}
		}

		public override void Dispose( )
		{
			Amount = 0;
			base.Dispose( );
		}

		#endregion "Methods"
	}

	//IMyInventoryOwner
	public class InventoryItemManager : BaseObjectManager
	{
		#region "Attributes"

		private InventoryEntity m_parent;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public InventoryItemManager( InventoryEntity parent )
		{
			m_parent = parent;
		}

		public InventoryItemManager( InventoryEntity parent, Object backingSource, string backingSourceMethodName )
			: base( backingSource, backingSourceMethodName, InternalBackingType.List )
		{
			m_parent = parent;
		}

		#endregion "Constructors and Initializers"

		#region "Methods"

		protected override bool IsValidEntity( Object entity )
		{
			try
			{
				if ( entity == null )
					return false;

				return true;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		protected override void LoadDynamic( )
		{
			try
			{
				/*
				List<Object> rawEntities = GetBackingDataList();
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>(GetInternalData());

				//Update the main data mapping
				foreach (Object entity in rawEntities)
				{
					try
					{
						if (!IsValidEntity(entity))
							continue;

						MyObjectBuilder_InventoryItem baseEntity = (MyObjectBuilder_InventoryItem)InventoryItemEntity.InvokeEntityMethod(entity, InventoryItemEntity.InventoryItemGetObjectBuilderMethod);
						if (baseEntity == null)
							continue;

						uint entityItemId = InventoryItemEntity.GetInventoryItemId(entity);
						long itemId = baseEntity.ItemId;

						//If the original data already contains an entry for this, skip creation
						if (internalDataCopy.ContainsKey(itemId))
						{
							InventoryItemEntity matchingItem = (InventoryItemEntity)GetEntry(itemId);
							if (matchingItem == null || matchingItem.IsDisposed)
								continue;

							matchingItem.BackingObject = entity;
							matchingItem.ObjectBuilder = baseEntity;
						}
						else
						{
							InventoryItemEntity newItemEntity = new InventoryItemEntity(baseEntity, entity);
							newItemEntity.Container = m_parent;

							AddEntry(newItemEntity.ItemId, newItemEntity);
						}
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				//Cleanup old entities
				foreach (var entry in internalDataCopy)
				{
					try
					{
						if (!rawEntities.Contains(entry.Value.BackingObject))
							DeleteEntry(entry.Value);
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}
				 */
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}