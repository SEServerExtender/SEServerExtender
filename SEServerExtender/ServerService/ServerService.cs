namespace SEServerExtender.ServerService
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using SEComm;
	using SEModAPIInternal;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.Support;

	[ServiceBehavior( ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerCall )]
	public class ServerService : IServerService
	{
		private static readonly Version ProtocolVersion = new Version( 1, 0, 0 );

		/// <summary>
		/// Starts the SE server with the given configuration name.
		/// </summary>
		/// <param name="request"></param>
		/// <remarks>Should not be called if a server is already running for this instance of SESE.</remarks>
		public StartServerResponse StartServer( StartServerRequest request )
		{
			LogManager.APILog.WriteLineAndConsole( "Received request to start server via WCF" );
			if ( Program.ServerExtenderForm != null && Program.ServerExtenderForm.Visible )
			{
				LogManager.APILog.WriteLineAndConsole( "Ignoring WCF StartServer request because GUI is active." );
				//If the local GUI is running, indicate failure for that reason.
				return new StartServerResponse
					   {
						   ExtenderVersion = Assembly.GetExecutingAssembly( ).GetName( ).Version,
						   ProtocolVersion = ProtocolVersion,
						   Status = "Not supported while GUI visible on server.",
						   StatusCode = 500
					   };
			}
			if ( Program.Server != null )
			{
				Program.Server.InstanceName = request.ConfigurationName;
				SandboxGameAssemblyWrapper.Instance.InitMyFileSystem( request.ConfigurationName );

				Program.Server.LoadServerConfig( );
				Program.Server.SaveServerConfig( );

				if ( Program.Server.Config != null )
				{
					Program.Server.StartServer( );

					return new StartServerResponse
						   {
							   StatusCode = 200,
							   Status = "OK",
							   ExtenderVersion = Assembly.GetExecutingAssembly( ).GetName( ).Version,
							   ProtocolVersion = ProtocolVersion
						   };
				}
				else
				{
					LogManager.ErrorLog.WriteLineAndConsole( "Unable to start server. Config null." );
				}
			}
			else
			{
				LogManager.ErrorLog.WriteLineAndConsole( "Unable to start server. Server null." );
			}

			return new StartServerResponse
				   {
					   ExtenderVersion = Assembly.GetExecutingAssembly( ).GetName( ).Version,
					   ProtocolVersion = ProtocolVersion,
					   Status = "Unknown error attempting to start server.",
					   StatusCode = 500
				   };
		}

		/// <summary>
		/// Stops the SE server, if it is running.
		/// </summary>
		public void StopServer( )
		{
			LogManager.APILog.WriteLineAndConsole( "Received request to stop server via WCF" );
			if ( Program.Server == null )
				return;
			if ( !Program.Server.IsRunning )
				return;
			if ( Program.ServerExtenderForm == null || !Program.ServerExtenderForm.Visible )
			{
				Program.Server.StopServer( );
			}
		}

		public void Exit( int exitCode )
		{
			StopServer( );
			Environment.Exit( exitCode );
		}

		public IEnumerable<ChatUserItem> GetPlayersOnline( )
		{
			string ip = GetRemoteEndpoint( );
			LogManager.APILog.WriteLineAndConsole( "Received character list request from {0}", null, ip );
			List<ulong> playersOnline = PlayerManager.Instance.ConnectedPlayers;
			return playersOnline.Select( remoteUserId => new ChatUserItem { Username = PlayerMap.Instance.GetPlayerNameFromSteamId( remoteUserId ), SteamId = remoteUserId } );
		}

		private static string GetRemoteEndpoint( )
		{
			OperationContext context = OperationContext.Current;
			MessageProperties prop = context.IncomingMessageProperties;
			RemoteEndpointMessageProperty endpoint = prop[ RemoteEndpointMessageProperty.Name ] as RemoteEndpointMessageProperty;
			string ip = endpoint.Address;
			return ip;
		}

		public void KickPlayer( ulong steamId )
		{
			string ip = GetRemoteEndpoint( );
			LogManager.APILog.WriteLineAndConsole( "Received request to kick player {0} from {1}", null, steamId, ip );
			PlayerManager.Instance.KickPlayer( steamId );
		}

		public void BanPlayer( ulong steamId )
		{
			string ip = GetRemoteEndpoint( );
			LogManager.APILog.WriteLineAndConsole( "Received request to ban player {0} from {1}", null, steamId, ip );
			PlayerManager.Instance.BanPlayer( steamId );
		}

		public void UnBanPlayer( ulong steamId )
		{
			string ip = GetRemoteEndpoint( );
			LogManager.APILog.WriteLineAndConsole( "Received request to un-ban player {0} from {1}", null, steamId, ip );
			PlayerManager.Instance.UnBanPlayer( steamId );
		}
	}
}
