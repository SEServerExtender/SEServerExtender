namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	using System;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class CubeGridManagerManager
	{
		#region "Attributes"

		private CubeGridEntity m_parent;
		private Object m_backingObject;

		//private CubeGridThrusterManager m_thrusterManager;

		public static string CubeGridManagerManagerNamespace = "";
		public static string CubeGridManagerManagerClass = "Sandbox.Game.Entities.Cube.MyCubeGridSystems";

		//public static string CubeGridManagerManagerGetPowerManagerMethod = "get_PowerDistributor";
		//public static string CubeGridManagerManagerGetThrusterManagerMethod = "get_ThrustSystem";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CubeGridManagerManager( CubeGridEntity parent, Object backingObject )
		{
			m_parent = parent;
			m_backingObject = backingObject;

			//m_thrusterManager = new CubeGridThrusterManager( GetThrusterManager( ), m_parent );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CubeGridManagerManagerNamespace, CubeGridManagerManagerClass );
				return type;
			}
		}

		public Object BackingObject
		{
			get { return m_backingObject; }
		}

		//public CubeGridThrusterManager ThrusterManager
		//{
		//	get { return m_thrusterManager; }
		//}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for CubeGridManagerManager" );
				bool result = true;
				//result &= Reflection.HasMethod( type, CubeGridManagerManagerGetPowerManagerMethod );
				//result &= Reflection.HasMethod( type, CubeGridManagerManagerGetThrusterManagerMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		//private Object GetThrusterManager( )
		//{
		//	Object manager = BaseObject.InvokeEntityMethod( BackingObject, CubeGridManagerManagerGetThrusterManagerMethod );
		//	return manager;
		//}

		#endregion "Methods"
	}
}