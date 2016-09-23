namespace SEModAPIExtensions.API
{
	public class CommandLineArgs
	{
		public bool AutoStart;
		public string WorldName;
        /// <summary>The title of the console title</summary>
        public string ConsoleTitle;
		/// <summary>The name of the game instance to load</summary>
		public string InstanceName { get; set; }
		/// <summary>The path to the instance folder</summary>
		public string InstancePath;
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
		public bool CloseOnCrash;
		public bool AutoSaveSync;
		public bool RestartOnCrash;
		public bool WorldRequestReplace;
		public bool WorldDataModify;
        public bool WorldVoxelModify;
		public string Args;
        /// <summary>
        /// Disable Keen's terrible profiler
        /// </summary>
	    public bool NoProfiler;
	}
}