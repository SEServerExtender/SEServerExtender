namespace SEModAPIInternal.API.Entity
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Common.ObjectBuilders.Serializer;
	using Sandbox.Definitions;
	using Sandbox.ModAPI;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class InventoryItemEntity : BaseObject
	{
		#region "Attributes"

		private InventoryEntity m_parentContainer;

		public static string InventoryItemNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryItemClass = "555069178719BB1B546FB026B906CE00";

		public static string InventoryItemGetObjectBuilderMethod = "B45B0C201826847F0E087D82F9AD3DF1";

		public static string InventoryItemItemIdField = "33FDC4CADA8125F411D1F07103A65358";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public InventoryItemEntity( MyObjectBuilder_InventoryItem definition )
			: base( definition )
		{
			MDefinition = MyDefinitionManager.Static.GetPhysicalItemDefinition( PhysicalContent );
			MDefinitionId = MDefinition.Id;
		}

		public InventoryItemEntity( MyObjectBuilder_InventoryItem definition, Object backingObject )
			: base( definition, backingObject )
		{
			MDefinition = MyDefinitionManager.Static.GetPhysicalItemDefinition( PhysicalContent );
			MDefinitionId = MDefinition.Id;
		}

		public InventoryItemEntity( Object backingObject, InventoryEntity parent )
		{
			MBackingObject = backingObject;
			m_parentContainer = parent;

			Sandbox.ModAPI.Interfaces.IMyInventoryItem item = (Sandbox.ModAPI.Interfaces.IMyInventoryItem)backingObject;
			MyObjectBuilder_InventoryItem newItem = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_InventoryItem>( );
			newItem.Amount = item.Amount;
			newItem.Content = item.Content;
			newItem.ItemId = item.ItemId;
			MObjectBuilder = newItem;

			MDefinition = MyDefinitionManager.Static.GetPhysicalItemDefinition( item.Content.GetId( ) );
			MDefinitionId = MDefinition.Id;
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
						SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
						                                                {
							                                                IMyInventory inventory = (IMyInventory) m_parentContainer.BackingObject;
							                                                item = inventory.GetItemByID( ObjectBuilder.ItemId );
							                                                BackingObject = item;
						                                                } );
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

				MBackingObject = null;
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
				result &= HasMethod( type, InventoryItemGetObjectBuilderMethod );
				result &= HasField( type, InventoryItemItemIdField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
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
				ApplicationLog.BaseLog.Error( ex );
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
}