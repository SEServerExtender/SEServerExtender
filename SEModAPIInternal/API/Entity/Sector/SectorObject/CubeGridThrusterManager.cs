using System;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class CubeGridThrusterManager
	{
		#region "Attributes"

		private Object m_thrusterManager;
		private CubeGridEntity m_parent;

		private bool m_dampenersEnabled;

		public static string CubeGridThrusterManagerNamespace = "8EAF60352312606996BD8147B0A3C880";
		public static string CubeGridThrusterManagerClass = "958ADAA3423FBDC5DE98C1A32DE7258C";

		public static string CubeGridThrusterManagerGetEnabled = "51FDDFF9224B3F717EEFFEBEA5F1BAF6";
		public static string CubeGridThrusterManagerSetEnabled = "86B66668D555E1C1B744C17D2AFA77F7";
		public static string CubeGridThrusterManagerSetControlEnabled = "BC83851AFAE183711CFB864BA6F62CC6";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CubeGridThrusterManager( Object thrusterManager, CubeGridEntity parent )
		{
			m_thrusterManager = thrusterManager;
			m_parent = parent;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public Object BackingObject
		{
			get { return m_thrusterManager; }
		}

		public bool DampenersEnabled
		{
			get { return InternalGetDampenersEnabled( ); }
			set
			{
				m_dampenersEnabled = value;

				Action action = InternalUpdateDampenersEnabled;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CubeGridThrusterManagerNamespace, CubeGridThrusterManagerClass );
				if ( type == null )
					throw new Exception( "Could not find type for CubeGridThrusterManager" );
				result &= BaseObject.HasMethod( type, CubeGridThrusterManagerGetEnabled );
				result &= BaseObject.HasMethod( type, CubeGridThrusterManagerSetEnabled );
				result &= BaseObject.HasMethod( type, CubeGridThrusterManagerSetControlEnabled );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		protected bool InternalGetDampenersEnabled( )
		{
			bool result = (bool)BaseObject.InvokeEntityMethod( BackingObject, CubeGridThrusterManagerGetEnabled );
			return result;
		}

		protected void InternalUpdateDampenersEnabled( )
		{
			foreach ( CubeBlockEntity cubeBlock in m_parent.CubeBlocks )
			{
				if ( cubeBlock is CockpitEntity )
				{
					CockpitEntity cockpit = (CockpitEntity)cubeBlock;
					if ( cockpit.IsPassengerSeat )
						continue;

					cockpit.NetworkManager.BroadcastDampenersStatus( m_dampenersEnabled );
					break;
				}
			}

			BaseObject.InvokeEntityMethod( BackingObject, CubeGridThrusterManagerSetEnabled, new object[ ] { m_dampenersEnabled } );
			//BaseObject.InvokeEntityMethod(BackingObject, CubeGridThrusterManagerSetControlEnabled, new object[] { m_dampenersEnabled });
		}

		#endregion "Methods"
	}
}