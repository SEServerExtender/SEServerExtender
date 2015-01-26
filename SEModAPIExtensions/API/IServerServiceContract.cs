namespace SEModAPIExtensions.API
{
	using System.ServiceModel;

	[ServiceContract]
	public interface IServerServiceContract
	{
		[OperationContract]
		Server GetServer();

		[OperationContract]
		void StartServer();

		[OperationContract]
		void StopServer();

		[OperationContract]
		void LoadServerConfig();

		[OperationContract]
		void SaveServerConfig();

		[OperationContract]
		void SetAutosaveInterval(double interval);
	}
}