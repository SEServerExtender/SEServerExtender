namespace SEModAPIExtensions.API
{
	public struct CommandLineArgs
	{
		public bool autoStart;
		public string worldName;
		public string instanceName;
		public bool noGUI;
		public bool noConsole;
		public bool debug;
		public string gamePath;
		public bool noWCF;
		public ushort wcfPort;
		public int autosave;
		public string path;
		public bool closeOnCrash;
		public bool autoSaveSync;
		public bool restartOnCrash;
		public bool worldRequestReplace;
		public bool worldDataModify;
		public string args;
	}
}