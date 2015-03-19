using SEModAPIInternal.API.Entity;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IBaseEntityHandler
	{
		void OnBaseEntityMoved(BaseEntity entity);
		void OnBaseEntityCreated(BaseEntity entity);
		void OnBaseEntityDeleted(BaseEntity entity);
	}
}
