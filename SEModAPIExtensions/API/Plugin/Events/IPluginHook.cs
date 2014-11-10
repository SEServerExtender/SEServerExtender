using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IPluginHook
	{
		void OnChatHook(ChatManager.ChatEvent chatEvent, object plugin, out bool discard);
	}
}
