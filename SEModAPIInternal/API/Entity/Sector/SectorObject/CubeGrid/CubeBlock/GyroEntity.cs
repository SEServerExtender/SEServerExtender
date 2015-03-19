using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "GyroEntityProxy" )]
	public class GyroEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private GyroNetworkManager m_networkManager;

		public static string GyroNamespace = "";
		public static string GyroClass = "=Naf3gC9AWnHHOtllqy1OLLb5BL=";

		public static string GyroSetOverrideMethod = "set_GyroOverride";
		public static string GyroSetPowerMethod = "set_GyroPower";
		public static string GyroSetTargetAngularVelocityMethod = "SetGyroTorque";

		public static string GyroNetworkManagerField = "=gNAvpmtOwXYmmcG9ynRDGRitUw=";

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
					Action action = InternalUpdateGyroOverride;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
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
					Action action = InternalUpdateGyroPower;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
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
					Action action = InternalUpdateTargetAngularVelocity;
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GyroNamespace, GyroClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GyroEntity" );
				result &= HasMethod( type, GyroSetOverrideMethod );
				result &= HasMethod( type, GyroSetPowerMethod );
				result &= HasMethod( type, GyroSetTargetAngularVelocityMethod );
				result &= HasField( type, GyroNetworkManagerField );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
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
				LogManager.ErrorLog.WriteLine( ex );
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

	public class GyroNetworkManager
	{
		#region "Attributes"

		private GyroEntity m_parent;
		private Object m_backingObject;

		public static string GyroNetworkManagerNamespace = "";
		public static string GyroNetworkManagerClass = "=oxJceWENGF516EExThNDB8OIOS=";

		//Packet ID 7587
		public static string GyroNetworkManagerBroadcastOverrideMethod = "SendGyroOverrideRequest";

		//Packet ID 7586
		public static string GyroNetworkManagerBroadcastPowerMethod = "SendChangeGyroPowerRequest";

		//Packet ID 7588
		public static string GyroNetworkManagerBroadcastTargetAngularVelocityMethod = "SendGyroTorqueRequest";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public GyroNetworkManager( GyroEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Initializers"



		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GyroNetworkManagerNamespace, GyroNetworkManagerClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for GyroNetworkManager" );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastOverrideMethod );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastPowerMethod );
				result &= BaseObject.HasMethod( type, GyroNetworkManagerBroadcastTargetAngularVelocityMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public void BroadcastOverride( )
		{
			Action action = InternalBroadcastOverride;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void BroadcastPower( )
		{
			Action action = InternalBroadcastPower;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void BroadcastTargetAngularVelocity( )
		{
			Action action = InternalBroadcastTargetAngularVelocity;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#region "Internal"

		protected void InternalBroadcastOverride( )
		{
			BaseObject.InvokeEntityMethod( m_backingObject, GyroNetworkManagerBroadcastOverrideMethod, new object[ ] { m_parent.GyroOverride } );
		}

		protected void InternalBroadcastPower( )
		{
			BaseObject.InvokeEntityMethod( m_backingObject, GyroNetworkManagerBroadcastPowerMethod, new object[ ] { m_parent.GyroPower } );
		}

		protected void InternalBroadcastTargetAngularVelocity( )
		{
			Vector3 newTarget = (Vector3)m_parent.TargetAngularVelocity;
			BaseObject.InvokeEntityMethod( m_backingObject, GyroNetworkManagerBroadcastTargetAngularVelocityMethod, new object[ ] { newTarget } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}