using VRage.Game;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Game.EntityComponents;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class ReactorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;
		private float m_maxPowerOutput;
		private DateTime m_lastInventoryRefresh;

		public static string ReactorNamespace = "Sandbox.Game.Entities";
		public static string ReactorClass = "MyReactor";

		public static string ReactorGetInventoryMethod = "GetInventory";
		public static string ReactorSetMaxPowerOutputMethod = "set_MaxPowerOutput";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ReactorEntity( CubeGridEntity parent, MyObjectBuilder_Reactor definition )
			: base( parent, definition )
		{
			m_Inventory = new InventoryEntity( definition.Inventory );

			m_lastInventoryRefresh = DateTime.Now;
		}

		public ReactorEntity( CubeGridEntity parent, MyObjectBuilder_Reactor definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_Inventory = new InventoryEntity( definition.Inventory, InternalGetReactorInventory( ) );

			m_lastInventoryRefresh = DateTime.Now;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Reactor" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Reactor ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Reactor reactor = (MyObjectBuilder_Reactor)base.ObjectBuilder;

				TimeSpan timeSinceLastInventoryRefresh = DateTime.Now - m_lastInventoryRefresh;
				if ( timeSinceLastInventoryRefresh.TotalSeconds > 10 )
				{
					m_lastInventoryRefresh = DateTime.Now;

					//Make sure the inventory is up-to-date
					Inventory.RefreshInventory( );
					reactor.Inventory = Inventory.ObjectBuilder;
				}

				return reactor;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Reactor" )]
		[Browsable( false )]
		public InventoryEntity Inventory
		{
			get
			{
				return m_Inventory;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Reactor" )]
		public float Fuel
		{
			get
			{
				float fuelMass = 0;
				foreach ( MyObjectBuilder_InventoryItem item in ObjectBuilder.Inventory.Items )
				{
					fuelMass += (float)item.Amount;
				}
				return fuelMass;
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
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ReactorNamespace, ReactorClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for ReactorEntity" );
				result &= Reflection.HasMethod( type, ReactorGetInventoryMethod );
				result &= Reflection.HasMethod( type, ReactorSetMaxPowerOutputMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetReactorInventory( )
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject( );
				Object inventory = InvokeEntityMethod( actualObject, ReactorGetInventoryMethod, new object[ ] { 0 } );

				return inventory;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected void InternalUpdateMaxPowerOutput( )
		{
			InvokeEntityMethod( ActualObject, ReactorSetMaxPowerOutputMethod, new object[ ] { m_maxPowerOutput } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}