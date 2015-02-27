namespace SEServerExtender.ServerService
{
	using System.Runtime.Serialization;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;
	using System.ServiceModel.Description;

	[ServiceContract( Namespace = "http://SEServerExtender/ServerService" )]
	public interface IServerService
	{
		[OperationContract]
		StartServerResponse StartServer( StartServerRequest request );
		[OperationContract]
		void StopServer( );
	}
}
