using System;
using System.Collections.Generic;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Server;
using System.Reflection;

namespace SEModAPIInternal.API.Common
{
	public class PlayerManager
	{
		#region "Attributes"

		private static PlayerManager m_instance;
        private static Type m_internalType;

		//public static string PlayerManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		//public static string PlayerManagerClass = "08FBF1782D25BEBDA2070CAF8CE47D72";
        //public static string PlayerManagerPlayerMapField = "3F86E23829227B55C95CEB9F813578B2";

        public static string PlayerManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
        public static string PlayerManagerClass = "0735AF1E7659DFDA65E92992C7ECBE13";
        public static string PlayerManagerPlayerMapField = "766455F8C87C6254FB177903415443F6";

        //5F381EA9388E0A32A8C817841E192BE8
        //0735AF1E7659DFDA65E92992C7ECBE13
        //766455F8C87C6254FB177903415443F6

		#endregion

		#region "Constructors and Initializers"

		protected PlayerManager()
		{
			m_instance = this;

			Console.WriteLine("Finished loading PlayerManager");
		}

		#endregion

		#region "Properties"

		public static PlayerManager Instance
		{
			get { return m_instance ?? ( m_instance = new PlayerManager( ) ); }
		}

		public Object BackingObject
		{
			get
			{
				Object backingObject = WorldManager.Instance.InternalGetPlayerManager();

				return backingObject;
			}
		}

		public PlayerMap PlayerMap
		{
			get { return PlayerMap.Instance; }
		}

		public List<ulong> ConnectedPlayers
		{
			get
			{
				List<ulong> connectedPlayers = new List<ulong>(ServerNetworkManager.Instance.GetConnectedPlayers());

				return connectedPlayers;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerManagerNamespace, PlayerManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PlayerManager");
				bool result = true;
                result &= BaseObject.HasMethod(type1, PlayerManagerPlayerMapField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public Object InternalGetPlayerMap()
		{
            Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerManagerNamespace, PlayerManagerClass);
            MethodInfo playerMapHandler = BaseObject.GetStaticMethod(type, PlayerManagerPlayerMapField);
            Object playerMap = playerMapHandler.Invoke(type, new object[] { });
            
			return playerMap;
		}

		public void KickPlayer(ulong steamId)
		{
			ServerNetworkManager.Instance.KickPlayer(steamId);
		}

		public void BanPlayer(ulong steamId)
		{
			ServerNetworkManager.Instance.SetPlayerBan(steamId, true);
		}

		public void UnBanPlayer(ulong steamId)
		{
			ServerNetworkManager.Instance.SetPlayerBan(steamId, false);
		}

		public bool IsUserAdmin(ulong remoteUserId)
		{
			bool result = false;

			List<string> adminUsers = SandboxGameAssemblyWrapper.Instance.GetServerConfig().Administrators;
			foreach (string userId in adminUsers)
			{
				if (remoteUserId.ToString().Equals(userId))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		#endregion
	}
}

