namespace SEModAPIExtensions.API
{
	using System.ServiceModel;

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Single,
		IncludeExceptionDetailInFaults = true,
		IgnoreExtensionDataObject = true
		)]
	public class ServerService : IServerServiceContract
	{
		public Server GetServer()
		{
			return Server.Instance;
		}

		public void StartServer()
		{
			Server.Instance.StartServer();
		}

		public void StopServer()
		{
			Server.Instance.StopServer();
		}

		public void LoadServerConfig()
		{
			Server.Instance.LoadServerConfig();
		}

		public void SaveServerConfig()
		{
			Server.Instance.SaveServerConfig();
		}

		public void SetAutosaveInterval(double interval)
		{
			Server.Instance.AutosaveInterval = interval;
		}
	}
}