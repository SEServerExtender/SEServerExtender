namespace SEModAPIInternal.API.Common
{
	using System;
	using System.ComponentModel;
	using Sandbox;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;

	public class RadioManager
	{
		#region "Attributes"

		private Object m_backingObject;
		private RadioManagerNetworkManager m_networkManager;

		private float m_broadcastRadius;
		private Object m_linkedEntity;
		private bool m_isEnabled;
		private int m_aabbTreeId;

		public static string RadioManagerNamespace = "Sandbox.Game.Entities.Cube";
		public static string RadioManagerClass = "MyRadioBroadcaster";

		public static string RadioManagerGetBroadcastRadiusMethod = "get_BroadcastRadius";
		public static string RadioManagerSetBroadcastRadiusMethod = "set_BroadcastRadius";
		public static string RadioManagerGetEnabledMethod = "get_Enabled";
		public static string RadioManagerSetEnabledMethod = "set_Enabled";
		public static string RadioManagerGetAABBTreeIdMethod = "get_RadioProxyID";
		public static string RadioManagerSetAABBTreeIdMethod = "set_RadioProxyID";

		public static string RadioManagerNetworkManagerField = "SyncObject";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public RadioManager( Object backingObject )
		{
			try
			{
				m_backingObject = backingObject;
				m_networkManager = new RadioManagerNetworkManager( this );

				m_broadcastRadius = (float)BaseObject.InvokeEntityMethod( BackingObject, RadioManagerGetBroadcastRadiusMethod );
				//m_linkedEntity = BaseObject.InvokeEntityMethod( BackingObject, RadioManagerGetLinkedEntityMethod );
				m_isEnabled = (bool)BaseObject.InvokeEntityMethod( BackingObject, RadioManagerGetEnabledMethod );
				m_aabbTreeId = (int)BaseObject.InvokeEntityMethod( BackingObject, RadioManagerGetAABBTreeIdMethod );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[Category( "Radio Manager" )]
		[Browsable( false )]
		public Object BackingObject
		{
			get { return m_backingObject; }
		}

		[Category( "Radio Manager" )]
		public float BroadcastRadius
		{
			get { return m_broadcastRadius; }
			set
			{
				if ( m_broadcastRadius == value ) return;
				m_broadcastRadius = value;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateBroadcastRadius );
				}
			}
		}

		[Category( "Radio Manager" )]
		[Browsable( false )]
		public Object LinkedEntity
		{
			get { return m_linkedEntity; }
			set
			{
				if ( m_linkedEntity == value ) return;
				m_linkedEntity = value;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateLinkedEntity );
				}
			}
		}

		[Category( "Radio Manager" )]
		public bool Enabled
		{
			get { return m_isEnabled; }
			set
			{
				if ( m_isEnabled == value ) return;
				m_isEnabled = value;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateEnabled );
				}
			}
		}

		[Category( "Radio Manager" )]
		public int TreeId
		{
			get { return m_aabbTreeId; }
			set
			{
				if ( m_aabbTreeId == value ) return;
				m_aabbTreeId = value;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateTreeId );
				}
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( RadioManagerNamespace, RadioManagerClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for RadioManager" );
				bool result = true;
				result &= Reflection.HasMethod( type1, RadioManagerGetBroadcastRadiusMethod );
				result &= Reflection.HasMethod( type1, RadioManagerSetBroadcastRadiusMethod );
				//result &= BaseObject.HasMethod( type1, RadioManagerGetLinkedEntityMethod );
				//result &= BaseObject.HasMethod( type1, RadioManagerSetLinkedEntityMethod );
				result &= Reflection.HasMethod( type1, RadioManagerGetEnabledMethod );
				result &= Reflection.HasMethod( type1, RadioManagerSetEnabledMethod );
				result &= Reflection.HasMethod( type1, RadioManagerGetAABBTreeIdMethod );
				result &= Reflection.HasMethod( type1, RadioManagerSetAABBTreeIdMethod );
				result &= Reflection.HasField( type1, RadioManagerNetworkManagerField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error(  ex );
				return false;
			}
		}

		internal Object GetNetworkManager( )
		{
			try
			{
				Object result = BaseObject.GetEntityFieldValue( BackingObject, RadioManagerNetworkManagerField );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected void InternalUpdateBroadcastRadius( )
		{
			m_networkManager.BroadcastRadius( );
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerSetBroadcastRadiusMethod, new object[ ] { BroadcastRadius } );
		}

		protected void InternalUpdateLinkedEntity( )
		{
			//BaseObject.InvokeEntityMethod( BackingObject, RadioManagerSetLinkedEntityMethod, new object[ ] { LinkedEntity } );
		}

		protected void InternalUpdateEnabled( )
		{
			m_networkManager.BroadcastEnabled( );
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerSetEnabledMethod, new object[ ] { Enabled } );
		}

		protected void InternalUpdateTreeId( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerSetAABBTreeIdMethod, new object[ ] { TreeId } );
		}

		#endregion "Methods"
	}

	public class RadioManagerNetworkManager
	{
		#region "Attributes"

		private RadioManager m_parent;

		public static string RadioManagerNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string RadioManagerNetManagerClass = "MySyncRadioBroadcaster";

		public static string RadioManagerNetManagerBroadcastRadiusMethod = "SendChangeRadioAntennaRequest";
		public static string RadioManagerNetManagerBroadcastEnabledMethod = "SendChangeRadioAntennaDisplayName";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public RadioManagerNetworkManager( RadioManager parent )
		{
			m_parent = parent;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public Object BackingObject
		{
			get
			{
				Object result = m_parent.GetNetworkManager( );
				return result;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( RadioManagerNetManagerNamespace, RadioManagerNetManagerClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for RadioManagerNetworkManager" );
				bool result = true;
				result &= Reflection.HasMethod( type1, RadioManagerNetManagerBroadcastRadiusMethod );
				result &= Reflection.HasMethod( type1, RadioManagerNetManagerBroadcastEnabledMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error(  ex );
				return false;
			}
		}

		public void BroadcastRadius( )
		{
			MySandboxGame.Static.Invoke( InternalBroadcastRadius );
		}

		public void BroadcastEnabled( )
		{
			MySandboxGame.Static.Invoke( InternalBroadcastEnabled );
		}

		#region "Internal"

		protected void InternalBroadcastRadius( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerNetManagerBroadcastRadiusMethod, new object[ ] { m_parent.BroadcastRadius, true } );
		}

		protected void InternalBroadcastEnabled( )
		{
			BaseObject.InvokeEntityMethod( BackingObject, RadioManagerNetManagerBroadcastEnabledMethod, new object[ ] { m_parent.Enabled } );
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}