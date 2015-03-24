namespace SEModAPIExtensions.API.Plugin.Events
{
	using SEModAPIInternal.API.Entity;

	public interface IBaseEntityHandler
	{
		void OnBaseEntityMoved(BaseEntity entity);
		void OnBaseEntityCreated(BaseEntity entity);
		void OnBaseEntityDeleted(BaseEntity entity);
	}
}
