namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class MergeBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string MergeBlockNamespace = "Sandbox.Game.Entities.Blocks";
		public static string MergeBlockClass = "MyShipMergeBlock";

		public static string MergeBlockConnectedMergeBlockField = "m_other";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public MergeBlockEntity( CubeGridEntity parent, MyObjectBuilder_MergeBlock definition )
			: base( parent, definition )
		{
		}

		public MergeBlockEntity( CubeGridEntity parent, MyObjectBuilder_MergeBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Merge Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( MergeBlockNamespace, MergeBlockClass );
				return type;
			}
		}

		[IgnoreDataMember]
		[Category( "Merge Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_MergeBlock ObjectBuilder
		{
			get { return (MyObjectBuilder_MergeBlock)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Merge Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public CubeGridEntity AttachedCubeGrid
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return null;

				CubeGridEntity attachedGrid = null;
				try
				{
					Object connectedMergeBlock = GetConnectedBlock( );
					if ( connectedMergeBlock != null )
					{
						Object backingGrid = GetInternalParentCubeGrid( connectedMergeBlock );
						long entityId = BaseEntity.GetEntityId( backingGrid );
						BaseObject matchedObject = GameEntityManager.GetEntity( entityId );
						if ( matchedObject is CubeGridEntity )
						{
							attachedGrid = (CubeGridEntity)matchedObject;
						}
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
				return attachedGrid;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Merge Block" )]
		[ReadOnly( true )]
		public bool IsAttached
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return false;

				return ( GetConnectedBlock( ) != null );
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

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for MergeBlockEntity" );

				result &= Reflection.HasField( type, MergeBlockConnectedMergeBlockField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected Object GetConnectedBlock( )
		{
			Object result = GetEntityFieldValue( ActualObject, MergeBlockConnectedMergeBlockField );
			return result;
		}

		public static Object GetConnectedBlock( object ActualObject )
		{
			Object result = GetEntityFieldValue( ActualObject, MergeBlockConnectedMergeBlockField );
			return result;
		}

		#endregion "Methods"
	}
}