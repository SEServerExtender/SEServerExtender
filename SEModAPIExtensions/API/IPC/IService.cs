namespace SEModAPIExtensions.API.IPC
{
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Web;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;

	[ServiceContract]
	public interface IWebServiceContract
	{
		[OperationContract]
		[WebInvoke(Method = "OPTIONS", UriTemplate = "")]
		void GetOptions();

		[OperationContract]
		[WebGet]
		List<BaseEntity> GetSectorEntities();

		[OperationContract]
		[WebGet]
		List<CubeGridEntity> GetSectorCubeGridEntities();

		[OperationContract]
		[WebGet(UriTemplate = "GetCubeBlocks/{cubeGridEntityId}")]
		List<CubeBlockEntity> GetCubeBlocks(string cubeGridEntityId);
	}
}
