namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.TypeConverters;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;
	using VRageMath;

	[DataContract( Name = "GyroEntityProxy" )]
	public class GyroEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private GyroNetworkManager m_networkManager;

		public static string GyroNamespace = "Sandbox.Game.Entities";
		public static string GyroClass = "MyGyro";

		public static string GyroSetOverrideMethod = "set_GyroOverride";
		public static string GyroSetPowerMethod = "set_GyroPower";
		public static string GyroSetTargetAngularVelocityMethod = "SetGyroTorque";

		public static string GyroNetworkManagerField = "SyncObject";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public GyroEntity( CubeGridEntity parent, MyObjectBuilder_Gyro definition )
			: base( parent, definition )
		{
		}

		public GyroEntity( CubeGridEntity parent, MyObjectBuilder_Gyro definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_networkManager = new GyroNetworkManager( this, InternalGetGyroNetworkManager( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Gyro" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Gyro ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Gyro gyro = (MyObjectBuilder_Gyro)base.ObjectBuilder;

				return gyro;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Gyro" )]
		[Browsable( true )]
		[ReadOnly( false )]
		public bool GyroOverride
		{
			get { return ObjectBuilder.GyroOverride; }
			set
			{
				if ( ObjectBuilder.GyroOverride == value ) return;
				ObjectBuilder.GyroOverride = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateGyroOverride );
				}
			}
		}

		[DataMember]
		[Category( "Gyro" )]
		[Browsable( true )]
		[ReadOnly( false )]
		public float GyroPower
		{
			get { return ObjectBuilder.GyroPower; }
			set
			{
				if ( ObjectBuilder.GyroPower == value ) return;
				ObjectBuilder.GyroPower = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateGyroPower );
				}
			}
		}

		[DataMember]
		[Category( "Gyro" )]
		[Browsable( true )]
		[ReadOnly( false )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper TargetAngularVelocity
		{
			get { return (Vector3Wrapper)ObjectBuilder.TargetAngularVelocity; }
			set
			{
				if ( (Vector3)ObjectBuilder.TargetAngularVelocity == (Vector3)value ) return;
				ObjectBuilder.TargetAngularVelocity = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateTargetAngularVelocity );
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GyroNamespace, GyroClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GyroEntity" );
				result &= Reflection.HasMethod( type, GyroSetOverrideMethod );
				result &= Reflection.HasMethod( type, GyroSetPowerMethod );
				result &= Reflection.HasMethod( type, GyroSetTargetAngularVelocityMethod );
				result &= Reflection.HasField( type, GyroNetworkManagerField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error(  ex );
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetGyroNetworkManager( )
		{
			try
			{
				FieldInfo field = GetEntityField( ActualObject, GyroNetworkManagerField );
				Object result = field.GetValue( ActualObject );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected void InternalUpdateGyroOverride( )
		{
			InvokeEntityMethod( ActualObject, GyroSetOverrideMethod, new object[ ] { GyroOverride } );
			m_networkManager.BroadcastOverride( );
		}

		protected void InternalUpdateGyroPower( )
		{
			InvokeEntityMethod( ActualObject, GyroSetPowerMethod, new object[ ] { GyroPower } );
			m_networkManager.BroadcastPower( );
		}

		protected void InternalUpdateTargetAngularVelocity( )
		{
			InvokeEntityMethod( ActualObject, GyroSetTargetAngularVelocityMethod, new object[ ] { (Vector3)TargetAngularVelocity } );
			m_networkManager.BroadcastTargetAngularVelocity( );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}