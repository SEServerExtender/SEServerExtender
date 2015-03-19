using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "RotorEntityProxy" )]
	public class RotorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string RotorNamespace = "";
		public static string RotorClass = "=6Wy9Sq1bSnd1NU3pZEtIJjR4rJ=";

		public static string RotorTopBlockEntityIdField = "=3VfRAqLnpYlKKw2kDSOSyVqUsR=";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public RotorEntity( CubeGridEntity parent, MyObjectBuilder_MotorStator definition )
			: base( parent, definition )
		{
		}

		public RotorEntity( CubeGridEntity parent, MyObjectBuilder_MotorStator definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Rotor" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( RotorNamespace, RotorClass );
				return type;
			}
		}

		[IgnoreDataMember]
		[Category( "Rotor" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_MotorStator ObjectBuilder
		{
			get { return (MyObjectBuilder_MotorStator)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Rotor" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public CubeBlockEntity TopBlock
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return null;

				long topBlockEntityId = GetTopBlockEntityId( );
				if ( topBlockEntityId == 0 )
					return null;
				BaseObject baseObject = GameEntityManager.GetEntity( topBlockEntityId );
				if ( !( baseObject is CubeBlockEntity ) )
					return null;
				CubeBlockEntity block = (CubeBlockEntity)baseObject;
				return block;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Rotor" )]
		[ReadOnly( true )]
		public long TopBlockId
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.RotorEntityId;

				return GetTopBlockEntityId( );
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
					throw new Exception( "Could not find internal type for RotorEntity" );

				result &= HasField( type, RotorTopBlockEntityIdField );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		protected long GetTopBlockEntityId( )
		{
			Object rawResult = GetEntityFieldValue( ActualObject, RotorTopBlockEntityIdField );
			if ( rawResult == null )
				return 0;
			long result = (long)rawResult;
			return result;
		}

		#endregion "Methods"
	}
}