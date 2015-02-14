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
		private readonly GyroNetworkManager _networkManager;

		public const string GyroNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public const string GyroClass = "C33277D56ED00A2772C484D143A9CB42";
			   
		public const string GyroSetOverrideMethod = "248F13F6DFA54470FE81204B119B7DE1";
		public const string GyroSetPowerMethod = "46001A635CDAA4591E15661BA2083B75";
		public const string GyroSetTargetAngularVelocityMethod = "5E85AE802570A4C0028157B436BDF0B5";
			   
		public const string GyroNetworkManagerField = "172E1F246A16951E55BC16D121BE2667";


		public GyroEntity( CubeGridEntity parent, MyObjectBuilder_Gyro definition )
			: base( parent, definition )
		{
		}

		public GyroEntity( CubeGridEntity parent, MyObjectBuilder_Gyro definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			_networkManager = new GyroNetworkManager( this, InternalGetGyroNetworkManager( ) );
		}

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
			_networkManager.BroadcastOverride( );
		}

		protected void InternalUpdateGyroPower( )
		{
			InvokeEntityMethod( ActualObject, GyroSetPowerMethod, new object[ ] { GyroPower } );
			_networkManager.BroadcastPower( );
		}

		protected void InternalUpdateTargetAngularVelocity( )
		{
			InvokeEntityMethod( ActualObject, GyroSetTargetAngularVelocityMethod, new object[ ] { (Vector3)TargetAngularVelocity } );
			_networkManager.BroadcastTargetAngularVelocity( );
		}

	}
}