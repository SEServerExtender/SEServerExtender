namespace SEServerExtender.ServerService
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using SEComm;
	using SEComm.Plugins;
	using SEModAPI.API.Utility;
	using SEModAPIExtensions.API;
	using SEModAPIInternal;
	using SEModAPIInternal.API.Chat;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[ServiceBehavior( ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerCall )]
	public class ServerService : IServerService
	{
		private static readonly Version ProtocolVersion = new Version( 1, 1, 0 );

		/// <summary>
		/// Starts the SE server with the given configuration name.
		/// </summary>
		/// <param name="request"></param>
		/// <remarks>Should not be called if a server is already running for this instance of SESE.</remarks>
		public StartServerResponse StartServer( StartServerRequest request )
		{
			ApplicationLog.BaseLog.Info( "Received request to start server via WCF" );
			if ( Program.ServerExtenderForm != null && Program.ServerExtenderForm.Visible )
			{
				ApplicationLog.BaseLog.Info( "Ignoring WCF StartServer request because GUI is active." );
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
				FileSystem.InitMyFileSystem( request.ConfigurationName );

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
					ApplicationLog.BaseLog.Error( "Unable to start server. Config null." );
				}
			}
			else
			{
				ApplicationLog.BaseLog.Error( "Unable to start server. Server null." );
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
			ApplicationLog.BaseLog.Info( "Received request to stop server via WCF" );
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
			ApplicationLog.BaseLog.Info( "Received character list request from {0}", null, ip );
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
			ApplicationLog.BaseLog.Info( "Received request to kick player {0} from {1}", null, steamId, ip );
			PlayerManager.Instance.KickPlayer( steamId );
		}

		public void BanPlayer( ulong steamId )
		{
			string ip = GetRemoteEndpoint( );
			ApplicationLog.BaseLog.Info( "Received request to ban player {0} from {1}", null, steamId, ip );
			PlayerManager.Instance.BanPlayer( steamId );
		}

		public void UnBanPlayer( ulong steamId )
		{
			string ip = GetRemoteEndpoint( );
			ApplicationLog.BaseLog.Info( "Received request to un-ban player {0} from {1}", null, steamId, ip );
			PlayerManager.Instance.UnBanPlayer( steamId );
		}

		/// <summary>
		/// Gets the version of SESE that is currently running.
		/// </summary>
		public Version GetExtenderVersion( )
		{
			return Assembly.GetExecutingAssembly( ).GetName( ).Version;
		}

		/// <summary>
		/// Gets the list of plugins currently loaded by the server.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PluginInfo> GetLoadedPluginList( )
		{
			return PluginManager.Instance.Plugins.Select( p => new PluginInfo
															   {
																   Id = p.Value.Id,
																   Version = p.Value.Version,
																   Name = p.Value.Name
															   } );
		}

		/// <summary>
		/// Begins a chat monitoring session.
		/// </summary>
		public Guid BeginChatSession( )
		{
			Guid sessionGuid = Guid.NewGuid( );
			ApplicationLog.BaseLog.Info( "Starting new chat monitoring session {0}", sessionGuid );
			lock ( ChatSessionManager.SessionsMutex )
				ChatSessionManager.Instance.Sessions.Add( sessionGuid, new ChatSession { Id = sessionGuid } );
			return sessionGuid;
		}

		/// <summary>
		/// Gets all the messages that have not yet been delivered to the chat monitoring session.
		/// </summary>
		/// <param name="sessionGuid">The session identifier to retrieve messages for</param>
		public IEnumerable<ChatMessage> GetChatMessages( Guid sessionGuid )
		{
			ApplicationLog.BaseLog.Debug( "Getting messages for chat monitoring session {0}", sessionGuid );
			ChatMessage[ ] messages;
			lock ( ChatSessionManager.SessionsMutex )
			{
				messages = ChatSessionManager.Instance.Sessions[ sessionGuid ].Messages.ToArray(  );
				ChatSessionManager.Instance.Sessions[ sessionGuid ].Messages.Clear( );
				ChatSessionManager.Instance.Sessions[ sessionGuid ].LastUpdatedTime = DateTimeOffset.Now;
			}
			return messages;
		}

		/// <summary>
		/// Ends a chat monitoring session explicitly.
		/// </summary>
		/// <param name="sessionGuid">The session identifier to terminate</param>
		public void EndChatSession( Guid sessionGuid )
		{
			ApplicationLog.BaseLog.Info( "Ending chat monitoring session {0}", sessionGuid );
			lock ( ChatSessionManager.SessionsMutex )
				ChatSessionManager.Instance.Sessions.Remove( sessionGuid );
		}

		/// <summary>
		/// Sends a public chat message to all users, as the Server user.
		/// </summary>
		/// <param name="message">The text of the message to send.</param>
		public void SendPublicChatMessage( string message )
		{
			ChatManager.Instance.SendPublicChatMessage( message );
		}
	}
}
