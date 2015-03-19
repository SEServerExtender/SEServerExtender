namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IPluginHook
	{
		void OnChatHook(ChatEvent chatEvent, object plugin, out bool discard);
	}
}
