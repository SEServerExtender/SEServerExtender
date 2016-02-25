namespace SEServerGUI
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ServiceModel;
	using SEComm;
	using SEComm.Plugins;
	using SEModAPIInternal;
	using SEModAPIInternal.API.Chat;

	public sealed class ServerServiceProxy : ClientBase<IServerService>, IServerService
	{
		public ServerServiceProxy( )
			: base( "ServerService" )
		{

		}

		public ServerServiceProxy( string endpointConfigurationName )
			: base( endpointConfigurationName )
		{

		}

		public StartServerResponse StartServer( StartServerRequest request )
		{
			return Channel.StartServer( request );
		}

		public void StopServer( )
		{
			Channel.StopServer( );
		}

		public void Exit( int exitCode )
		{
			Channel.Exit( exitCode );
		}

		public IEnumerable<ChatUserItem> GetPlayersOnline( )
		{
			return Channel.GetPlayersOnline( );
		}

		public void KickPlayer( ulong steamId )
		{
			Channel.KickPlayer( steamId );
		}

		public void BanPlayer( ulong steamId )
		{
			Channel.BanPlayer( steamId );
		}

		public void UnBanPlayer( ulong steamId )
		{
			Channel.UnBanPlayer( steamId );
		}

		public Version GetExtenderVersion( )
		{
			return Channel.GetExtenderVersion( );
		}

		public IEnumerable<PluginInfo> GetLoadedPluginList( )
		{
			return Channel.GetLoadedPluginList( );
		}

		public Guid BeginChatSession( )
		{
			return Channel.BeginChatSession( );
		}

		public IEnumerable<ChatMessage> GetChatMessages( Guid sessionGuid )
		{
			return Channel.GetChatMessages( sessionGuid );
		}

		public void EndChatSession( Guid sessionGuid )
		{
			Channel.EndChatSession( sessionGuid );
		}

		public void SendPublicChatMessage( string message )
		{
			Channel.SendPublicChatMessage( message );
		}
	}
}