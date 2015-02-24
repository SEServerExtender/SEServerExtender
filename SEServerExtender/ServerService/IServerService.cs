namespace SEServerExtender.ServerService
{
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;

	[ServiceContract( Namespace = "http://SEServerExtender/ServerService" )]
	public interface IServerService
	{
		[OperationContract]
		void StartServer( string configurationName );
		[OperationContract]
		void StopServer( );
	}
}
