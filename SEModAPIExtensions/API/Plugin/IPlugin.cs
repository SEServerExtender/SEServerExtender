namespace SEModAPIExtensions.API.Plugin
{
	using System;

	public interface IPlugin
	{
		void Init();
		void Update();
		void Shutdown();
		Guid Id
		{ get; }
		string Name
		{ get; }
		Version Version
		{ get; }
	}
}
