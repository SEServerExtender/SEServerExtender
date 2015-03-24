namespace SEModAPIInternal.API.Entity
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.Serialization;
	using Havok;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.Components;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.ModAPI;
	using SEModAPI.API.TypeConverters;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;
	using VRageMath;

	[DataContract( Name = "BaseEntityProxy" )]
	public class BaseEntity : BaseObject
	{
		#region "Attributes"

		private static Type m_internalType;

		private float m_maxLinearVelocity;
		private long m_entityId;
		private MyPositionAndOrientation m_positionOrientation;
		private Vector3 m_linearVelocity;
		private Vector3 m_angularVelocity;
		private BaseEntityNetworkManager m_networkManager;
		private string m_displayName;

		//Definition
		public static string BaseEntityNamespace = "";

		public static string BaseEntityClass = "Sandbox.Game.Entities.MyEntity";

		//Methods
		public static string BaseEntityGetObjectBuilderMethod = "GetObjectBuilder";

		public static string BaseEntityGetPhysicsManagerMethod = "get_Physics";

		//public static string BaseEntityCombineOnMovedEventMethod = "04F6493DF187FBA38C2B379BA9484304";
		public static string BaseEntityCombineOnClosedEventMethod = "add_OnClose";

		public static string BaseEntityGetIsDisposedMethod = "get_Closed";
		public static string BaseEntityGetOrientationMatrixMethod = "get_WorldMatrix";

		//public static string BaseEntityGetNetManagerMethod = "F4456F82186EC3AE6C73294FA6C0A11D";
		public static string BaseEntityGetNetManagerMethod = "SyncObject";

		public static string BaseEntitySetEntityIdMethod = "set_EntityId";
		public static string BaseEntityGetDisplayNameMethod = "get_DisplayName";
		public static string BaseEntitySetDisplayNameMethod = "set_DisplayName";

		public static string BaseEntityGetPositionManagerMethod = "get_PositionComp";
		//public static string BaseEntityCombineOnMovedEventMethod = "";

		public static string BaseEntityEntityIdField = "m_entityId";

		//////////////////////////////////////////////////////////

		public static string PhysicsManagerNamespace = "";
		public static string PhysicsManagerClass = "=wF2nlwFYbuUxt9MEr3pF84L4ho=";
		public static string PhysicsManagerGetRigidBodyMethod = "get_RigidBody";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BaseEntity( MyObjectBuilder_EntityBase baseEntity )
			: base( baseEntity )
		{
			if ( baseEntity != null )
			{
				m_entityId = baseEntity.EntityId;
				if ( baseEntity.PositionAndOrientation != null )
				{
					m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault( );
				}
				else
				{
					m_positionOrientation = new MyPositionAndOrientation( );
					m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition( new Vector3D( -500000, -500000, -500000 ), new Vector3D( 500000, 500000, 500000 ) );
					m_positionOrientation.Forward = new Vector3( 0, 0, 1 );
					m_positionOrientation.Up = new Vector3( 0, 1, 0 );
				}
			}
			else
			{
				m_entityId = 0;
				m_positionOrientation = new MyPositionAndOrientation( );
				m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition( new Vector3( -500000, -500000, -500000 ), new Vector3( 500000, 500000, 500000 ) );
				m_positionOrientation.Forward = new Vector3( 0, 0, 1 );
				m_positionOrientation.Up = new Vector3( 0, 1, 0 );
			}

			m_linearVelocity = new Vector3( 0, 0, 0 );
			m_angularVelocity = new Vector3( 0, 0, 0 );
			m_maxLinearVelocity = (float)104.7;
		}

		public BaseEntity( MyObjectBuilder_EntityBase baseEntity, Object backingObject )
			: base( baseEntity, backingObject )
		{
			if ( baseEntity != null )
			{
				m_entityId = baseEntity.EntityId;
				if ( baseEntity.PositionAndOrientation != null )
				{
					m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault( );
				}
				else
				{
					m_positionOrientation = new MyPositionAndOrientation( );
					m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition( new Vector3( -500000, -500000, -500000 ), new Vector3( 500000, 500000, 500000 ) );
					m_positionOrientation.Forward = new Vector3( 0, 0, 1 );
					m_positionOrientation.Up = new Vector3( 0, 1, 0 );
				}
			}
			else
			{
				m_entityId = 0;
				m_positionOrientation = new MyPositionAndOrientation( );
				m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition( new Vector3( -500000, -500000, -500000 ), new Vector3( 500000, 500000, 500000 ) );
				m_positionOrientation.Forward = new Vector3( 0, 0, 1 );
				m_positionOrientation.Up = new Vector3( 0, 1, 0 );
			}

			m_networkManager = new BaseEntityNetworkManager( this, GetEntityNetworkManager( BackingObject ) );

			m_linearVelocity = new Vector3( 0, 0, 0 );
			m_angularVelocity = new Vector3( 0, 0, 0 );
			m_maxLinearVelocity = (float)104.7;

			if ( EntityId != 0 )
			{
				GameEntityManager.AddEntity( EntityId, this );
			}

			Action action = InternalRegisterEntityMovedEvent;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal static Type InternalType
		{
			get
			{
				if ( m_internalType == null )
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( BaseEntityNamespace, BaseEntityClass );
				return m_internalType;
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "Formatted Name of an entity" )]
		public override string Name
		{
			get
			{
				return ObjectBuilder.Name == "" ? ObjectBuilder.TypeId.ToString( ) : ObjectBuilder.EntityId.ToString( );
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_EntityBase ObjectBuilder
		{
			get
			{
				MyObjectBuilder_EntityBase entityBase = (MyObjectBuilder_EntityBase)base.ObjectBuilder;
				if ( entityBase == null )
					return (MyObjectBuilder_EntityBase)null;

				entityBase.EntityId = m_entityId;
				entityBase.PositionAndOrientation = m_positionOrientation;

				return (MyObjectBuilder_EntityBase)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Entity" )]
		public virtual string DisplayName
		{
			get { return m_displayName; }
			set
			{
				if ( m_displayName == value ) return;
				m_displayName = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateDisplayName;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[Browsable( true )]
		[Description( "The unique entity ID representing a functional entity in-game" )]
		public long EntityId
		{
			get
			{
				if ( BackingObject == null )
					return m_entityId;

				long entityId = GetEntityId( BackingObject );
				if ( entityId == 0 )
					return m_entityId;

				return entityId;
			}
			set
			{
				if ( m_entityId == value ) return;
				m_entityId = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateEntityId;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public MyPersistentEntityFlags2 PersistentFlags
		{
			get { return ObjectBuilder.PersistentFlags; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		public MyPositionAndOrientation PositionAndOrientation
		{
			get
			{
				/*
				if (BackingObject == null)
					return m_positionOrientation;

				HkRigidBody body = PhysicsBody;
				if (body == null || body.IsDisposed)
					return m_positionOrientation;
				Matrix orientationMatrix = Matrix.CreateFromQuaternion(body.Rotation);
				MyPositionAndOrientation positionOrientation = new MyPositionAndOrientation(orientationMatrix);
				positionOrientation.Position = body.Position;
				 */

				HkRigidBody body = PhysicsBody;
				if ( body == null || body.IsDisposed )
					return m_positionOrientation;

				IMyEntity entity = Entity;
				if ( entity == null )
					return m_positionOrientation;

				if ( entity.Physics == null )
					return m_positionOrientation;

				MyPositionAndOrientation positionOrientation = new MyPositionAndOrientation( entity.Physics.GetWorldMatrix( ) );
				return positionOrientation;
			}
			set
			{
				m_positionOrientation = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdatePosition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
					Action action2 = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action2 );
				}
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[TypeConverter( typeof( Vector3DTypeConverter ) )]
		public Vector3DWrapper Position
		{
			get { return PositionAndOrientation.Position; }
			set
			{
				m_positionOrientation.Position = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdatePosition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper Up
		{
			get { return PositionAndOrientation.Up; }
			set
			{
				m_positionOrientation.Up = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper Forward
		{
			get { return PositionAndOrientation.Forward; }
			set
			{
				m_positionOrientation.Forward = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper LinearVelocity
		{
			get
			{
				if ( BackingObject == null )
					return m_linearVelocity;

				HkRigidBody body = PhysicsBody;
				if ( body == null || body.IsDisposed )
					return m_linearVelocity;

				if ( body.LinearVelocity == Vector3.Zero )
					return m_linearVelocity;

				return body.LinearVelocity;
			}
			set
			{
				m_linearVelocity = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper AngularVelocity
		{
			get
			{
				if ( BackingObject == null )
					return m_angularVelocity;

				HkRigidBody body = PhysicsBody;
				if ( body == null || body.IsDisposed )
					return m_angularVelocity;

				return body.AngularVelocity;
			}
			set
			{
				m_angularVelocity = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateAngularVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		public float MaxLinearVelocity
		{
			get
			{
				if ( BackingObject == null )
					return m_maxLinearVelocity;

				HkRigidBody body = PhysicsBody;
				if ( body == null || body.IsDisposed )
					return m_maxLinearVelocity;

				return body.MaxLinearVelocity;
			}
			set
			{
				if ( m_maxLinearVelocity == value ) return;
				m_maxLinearVelocity = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateMaxLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[Browsable( true )]
		[ReadOnly( true )]
		public float Mass
		{
			get
			{
				if ( BackingObject == null )
					return 0;

				HkRigidBody body = PhysicsBody;
				if ( body == null || body.IsDisposed )
					return 0;

				return body.Mass;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Entity" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3 CenterOfMass
		{
			get
			{
				return PhysicsBody.CenterOfMassWorld;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public HkRigidBody PhysicsBody
		{
			get
			{
				if ( BackingObject == null )
					return null;

				return GetRigidBody( BackingObject );
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal IMyEntity Entity
		{
			get
			{
				if ( BackingObject == null )
					return null;

				return (IMyEntity)BackingObject;
			}
		}

		[IgnoreDataMember]
		[Category( "Entity" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal BaseEntityNetworkManager BaseNetworkManager
		{
			get { return m_networkManager; }
		}

		#endregion "Properties"

		#region "Methods"

		public static long GenerateEntityId( )
		{
			return UtilityFunctions.GenerateEntityId( );
		}

		public override void Dispose( )
		{
			if ( IsDisposed )
				return;

			base.Dispose( );

			if ( BackingObject != null )
			{
				//Only remove if the backing object isn't already disposed
				bool isDisposed = (bool)InvokeEntityMethod( BackingObject, BaseEntityGetIsDisposedMethod );
				if ( !isDisposed )
				{
					m_networkManager.RemoveEntity( );

					Action action = InternalRemoveEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}

			if ( EntityId != 0 )
			{
				GameEntityManager.RemoveEntity( EntityId );
			}

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
			newEvent.type = EntityEventManager.EntityEventType.OnBaseEntityDeleted;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent( newEvent );
		}

		public override void Export( FileInfo fileInfo )
		{
			BaseObjectManager.SaveContentFile<MyObjectBuilder_EntityBase, MyObjectBuilder_EntityBaseSerializer>( ObjectBuilder, fileInfo );
		}

		new public MyObjectBuilder_EntityBase Export( )
		{
			return ObjectBuilder;
		}

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for BaseEntity" );
				bool result = true;
				result &= HasMethod( type, BaseEntityGetObjectBuilderMethod );
				result &= HasMethod( type, BaseEntityGetPhysicsManagerMethod );
				result &= HasMethod( type, BaseEntityGetPositionManagerMethod );
				//result &= HasMethod(type, BaseEntityCombineOnMovedEventMethod);
				result &= HasMethod( type, BaseEntityCombineOnClosedEventMethod );
				result &= HasMethod( type, BaseEntityGetIsDisposedMethod );
				result &= HasMethod( type, BaseEntityGetOrientationMatrixMethod );
				//result &= HasMethod( type, BaseEntityGetNetManagerMethod );
				result &= HasProperty( type, BaseEntityGetNetManagerMethod );
				result &= HasMethod( type, BaseEntitySetEntityIdMethod );
				result &= HasMethod( type, BaseEntityGetDisplayNameMethod );
				result &= HasMethod( type, BaseEntitySetDisplayNameMethod );
				result &= HasField( type, BaseEntityEntityIdField );				

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PhysicsManagerNamespace, PhysicsManagerClass );
				if ( type2 == null )
					throw new Exception( "Could not find physics manager type for BaseEntity" );
				result &= HasMethod( type2, PhysicsManagerGetRigidBodyMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error(  ex );
				return false;
			}
		}

		#region "Internal"

		private static Object GetEntityPhysicsObject( Object entity )
		{
			try
			{
				Object physicsObject = InvokeEntityMethod( entity, BaseEntityGetPhysicsManagerMethod );

				return physicsObject;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		internal static HkRigidBody GetRigidBody( Object entity )
		{
			try
			{
				Object physicsObject = GetEntityPhysicsObject( entity );
				if ( physicsObject == null )
					return null;
				HkRigidBody rigidBody = (HkRigidBody)InvokeEntityMethod( physicsObject, PhysicsManagerGetRigidBodyMethod );

				return rigidBody;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		internal static Object GetEntityNetworkManager( Object entity )
		{
			try
			{
				Object result = GetEntityPropertyValue(entity, BaseEntityGetNetManagerMethod);
				//Object result = InvokeEntityMethod( entity, BaseEntityGetNetManagerMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		internal static long GetEntityId( Object entity )
		{
			try
			{
				long entityId = 0L;
				try
				{
					entityId = (long)GetEntityFieldValue( entity, BaseEntityEntityIdField );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
				return entityId;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return 0;
			}
		}

		public static MyObjectBuilder_EntityBase GetObjectBuilder( Object entity )
		{
			MyObjectBuilder_EntityBase objectBuilder = (MyObjectBuilder_EntityBase)InvokeEntityMethod( entity, BaseEntityGetObjectBuilderMethod, new object[ ] { Type.Missing } );
			return objectBuilder;
		}

		protected void InternalUpdateMaxLinearVelocity( )
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if ( havokBody == null )
					return;

				havokBody.MaxLinearVelocity = m_maxLinearVelocity;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateEntityId( )
		{
			InvokeEntityMethod( BackingObject, BaseEntitySetEntityIdMethod, new object[ ] { EntityId } );
		}

		protected void InternalUpdatePosition( )
		{
			try
			{
				/*
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				Vector3D newPosition = m_positionOrientation.Position;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					ApplicationLog.BaseLog.Debug((this.GetType().Name + " - Changing position of '" + Name + "' from '" + havokBody.Position.ToString() + "' to '" + newPosition.ToString() + "'");
				}

				havokBody.Position = newPosition;
				 */

				IMyEntity entity = Entity;
				if ( entity == null )
					return;

				Vector3D newPosition = m_positionOrientation.Position;
				if ( SandboxGameAssemblyWrapper.IsDebugging )
				{
					ApplicationLog.BaseLog.Debug( "{0} - Changing position of '{1}' from '{2}' to '{3}'", GetType( ).Name, Name, entity.GetPosition( ).ToString( ), newPosition.ToString( ) );
				}

				entity.SetPosition( newPosition );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateOrientation( )
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if ( havokBody == null )
					return;

				Matrix orientationMatrix = m_positionOrientation.GetMatrix( ).GetOrientation( );
				Quaternion orientation = Quaternion.CreateFromRotationMatrix( orientationMatrix );
				havokBody.Rotation = orientation;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateLinearVelocity( )
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if ( havokBody == null )
					return;

				if ( SandboxGameAssemblyWrapper.IsDebugging )
				{
					ApplicationLog.BaseLog.Debug( "{0} - Changing linear velocity of '{1}' from '{2}' to '{3}'", GetType( ).Name, Name, havokBody.LinearVelocity, m_linearVelocity );
				}

				havokBody.LinearVelocity = m_linearVelocity;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateAngularVelocity( )
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if ( havokBody == null )
					return;

				if ( SandboxGameAssemblyWrapper.IsDebugging )
				{
					ApplicationLog.BaseLog.Debug( "{0} - Changing angular velocity of '{1}' from '{2}' to '{3}'", GetType( ).Name, Name, havokBody.AngularVelocity.ToString( ), m_angularVelocity.ToString( ) );
				}

				havokBody.AngularVelocity = m_angularVelocity;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalRemoveEntity( )
		{
			try
			{
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					ApplicationLog.BaseLog.Debug( "{0} '{1}': Calling 'Close' to remove entity", GetType( ).Name, Name );

				InvokeEntityMethod( BackingObject, "Close" );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( "Failed to remove entity '" + Name + "'" );
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private static Object GetEntityPositionObject( Object entity )
		{
			try
			{
				Object positionObject = InvokeEntityMethod( entity, BaseEntityGetPositionManagerMethod );
				return positionObject;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected void InternalRegisterEntityMovedEvent( )
		{
			try
			{
				MyPositionComponentBase positionComponent = (MyPositionComponentBase)GetEntityPositionObject( BackingObject );
				if ( positionComponent == null )
					return;

				Action<MyPositionComponentBase> action = InternalEntityMovedEvent;
				positionComponent.OnPositionChanged -= action;
				positionComponent.OnPositionChanged += action;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalEntityMovedEvent( Object entity )
		{
			try
			{
				if ( IsDisposed )
					return;

				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
				newEvent.type = EntityEventManager.EntityEventType.OnBaseEntityMoved;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 10;
				EntityEventManager.Instance.AddEvent( newEvent );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateDisplayName( )
		{
			InvokeEntityMethod( BackingObject, BaseEntitySetDisplayNameMethod, new object[ ] { m_displayName } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}

	public class BaseEntityNetworkManager
	{
		#region "Attributes"

		private BaseEntity m_parent;
		private Object m_networkManager;

		public static string BaseEntityNetworkManagerNamespace = "";
		public static string BaseEntityNetworkManagerClass = "=uTZ4uCH8frEHtn0VfQJ0coHE2v=";

		//public static string BaseEntityBroadcastRemovalMethod = "5EBE421019EACEA0F25718E2585CF3D2";
		public static string BaseEntityBroadcastRemovalMethod = "SendCloseRequest";

		//Packets
		//10 - ??
		//11 - ??
		//12 - Remove entity
		//5741 - ??

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BaseEntityNetworkManager( BaseEntity parent, Object netManager )
		{
			m_parent = parent;
			m_networkManager = netManager;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( BaseEntityNetworkManagerNamespace, BaseEntityNetworkManagerClass );

				return type;
			}
		}

		public static void BroadcastRemoveEntity( IMyEntity entity, bool safe = true )
		{
			Object result = BaseObject.GetEntityPropertyValue(entity, BaseEntity.BaseEntityGetNetManagerMethod);
			//Object result = BaseEntity.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetNetManagerMethod );
			if ( result == null )
				return;

			if ( safe )
			{
				SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
				{
					BaseObject.InvokeEntityMethod( result, BaseEntityBroadcastRemovalMethod );
				} );
			}
			else
			{
				BaseObject.InvokeEntityMethod( result, BaseEntityBroadcastRemovalMethod );
			}
		}

		public Object NetworkManager
		{
			get
			{
				if ( m_networkManager == null )
				{
					m_networkManager = BaseEntity.GetEntityNetworkManager( m_parent.BackingObject );
				}

				return m_networkManager;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for BaseEntityNetworkManager" );
				bool result = BaseObject.HasMethod( type, BaseEntityBroadcastRemovalMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void RemoveEntity( )
		{
			if ( NetworkManager == null )
				return;

			Action action = InternalRemoveEntity;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		protected void InternalRemoveEntity( )
		{
			try
			{
				if ( NetworkManager == null )
					return;

				BaseObject.InvokeEntityMethod( NetworkManager, BaseEntityBroadcastRemovalMethod );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}