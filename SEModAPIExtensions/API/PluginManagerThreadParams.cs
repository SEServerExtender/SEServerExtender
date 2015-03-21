namespace SEModAPIExtensions.API
{
	using System;
	using System.Collections.Generic;
	using SEModAPIExtensions.API.Plugin;
	using SEModAPIInternal.API.Common;

	public class PluginManagerThreadParams
	{
		public Object Plugin;
		public Guid Key;
		public Dictionary<Guid, IPlugin> Plugins;
		public Dictionary<Guid, bool> PluginState;
		public List<EntityEventManager.EntityEvent> Events;
		public List<ChatManager.ChatEvent> ChatEvents;
	}
}