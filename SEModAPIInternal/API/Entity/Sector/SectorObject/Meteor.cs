namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class Meteor : BaseEntity
	{
		#region "Attributes"

		private InventoryItemEntity m_item;
		private static Type m_internalType;

		public static string MeteorNamespace = "";
		public static string MeteorClass = "Sandbox.Game.Entities.MyMeteor";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public Meteor( MyObjectBuilder_Meteor definition )
			: base( definition )
		{
			m_item = new InventoryItemEntity( definition.Item );
		}

		public Meteor( MyObjectBuilder_Meteor definition, Object backingObject )
			: base( definition, backingObject )
		{
			m_item = new InventoryItemEntity( definition.Item );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Meteor" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new internal static Type InternalType
		{
			get { return m_internalType ?? ( m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( MeteorNamespace, MeteorClass ) ); }
		}

		[DataMember]
		[Category( "Meteor" )]
		[Browsable( true )]
		public override string Name
		{
			get { return ObjectBuilder.Item.PhysicalContent.SubtypeName; }
		}

		[DataMember]
		[Category( "Meteor" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Meteor ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_Meteor)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Meteor" )]
		public float Integrity
		{
			get { return ObjectBuilder.Integrity; }
			set
			{
				if ( ObjectBuilder.Integrity == value ) return;
				ObjectBuilder.Integrity = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateMeteorIntegrity );
				}
			}
		}

		[DataMember]
		[Category( "Meteor" )]
		[Browsable( false )]
		public InventoryItemEntity Item
		{
			get { return m_item; }
			set
			{
				if ( m_item == value ) return;
				m_item = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateMeteorItem );
				}
			}
		}

		#endregion "Properties"

		#region "Methods"

		protected void InternalUpdateMeteorIntegrity( )
		{
			try
			{
				//TODO - Add methods to set integrity and item of the meteor
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateMeteorItem( )
		{
			try
			{
				//TODO - Add methods to set integrity and item of the meteor
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}