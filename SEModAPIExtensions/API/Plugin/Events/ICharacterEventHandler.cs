using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface ICharacterEventHandler
	{
		void OnCharacterCreated(CharacterEntity entity);
		void OnCharacterDeleted(CharacterEntity entity);
		void OnCharacterMoved(CharacterEntity cubeGrid);
	}
}
