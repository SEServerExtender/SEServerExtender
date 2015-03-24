namespace SEModAPIInternal.API.Client
{
	using System;
	using System.Collections.Generic;
	using SEModAPIInternal.API.Common;

	public class ClientNetworkManager : NetworkManager
	{
		#region "Attributes"

		new protected static ClientNetworkManager m_instance;

		public static string ClientNetworkManagerNamespace = "";
		public static string ClientNetworkManagerClass = "=VZlADLeS8fUdvHT4xyQSVm8dYk=";
		public static string ClientNetworkManagerConnectedPlayersField = "=hrd2r4yATC2NCJnhqOpJBW6EOU=";

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