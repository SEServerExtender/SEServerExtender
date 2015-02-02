namespace SEModAPIInternal.API.Entity
{
	using System;
	using Sandbox.ModAPI;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class BaseEntityNetworkManager
	{
		#region "Attributes"

		private BaseEntity m_parent;
		private Object m_networkManager;

		public static string BaseEntityNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string BaseEntityNetworkManagerClass = "48D79F8E3C8922F14D85F6D98237314C";

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
			Object result = BaseObject.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetNetManagerMethod );
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
			get { return m_networkManager ?? ( m_networkManager = BaseEntity.GetEntityNetworkManager( m_parent.BackingObject ) ); }
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
				LogManager.APILog.WriteLine( ex );
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
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}