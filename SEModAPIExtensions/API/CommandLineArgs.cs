namespace SEModAPIExtensions.API
{
	public class CommandLineArgs
	{
		public bool AutoStart;
		public string WorldName;
		public string InstanceName;
		/// <summary>Do not show the local GUI, if true.</summary>
		public bool NoGui;
		/// <summary>Do not show a command-line console, if true.</summary>
		public bool NoConsole;
		/// <summary>Output additional debugging log information.</summary>
		public bool Debug;
		/// <summary>The path to the game installation folder (parent of DedicatedServer64 folder).</summary>
		public string GamePath;
		/// <summary>Disable WCF services, if true.</summary>
		public bool NoWcf;
		public int Autosave;
		public string Path;
		public bool CloseOnCrash;
		public bool AutoSaveSync;
		public bool RestartOnCrash;
		public bool WorldRequestReplace;
		public bool WorldDataModify;
		public string Args;
	}
}