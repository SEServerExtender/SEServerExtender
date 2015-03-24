namespace SEModAPIExtensions.API.Plugin.Events
{
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;

	public interface ICubeBlockEventHandler
	{
		void OnCubeBlockCreated(CubeBlockEntity entity);
		void OnCubeBlockDeleted(CubeBlockEntity entity);
	}
}
