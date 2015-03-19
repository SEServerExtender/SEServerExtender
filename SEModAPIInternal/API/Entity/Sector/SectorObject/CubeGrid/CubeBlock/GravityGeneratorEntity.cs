using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "GravityGeneratorEntityProxy" )]
	public class GravityGeneratorEntity : GravityBaseEntity
	{
		#region "Attributes"

		public static string GravityGeneratorNamespace = "";
		public static string GravityGeneratorClass = "Sandbox.Game.Entities.MyGravityGenerator";

		public static string GravityGeneratorSetFieldSizeMethod = "set_FieldSize";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public GravityGeneratorEntity( CubeGridEntity parent, MyObjectBuilder_GravityGenerator definition )
			: base( parent, definition )
		{
		}

		public GravityGeneratorEntity( CubeGridEntity parent, MyObjectBuilder_GravityGenerator definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Gravity Generator" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_GravityGenerator ObjectBuilder
		{
			get
			{
				MyObjectBuilder_GravityGenerator gravity = (MyObjectBuilder_GravityGenerator)base.ObjectBuilder;

				return gravity;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Gravity Generator" )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public SerializableVector3 FieldSize
		{
			get { return ObjectBuilder.FieldSize; }
			set
			{
				if ( ObjectBuilder.FieldSize.Equals( value ) ) return;
				ObjectBuilder.FieldSize = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateFieldSize;
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
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GravityGeneratorNamespace, GravityGeneratorClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GravityGeneratorEntity" );
				result &= HasMethod( type, GravityGeneratorSetFieldSizeMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		protected void InternalUpdateFieldSize( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, GravityGeneratorSetFieldSizeMethod, new object[ ] { (Vector3)FieldSize } );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}