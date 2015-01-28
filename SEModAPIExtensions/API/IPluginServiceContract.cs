namespace SEModAPIExtensions.API
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	[ServiceContract]
	public interface IPluginServiceContract
	{
		[OperationContract]
		List<Guid> GetPluginGuids();

		[OperationContract]
		bool GetPluginStatus(Guid guid);

		[OperationContract]
		void LoadPlugin(Guid guid);

		[OperationContract]
		void UnloadPlugin(Guid guid);
	}
}