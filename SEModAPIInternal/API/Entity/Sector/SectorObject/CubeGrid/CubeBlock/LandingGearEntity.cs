using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract]
	public class LandingGearEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool _isLocked;
		private bool _autoLockEnabled;
		private float _brakeForce;

		public static string LandingGearNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string LandingGearClass = "5C73AAF1736F3AA9956574C6D9A2EEBE";

		//public static string LandingGearGetAutoLockMethod = "71F8F86678091875138C01C64F0C2F01";
		//public static string LandingGearGetAutoLockMethod = "3ECDCF46AB6230B4998CE81E37A36F34";

		public static string LandingGearSetAutoLockMethod = "F542ACDC0D61EB46F733A5527CFFBE14";
		public static string LandingGearGetBrakeForceMethod = "get_BreakForce";
		public static string LandingGearSetBrakeForceMethod = "013F45FD594F8A80D5952A7AC22A931E";

		public static string LandingGearIsLockedField = "00F45118D3A7F21253C28F4B11D1F70E";
		public static string LandingGearNetManagerField = "4D9CE737B011256C0232620C5234AAD4";
		public static string LandingGearGetAutoLockField = "B7C2D3F7EF52B638640C0DDB419A1DB4";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public LandingGearEntity( CubeGridEntity parent, MyObjectBuilder_LandingGear definition )
			: base( parent, definition )
		{
			_isLocked = definition.IsLocked;
			_autoLockEnabled = definition.AutoLock;
			_brakeForce = definition.BrakeForce;
		}

		public LandingGearEntity( CubeGridEntity parent, MyObjectBuilder_LandingGear definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			_isLocked = definition.IsLocked;
			_autoLockEnabled = definition.AutoLock;
			_brakeForce = definition.BrakeForce;

			LandingGearNetManager = new LandingGearNetworkManager( this, GetLandingGearNetManager( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Landing Gear" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( LandingGearNamespace, LandingGearClass );
				return type;
			}
		}

		[IgnoreDataMember]
		[Category( "Landing Gear" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_LandingGear ObjectBuilder
		{
			get { return (MyObjectBuilder_LandingGear)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Landing Gear" )]
		public bool IsLocked
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.IsLocked;

				return GetIsLocked( );
			}
			set
			{
				if ( ObjectBuilder.IsLocked == value ) return;
				ObjectBuilder.IsLocked = value;
				Changed = true;

				_isLocked = value;

				if ( BackingObject != null && ActualObject != null )
				{
					Action action = InternalUpdateIsLocked;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Landing Gear" )]
		public bool AutoLock
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.AutoLock;

				return GetAutoLockEnabled( );
			}
			set
			{
				if ( ObjectBuilder.AutoLock == value ) return;
				ObjectBuilder.AutoLock = value;
				Changed = true;

				_autoLockEnabled = value;

				if ( BackingObject != null && ActualObject != null )
				{
					Action action = InternalUpdateAutoLockEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Landing Gear" )]
		public float BrakeForce
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.BrakeForce;

				return GetBrakeForce( );
			}
			set
			{
				if ( ObjectBuilder.BrakeForce == value )
					return;
				ObjectBuilder.BrakeForce = value;
				Changed = true;

				_brakeForce = value;

				if ( BackingObject != null && ActualObject != null )
				{
					Action action = InternalUpdateBrakeForce;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="LandingGearNetworkManager" /> associated with this <see cref="LandingGearEntity" />
		/// </summary>
		[IgnoreDataMember]
		internal LandingGearNetworkManager LandingGearNetManager { get; private set; }

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( LandingGearNamespace, LandingGearClass );
				if ( type == null )
					throw new TypeLoadException( "Could not find internal type for LandingGearEntity" );

				//result &= HasMethod(type, LandingGearGetAutoLockMethod);
				result &= HasMethod( type, LandingGearSetAutoLockMethod );
				result &= HasMethod( type, LandingGearGetBrakeForceMethod );
				result &= HasMethod( type, LandingGearSetBrakeForceMethod );

				result &= HasField( type, LandingGearGetAutoLockField );
				result &= HasField( type, LandingGearIsLockedField );
				result &= HasField( type, LandingGearNetManagerField );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		protected Object GetLandingGearNetManager( )
		{
			Object result = GetEntityFieldValue( ActualObject, LandingGearNetManagerField );
			return result;
		}

		protected bool GetIsLocked( )
		{
			try
			{
				bool result = (bool)GetEntityFieldValue( ActualObject, LandingGearIsLockedField );
				return result;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return _isLocked;
			}
		}

		protected bool GetAutoLockEnabled( )
		{
			try
			{
				//bool result = (bool)InvokeEntityMethod(ActualObject, LandingGearGetAutoLockMethod);
				bool result = (bool)GetEntityFieldValue( ActualObject, LandingGearGetAutoLockField );
				return result;
			}
			catch ( InvalidCastException ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return _autoLockEnabled;
			}
		}

		protected float GetBrakeForce( )
		{
			try
			{
				float result = (float)InvokeEntityMethod( ActualObject, LandingGearGetBrakeForceMethod );
				return result;
			}
			catch ( InvalidCastException ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return _brakeForce;
			}
		}

		protected void InternalUpdateIsLocked( )
		{
			SetEntityFieldValue( ActualObject, LandingGearIsLockedField, _isLocked );

			LandingGearNetManager.BroadcastIsLocked( );
		}

		protected void InternalUpdateAutoLockEnabled( )
		{
			InvokeEntityMethod( ActualObject, LandingGearSetAutoLockMethod, new object[ ] { _autoLockEnabled } );

			LandingGearNetManager.BroadcastAutoLock( );
		}

		protected void InternalUpdateBrakeForce( )
		{
			InvokeEntityMethod( ActualObject, LandingGearSetBrakeForceMethod, new object[ ] { _brakeForce } );

			LandingGearNetManager.BroadcastBrakeForce( );
		}

		#endregion "Methods"
	}
}