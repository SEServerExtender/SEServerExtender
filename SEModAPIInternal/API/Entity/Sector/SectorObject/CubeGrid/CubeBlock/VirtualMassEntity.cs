namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class VirtualMassEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string VirtualMassNamespace = "";
		public static string VirtualMassClass = "Sandbox.Game.Entities.MyVirtualMass";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public VirtualMassEntity( CubeGridEntity parent, MyObjectBuilder_VirtualMass definition )
			: base( parent, definition )
		{
		}

		public VirtualMassEntity( CubeGridEntity parent, MyObjectBuilder_VirtualMass definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Virtual Mass" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_VirtualMass ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_VirtualMass)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Virtual Mass" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( VirtualMassNamespace, VirtualMassClass );
				return type;
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
					throw new Exception( "Could not find internal type for VirtualMassEntity" );

				//result &= HasMethod(type, AntennaGetRadioManagerMethod);

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#endregion "Methods"
	}
}