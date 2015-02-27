namespace SEComm
{
	using System.Collections.Generic;
	using System.ServiceModel;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;

	[ServiceContract]
	public interface IServerService
	{
		[OperationContract]
		StartServerResponse StartServer( StartServerRequest request );
		[OperationContract]
		void StopServer( );
		[OperationContract]
		void Exit( int exitCode );

		[OperationContract]
		List<CharacterEntity> GetPlayersOnline( );
	}
}