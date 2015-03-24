namespace SEModAPIExtensions.API.Plugin.Events
{
	using SEModAPIInternal.API.Entity.Sector.SectorObject;

	public interface ICharacterEventHandler
	{
		void OnCharacterCreated(CharacterEntity entity);
		void OnCharacterDeleted(CharacterEntity entity);
		void OnCharacterMoved(CharacterEntity cubeGrid);
	}
}
