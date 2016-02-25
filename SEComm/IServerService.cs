namespace SEComm
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using SEModAPIExtensions.API.Plugin;
	using SEModAPIInternal;
	using SEModAPIInternal.API.Chat;
	using PluginInfo = SEComm.Plugins.PluginInfo;

	[ServiceContract]
	public interface IServerService
	{
		/// <summary>
		/// Attempts to start the server and returns basic information about the server.
		/// </summary>
		/// <param name="request">A request including, most importantly, the configuration name of the SE instance to start.</param>
		/// <returns></returns>
		[OperationContract]
		StartServerResponse StartServer( StartServerRequest request );

		/// <summary>
		/// Attempts to stop the currently running SE instance.
		/// </summary>
		[OperationContract]
		void StopServer( );

		/// <summary>
		/// Immediately un-gracefully shuts the application down.
		/// </summary>
		/// <param name="exitCode">An exit code to attempt to return to the operating system.</param>
		/// <remarks>Since SESE doesn't generally close all that cleanly anyway, the exit code probably won't work and you'll probably get an exception.</remarks>
		[OperationContract]
		void Exit( int exitCode );

		[OperationContract]
		IEnumerable<ChatUserItem> GetPlayersOnline( );

		[OperationContract]
		void KickPlayer( ulong steamId );

		[OperationContract]
		void BanPlayer( ulong steamId );

		[OperationContract]
		void UnBanPlayer( ulong steamId );

		[OperationContract]
		Version GetExtenderVersion( );

		[OperationContract]
		IEnumerable<PluginInfo> GetLoadedPluginList( );

		[OperationContract]
		Guid BeginChatSession( );

		[OperationContract]
		IEnumerable<ChatMessage> GetChatMessages( Guid sessionGuid );

		[OperationContract]
		void EndChatSession( Guid sessionGuid );

		[OperationContract]
		void SendPublicChatMessage( string message );
	}
}