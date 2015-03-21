namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IPluginHook
	{
		void OnChatHook(ChatManager.ChatEvent chatEvent, object plugin, out bool discard);
	}
}
