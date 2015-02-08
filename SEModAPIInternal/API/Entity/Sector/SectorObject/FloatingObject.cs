using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract]
	public class FloatingObject : BaseEntity
	{
		#region "Attributes"

		private static Type _internalType;
		private InventoryItemEntity _item;

		public static string FloatingObjectNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectClass = "60663B6C2E735862064C925471BD4138";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public FloatingObject( MyObjectBuilder_FloatingObject definition )
			: base( definition )
		{
			_item = new InventoryItemEntity( definition.Item );
		}

		public FloatingObject( MyObjectBuilder_FloatingObject definition, Object backingObject )
			: base( definition, backingObject )
		{
			_item = new InventoryItemEntity( definition.Item );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Floating Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new internal static Type InternalType
		{
			get { return _internalType ?? ( _internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( FloatingObjectNamespace, FloatingObjectClass ) ); }
		}

		[DataMember]
		[Category( "Floating Object" )]
		[Browsable( true )]
		public override string Name
		{
			get { return ObjectBuilder.Item.PhysicalContent.SubtypeName; }
		}

		[DataMember]
		[Category( "Floating Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_FloatingObject ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_FloatingObject)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Floating Object" )]
		[Browsable( false )]
		public InventoryItemEntity Item
		{
			get { return _item; }
			set
			{
				if ( _item == value ) return;
				_item = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateItem;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
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
					throw new TypeLoadException( "Could not find internal type for FloatingObject" );
				const bool result = true;

				return result;
			}
			catch ( TypeLoadException ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		public override void Dispose( )
		{
			FloatingObjectManager.Instance.RemoveFloatingObject( this );

			MIsDisposed = true;
		}

		protected void InternalUpdateItem( )
		{
			try
			{
				//TODO - Add methods to set item of the floating object
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}