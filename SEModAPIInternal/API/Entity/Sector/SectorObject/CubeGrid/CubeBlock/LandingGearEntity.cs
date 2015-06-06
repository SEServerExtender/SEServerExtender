namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract( Name = "LandingGearEntityProxy" )]
	public class LandingGearEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private LandingGearNetworkManager m_landingNetManager;

		private bool m_isLocked;
		private bool m_autoLockEnabled;
		private float m_brakeForce;

		public static string LandingGearNamespace = "Sandbox.Game.Entities.Cube";
		public static string LandingGearClass = "MyLandingGear";

		//public static string LandingGearGetAutoLockMethod = "71F8F86678091875138C01C64F0C2F01";
		//public static string LandingGearGetAutoLockMethod = "3ECDCF46AB6230B4998CE81E37A36F34";

		public static string LandingGearSetAutoLockMethod = "set_AutoLock";
		public static string LandingGearGetBrakeForceMethod = "get_BreakForce";
		public static string LandingGearSetBrakeForceMethod = "set_BreakForce";

		public static string LandingGearIsLockedField = "m_needsToRetryLock";
		public static string LandingGearNetManagerField = "SyncObject";
		public static string LandingGearGetAutoLockField = "m_autoLock";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public LandingGearEntity( CubeGridEntity parent, MyObjectBuilder_LandingGear definition )
			: base( parent, definition )
		{
			m_isLocked = definition.IsLocked;
			m_autoLockEnabled = definition.AutoLock;
			m_brakeForce = definition.BrakeForce;
		}

		public LandingGearEntity( CubeGridEntity parent, MyObjectBuilder_LandingGear definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_isLocked = definition.IsLocked;
			m_autoLockEnabled = definition.AutoLock;
			m_brakeForce = definition.BrakeForce;

			m_landingNetManager = new LandingGearNetworkManager( this, GetLandingGearNetManager( ) );
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

				m_isLocked = value;

				if ( BackingObject != null && ActualObject != null )
				{
					Action action = InternalUpdateIsLocked;
					MySandboxGame.Static.Invoke( action );
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

				m_autoLockEnabled = value;

				if ( BackingObject != null && ActualObject != null )
				{
					Action action = InternalUpdateAutoLockEnabled;
					MySandboxGame.Static.Invoke( action );
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
				if ( ObjectBuilder.BrakeForce == value ) return;
				ObjectBuilder.BrakeForce = value;
				Changed = true;

				m_brakeForce = value;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBrakeForce );
				}
			}
		}

		[IgnoreDataMember]
		internal LandingGearNetworkManager LandingGearNetManager
		{
			get { return m_landingNetManager; }
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( LandingGearNamespace, LandingGearClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for LandingGearEntity" );

				//result &= HasMethod(type, LandingGearGetAutoLockMethod);
				result &= Reflection.HasMethod( type, LandingGearSetAutoLockMethod );
				result &= Reflection.HasMethod( type, LandingGearGetBrakeForceMethod );
				result &= Reflection.HasMethod( type, LandingGearSetBrakeForceMethod );

				result &= Reflection.HasField( type, LandingGearGetAutoLockField );
				result &= Reflection.HasField( type, LandingGearIsLockedField );
				result &= Reflection.HasField( type, LandingGearNetManagerField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
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
				ApplicationLog.BaseLog.Error( ex );
				return m_isLocked;
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
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return m_autoLockEnabled;
			}
		}

		protected float GetBrakeForce( )
		{
			try
			{
				float result = (float)InvokeEntityMethod( ActualObject, LandingGearGetBrakeForceMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return m_brakeForce;
			}
		}

		protected void InternalUpdateIsLocked( )
		{
			SetEntityFieldValue( ActualObject, LandingGearIsLockedField, m_isLocked );

			LandingGearNetManager.BroadcastIsLocked( );
		}

		protected void InternalUpdateAutoLockEnabled( )
		{
			InvokeEntityMethod( ActualObject, LandingGearSetAutoLockMethod, new object[ ] { m_autoLockEnabled } );

			LandingGearNetManager.BroadcastAutoLock( );
		}

		protected void InternalUpdateBrakeForce( )
		{
			InvokeEntityMethod( ActualObject, LandingGearSetBrakeForceMethod, new object[ ] { m_brakeForce } );

			LandingGearNetManager.BroadcastBrakeForce( );
		}

		#endregion "Methods"
	}

	public class LandingGearNetworkManager
	{
		#region "Attributes"

		private LandingGearEntity m_parent;
		private Object m_backingObject;

		public static string LandingGearNetworkManagerNamespace = LandingGearEntity.LandingGearNamespace + "." + LandingGearEntity.LandingGearClass;
		public static string LandingGearNetworkManagerClass = "MySyncLandingGear";

		public static string LandingGearNetworkManagerBroadcastIsLockedMethod = "SendAttachRequest";
		public static string LandingGearNetworkManagerBroadcastAutoLockMethod = "SendAutoLockChange";
		public static string LandingGearNetworkManagerBroadcastBrakeForceMethod = "SendBrakeForceChange";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public LandingGearNetworkManager( LandingGearEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		internal Object BackingObject
		{
			get { return m_backingObject; }
		}

		public static Type InternalType
		{
			get
			{
				Type type = LandingGearEntity.InternalType.GetNestedType( LandingGearNetworkManagerClass, BindingFlags.Public | BindingFlags.NonPublic );
				return type;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for LandingGearNetworkManager" );
				result &= Reflection.HasMethod( type, LandingGearNetworkManagerBroadcastIsLockedMethod );
				result &= Reflection.HasMethod( type, LandingGearNetworkManagerBroadcastAutoLockMethod );
				result &= Reflection.HasMethod( type, LandingGearNetworkManagerBroadcastBrakeForceMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void BroadcastIsLocked( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, LandingGearNetworkManagerBroadcastIsLockedMethod, new object[ ] { m_parent.IsLocked } );
		}

		public void BroadcastAutoLock( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, LandingGearNetworkManagerBroadcastAutoLockMethod, new object[ ] { m_parent.AutoLock } );
		}

		public void BroadcastBrakeForce( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, LandingGearNetworkManagerBroadcastBrakeForceMethod, new object[ ] { m_parent.BrakeForce } );
		}

		#endregion "Methods"
	}
}