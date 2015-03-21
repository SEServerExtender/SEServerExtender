using System;

namespace SEModAPIExtensions.API.Plugin
{
	public interface IPlugin
	{
		void Init();
//        void InitWithPath(String modPath);
		void Update();
		void Shutdown();
		Guid Id
		{ get; }
		string Name
		{ get; }
		string Version
		{ get; }
	}
}
