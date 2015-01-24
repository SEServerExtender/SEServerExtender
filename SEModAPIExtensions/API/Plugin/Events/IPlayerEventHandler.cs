namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IPlayerEventHandler
	{
		void OnPlayerJoined(ulong remoteUserId);
		void OnPlayerLeft(ulong remoteUserId);
		void OnPlayerWorldSent(ulong remoteUserId);
	}
}
