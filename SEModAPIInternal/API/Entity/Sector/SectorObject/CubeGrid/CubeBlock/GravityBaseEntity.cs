namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class GravityBaseEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private float m_acceleration;

		public static string GravityBaseNamespace = "";
		public static string GravityBaseClass = "=Gzts3NqimVWYP0iJIkraScr0Ks=";

		public static string GravityBaseSetAccelerationMethod = "set_GravityAcceleration";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public GravityBaseEntity( CubeGridEntity parent, MyObjectBuilder_FunctionalBlock definition )
			: base( parent, definition )
		{
			m_acceleration = 9.81f;
		}

		public GravityBaseEntity( CubeGridEntity parent, MyObjectBuilder_FunctionalBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_acceleration = 9.81f;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Gravity Base" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_FunctionalBlock ObjectBuilder
		{
			get
			{
				MyObjectBuilder_FunctionalBlock gravity = (MyObjectBuilder_FunctionalBlock)base.ObjectBuilder;

				return gravity;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Gravity Base" )]
		public float GravityAcceleration
		{
			get { return m_acceleration; }
			set
			{
				if ( m_acceleration == value ) return;
				m_acceleration = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateGravityAcceleration;
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GravityBaseNamespace, GravityBaseClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GravityBaseEntity" );
				result &= HasMethod( type, GravityBaseSetAccelerationMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected void InternalUpdateGravityAcceleration( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, GravityBaseSetAccelerationMethod, new object[ ] { GravityAcceleration } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}