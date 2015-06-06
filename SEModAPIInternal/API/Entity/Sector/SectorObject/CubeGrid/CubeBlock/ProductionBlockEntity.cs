namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class ProductionBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_inputInventory;
		private InventoryEntity m_outputInventory;

		public static string ProductionBlockNamespace = "Sandbox.Game.Entities.Cube";
		public static string ProductionBlockClass = "MyProductionBlock";

		public static string ProductionBlockGetInputInventoryMethod = "GetInventory";
		public static string ProductionBlockGetOutputInventoryMethod = "GetInventory";
		public static string ProductionBlockGetQueueMethod = "get_Queue";
		public static string ProductionBlockSetQueueMethod = "SwapQueue";
		public static string ProductionBlockTriggerQueueChangedCallbackMethod = "UpdatePower";

		public static string ProductionBlockQueueField = "m_queue";

		public static string ProductionBlockQueueItemStruct = "QueueItem";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public ProductionBlockEntity( CubeGridEntity parent, MyObjectBuilder_ProductionBlock definition )
			: base( parent, definition )
		{
			m_inputInventory = new InventoryEntity( definition.InputInventory );
			m_outputInventory = new InventoryEntity( definition.OutputInventory );
		}

		public ProductionBlockEntity( CubeGridEntity parent, MyObjectBuilder_ProductionBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_inputInventory = new InventoryEntity( definition.InputInventory, InternalGetInputInventory( ) );
			m_outputInventory = new InventoryEntity( definition.OutputInventory, InternalGetOutputInventory( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Production Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_ProductionBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_ProductionBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Production Block" )]
		[Browsable( false )]
		public InventoryEntity InputInventory
		{
			get
			{
				return m_inputInventory;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Production Block" )]
		[Browsable( false )]
		public InventoryEntity OutputInventory
		{
			get
			{
				return m_outputInventory;
			}
			private set
			{
				//Do nothing!
			}
		}

		public object QueueTest
		{
			get
			{
				object raw = InvokeEntityMethod( ActualObject, ProductionBlockGetQueueMethod );
				return raw;
			}
		}

		[IgnoreDataMember]
		[Category( "Production Block" )]
		[Browsable( false )]
		public int QueueCount
		{
			get
			{
				object raw = InvokeEntityMethod( ActualObject, ProductionBlockGetQueueMethod );
				if ( raw == null )
					return 0;

				return ( (IList)raw ).Count;
			}
		}

		[IgnoreDataMember]
		[Category( "Production Block" )]
		[Browsable( false )]
		public List<ProductionQueueItem> Queue
		{
			get
			{
				List<ProductionQueueItem> list = new List<ProductionQueueItem>( );
				if ( ObjectBuilder.Queue != null )
				{
					foreach ( MyObjectBuilder_ProductionBlock.QueueItem item in ObjectBuilder.Queue )
						list.Add( item );
				}

				return list;

				//return (List<ProductionQueueItem>)InvokeEntityMethod(ActualObject, ProductionBlockGetQueueMethod);
			}
			set
			{
				MyObjectBuilder_ProductionBlock.QueueItem[ ] newQueue = new MyObjectBuilder_ProductionBlock.QueueItem[ value.Count ];
				for ( int i = 0; i < value.Count; i++ )
				{
					newQueue[ i ] = value[ i ];
				}
				ObjectBuilder.Queue = newQueue;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateQueue );
				}
			}
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ProductionBlockNamespace, ProductionBlockClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for ProductionBlockEntity" );
				bool result = true;
				result &= Reflection.HasMethod( type, ProductionBlockGetInputInventoryMethod );
				result &= Reflection.HasMethod( type, ProductionBlockGetOutputInventoryMethod );
				result &= Reflection.HasMethod( type, ProductionBlockGetQueueMethod );
				result &= Reflection.HasMethod( type, ProductionBlockSetQueueMethod );
				result &= Reflection.HasMethod( type, ProductionBlockTriggerQueueChangedCallbackMethod );
				result &= Reflection.HasField( type, ProductionBlockQueueField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void ClearQueue( )
		{
			MySandboxGame.Static.Invoke( InternalClearQueue );
		}

		#region "Internal"

		protected Object InternalGetInputInventory( )
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject( );
				Object inventory = InvokeEntityMethod( actualObject, ProductionBlockGetInputInventoryMethod, new object[ ] { 0 } );

				return inventory;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected Object InternalGetOutputInventory( )
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject( );
				Object inventory = InvokeEntityMethod( actualObject, ProductionBlockGetOutputInventoryMethod, new object[ ] { 1 } );

				return inventory;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected void InternalClearQueue( )
		{
			try
			{
				FieldInfo field = GetEntityField( ActualObject, ProductionBlockQueueField );
				Object result = field.GetValue( ActualObject );
				InvokeEntityMethod( result, "Clear" );
				InvokeEntityMethod( ActualObject, ProductionBlockTriggerQueueChangedCallbackMethod );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateQueue( )
		{
			List<Object> newQueue = new List<object>( );

			//TODO - Copy the API queue into the new queue list

			InvokeEntityMethod( ActualObject, ProductionBlockSetQueueMethod, new object[ ] { newQueue } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}