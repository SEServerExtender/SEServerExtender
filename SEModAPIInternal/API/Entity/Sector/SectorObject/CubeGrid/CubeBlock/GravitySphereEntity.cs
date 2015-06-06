namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class GravitySphereEntity : GravityBaseEntity
	{
		#region "Attributes"

		public static string GravitySphereNamespace = "Sandbox.Game.Entities";
		public static string GravitySphereClass = "MyGravityGeneratorSphere";

		public static string GravitySphereSetFieldRadiusMethod = "set_Radius";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public GravitySphereEntity( CubeGridEntity parent, MyObjectBuilder_GravityGeneratorSphere definition )
			: base( parent, definition )
		{
		}

		public GravitySphereEntity( CubeGridEntity parent, MyObjectBuilder_GravityGeneratorSphere definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Gravity Generator Sphere" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_GravityGeneratorSphere ObjectBuilder
		{
			get
			{
				MyObjectBuilder_GravityGeneratorSphere gravity = (MyObjectBuilder_GravityGeneratorSphere)base.ObjectBuilder;

				return gravity;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Gravity Generator Sphere" )]
		public float FieldRadius
		{
			get { return ObjectBuilder.Radius; }
			set
			{
				if ( ObjectBuilder.Radius.Equals( value ) ) return;
				ObjectBuilder.Radius = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateFieldRadius );
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GravitySphereNamespace, GravitySphereClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GravitySphereEntity" );
				result &= Reflection.HasMethod( type, GravitySphereSetFieldRadiusMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected void InternalUpdateFieldRadius( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, GravitySphereSetFieldRadiusMethod, new object[ ] { FieldRadius } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}