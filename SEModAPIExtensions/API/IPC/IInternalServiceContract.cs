namespace SEModAPIExtensions.API.IPC
{
	using System.Collections.Generic;
	using System.ServiceModel;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;

	[ServiceContract]
	public interface IInternalServiceContract
	{
		[OperationContract]
		List<ulong> GetConnectedPlayers();

		[OperationContract]
		string GetPlayerName(ulong steamId);

		[OperationContract]
		List<BaseEntity> GetSectorEntities();

		[OperationContract]
		List<CubeGridEntity> GetSectorCubeGridEntities();

		[OperationContract]
		List<CubeBlockEntity> GetCubeBlocks(long cubeGridEntityId);

		[OperationContract]
		List<InventoryItemEntity> GetInventoryItems(long cubeGridEntityId, long containerBlockEntityId, ushort inventoryIndex);

		[OperationContract]
		void UpdateEntity(BaseEntity entity);

		[OperationContract]
		void UpdateCubeBlock(CubeGridEntity parent, CubeBlockEntity cubeBlock);
	}
}