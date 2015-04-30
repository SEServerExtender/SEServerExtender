namespace SEModAPIInternal.API.Client
{
	using System;
	using System.Collections.Generic;
	using SEModAPIInternal.API.Common;

	public class ClientNetworkManager : NetworkManager
	{
		#region "Attributes"

		new protected static ClientNetworkManager m_instance;

		public static string ClientNetworkManagerNamespace = "Sandbox.Engine.Multiplayer";
		public static string ClientNetworkManagerClass = "MyMultiplayerClient";
		public static string ClientNetworkManagerConnectedPlayersField = "m_members";

		#endregion "Attributes"

		#region "Properties"

		new public static ClientNetworkManager Instance
		{
			get { return m_instance ?? ( m_instance = new ClientNetworkManager( ) ); }
		}

		#endregion "Properties"

		#region "Methods"

		public override List<ulong> GetConnectedPlayers( )
		{
			throw new NotImplementedException( );
		}

		#endregion "Methods"
	}
}