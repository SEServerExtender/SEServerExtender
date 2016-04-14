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
	public class PistonEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private PistonNetworkManager m_networkManager;
		private float m_velocity;
		private float m_minLimit;
		private float m_maxLimit;

		public static string PistonNamespace = "Sandbox.Game.Entities.Blocks";
		public static string PistonClass = "MyPistonBase";

		public static string PistonGetVelocityMethod = "get_Velocity";
		public static string PistonSetVelocityMethod = "set_Velocity";
		public static string PistonGetMinLimitMethod = "get_MinLimit";
		public static string PistonSetMinLimitMethod = "set_MinLimit";
		public static string PistonGetMaxLimitMethod = "get_MaxLimit";
		public static string PistonSetMaxLimitMethod = "set_MaxLimit";
		public static string PistonGetNetworkManagerMethod = "get_SyncObject";

		public static string PistonTopBlockEntityIdField = "m_topBlockId";
		public static string PistonCurrentPositionField = "m_currentPos";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public PistonEntity( CubeGridEntity parent, MyObjectBuilder_PistonBase definition )
			: base( parent, definition )
		{
			m_velocity = definition.Velocity;
			m_minLimit = definition.MinLimit.GetValueOrDefault( 0 );
			m_maxLimit = definition.MaxLimit.GetValueOrDefault( 0 );
		}

		public PistonEntity( CubeGridEntity parent, MyObjectBuilder_PistonBase definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_velocity = definition.Velocity;
			m_minLimit = definition.MinLimit.GetValueOrDefault( 0 );
			m_maxLimit = definition.MaxLimit.GetValueOrDefault( 0 );

			m_networkManager = new PistonNetworkManager( this, GetPistonNetworkManager( ) );
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Piston" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PistonNamespace, PistonClass );
				return type;
			}
		}

		[IgnoreDataMember]
		[Category( "Piston" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_PistonBase ObjectBuilder
		{
			get { return (MyObjectBuilder_PistonBase)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Piston" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public CubeBlockEntity TopBlock
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return null;

				long topBlockEntityId = GetTopBlockEntityId( );
				if ( topBlockEntityId == 0 )
					return null;
				BaseObject baseObject = GameEntityManager.GetEntity( topBlockEntityId );
				if ( baseObject == null )
					return null;
				if ( !( baseObject is CubeBlockEntity ) )
					return null;
				CubeBlockEntity block = (CubeBlockEntity)baseObject;
				return block;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Piston" )]
		[ReadOnly( true )]
		public long? TopBlockId
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.TopBlockId;

				return GetTopBlockEntityId( );
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Piston" )]
		[ReadOnly( true )]
		public float CurrentPosition
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.CurrentPosition;

				return GetPistonCurrentPosition( );
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category( "Piston" )]
		public float Velocity
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.Velocity;

				return GetPistonVelocity( );
			}
			set
			{
				if ( Velocity == value ) return;
				ObjectBuilder.Velocity = value;
				m_velocity = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( SetPistonVelocity );
				}
			}
		}

		[DataMember]
		[Category( "Piston" )]
		public float MinLimit
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.MinLimit.GetValueOrDefault( 0 );

				return GetPistonMinLimit( );
			}
			set
			{
				if ( MinLimit == value ) return;
				ObjectBuilder.MinLimit = value;
				m_minLimit = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( SetPistonMinLimit );
				}
			}
		}

		[DataMember]
		[Category( "Piston" )]
		public float MaxLimit
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.MaxLimit.GetValueOrDefault( 0 );

				return GetPistonMaxLimit( );
			}
			set
			{
				if ( MaxLimit == value ) return;
				ObjectBuilder.MaxLimit = value;
				m_maxLimit = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( SetPistonMaxLimit );
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

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for PistonEntity" );

				result &= Reflection.HasMethod( type, PistonGetVelocityMethod );
				result &= Reflection.HasMethod( type, PistonSetVelocityMethod );
				result &= Reflection.HasMethod( type, PistonGetMinLimitMethod );
				result &= Reflection.HasMethod( type, PistonSetMinLimitMethod );
				result &= Reflection.HasMethod( type, PistonGetMaxLimitMethod );
				result &= Reflection.HasMethod( type, PistonSetMaxLimitMethod );
				result &= Reflection.HasMethod( type, PistonGetNetworkManagerMethod );

				result &= Reflection.HasField( type, PistonTopBlockEntityIdField );
				result &= Reflection.HasField( type, PistonCurrentPositionField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected Object GetPistonNetworkManager( )
		{
			Object result = InvokeEntityMethod( ActualObject, PistonGetNetworkManagerMethod );
			return result;
		}

		protected long GetTopBlockEntityId( )
		{
			Object rawResult = GetEntityFieldValue( ActualObject, PistonTopBlockEntityIdField );
			if ( rawResult == null )
				return 0;
			long result = (long)rawResult;
			return result;
		}

		protected float GetPistonCurrentPosition( )
		{
			Object rawResult = GetEntityFieldValue( ActualObject, PistonCurrentPositionField );
			if ( rawResult == null )
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected float GetPistonVelocity( )
		{
			Object rawResult = InvokeEntityMethod( ActualObject, PistonGetVelocityMethod );
			if ( rawResult == null )
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void SetPistonVelocity( )
		{
			InvokeEntityMethod( ActualObject, PistonSetVelocityMethod, new object[ ] { m_velocity } );
			m_networkManager.BroadcastVelocity( m_velocity );
		}

		protected float GetPistonMinLimit( )
		{
			Object rawResult = InvokeEntityMethod( ActualObject, PistonGetMinLimitMethod );
			if ( rawResult == null )
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void SetPistonMinLimit( )
		{
			InvokeEntityMethod( ActualObject, PistonSetMinLimitMethod, new object[ ] { m_minLimit } );
			m_networkManager.BroadcastVelocity( m_minLimit );
		}

		protected float GetPistonMaxLimit( )
		{
			Object rawResult = InvokeEntityMethod( ActualObject, PistonGetMaxLimitMethod );
			if ( rawResult == null )
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void SetPistonMaxLimit( )
		{
			InvokeEntityMethod( ActualObject, PistonSetMaxLimitMethod, new object[ ] { m_maxLimit } );
			m_networkManager.BroadcastVelocity( m_maxLimit );
		}

		#endregion "Methods"
	}

	public class PistonNetworkManager
	{
		#region "Attributes"

		private PistonEntity m_parent;
		private Object m_backingObject;

		public static string PistonNetworkManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string PistonNetworkManagerClass = "MySyncPistonBase";

		public static string PistonNetworkManagerBroadcastVelocity = "SetVelocity";
		public static string PistonNetworkManagerBroadcastMinLimit = "SetMin";
		public static string PistonNetworkManagerBroadcastMaxLimit = "SetMax";

		//Packets
		//324 - Top block id
		//325 - Velocity
		//326 - Min limit
		//327 - Max limit
		//328 - Current position

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public PistonNetworkManager( PistonEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PistonNetworkManagerNamespace, PistonNetworkManagerClass );
				return type;
			}
		}

		internal Object BackingObject
		{
			get { return m_backingObject; }
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
					throw new Exception( "Could not find internal type for PistonNetworkManager" );

				result &= Reflection.HasMethod( type, PistonNetworkManagerBroadcastVelocity );
				result &= Reflection.HasMethod( type, PistonNetworkManagerBroadcastMinLimit );
				result &= Reflection.HasMethod( type, PistonNetworkManagerBroadcastMaxLimit );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		internal void BroadcastVelocity( float velocity )
		{
			BaseObject.InvokeEntityMethod( BackingObject, PistonNetworkManagerBroadcastVelocity, new object[ ] { velocity } );
		}

		internal void BroadcastMinLimit( float minLimit )
		{
			BaseObject.InvokeEntityMethod( BackingObject, PistonNetworkManagerBroadcastMinLimit, new object[ ] { minLimit } );
		}

		internal void BroadcastMaxLimit( float maxLimit )
		{
			BaseObject.InvokeEntityMethod( BackingObject, PistonNetworkManagerBroadcastMaxLimit, new object[ ] { maxLimit } );
		}

		#endregion "Methods"
	}
}