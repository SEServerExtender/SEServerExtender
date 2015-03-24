namespace SEModAPIExtensions.API.Plugin.Events
{
	using SEModAPIInternal.API.Entity.Sector.SectorObject;

	public interface ICubeGridHandler
	{
		void OnCubeGridCreated(CubeGridEntity cubeGrid);
		void OnCubeGridDeleted(CubeGridEntity cubeGrid);
		void OnCubeGridMoved(CubeGridEntity cubeGrid);
		void OnCubeGridLoaded(CubeGridEntity cubeGrid);
	}
}
