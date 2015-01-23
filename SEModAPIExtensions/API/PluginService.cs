namespace SEModAPIExtensions.API
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Single,
		IncludeExceptionDetailInFaults = true,
		IgnoreExtensionDataObject = true
		)]
	public class PluginService : IPluginServiceContract
	{
		public List<Guid> GetPluginGuids()
		{
			return new List<Guid>(PluginManager.Instance.Plugins.Keys);
		}

		public bool GetPluginStatus(Guid guid)
		{
			return PluginManager.Instance.GetPluginState(guid);
		}

		public void LoadPlugin(Guid guid)
		{
			PluginManager.Instance.InitPlugin(guid);
		}

		public void UnloadPlugin(Guid guid)
		{
			PluginManager.Instance.UnloadPlugin(guid);
		}
	}
}