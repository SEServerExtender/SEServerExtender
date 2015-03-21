namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IChatEventHandler
	{
		void OnChatReceived(ChatEvent chatEvent);
		void OnChatSent(ChatEvent chatEvent);
	}
}
