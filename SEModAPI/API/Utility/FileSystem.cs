using System.Reflection;

namespace SEModAPI.API.Utility
{
	using System;
	using System.IO;
	using VRage.FileSystem;

	public static class FileSystem
	{
		public static void InitMyFileSystem( string instanceName = "", bool reset = true )
		{
			string contentPath = Path.Combine( new FileInfo( MyFileSystem.ExePath ).Directory.FullName, "Content" );
			string userDataPath = GetUserDataPath( instanceName );

			if ( reset )
			{
				MyFileSystem.Reset( );
			}
			else
			{
				try
				{
					if ( !string.IsNullOrWhiteSpace( MyFileSystem.ContentPath ) )
						return;
					if ( !string.IsNullOrWhiteSpace( MyFileSystem.UserDataPath ) )
						return;
				}
				catch ( Exception )
				{
					//Do nothing
				}
			}

            //MyFileSystem.Init( contentPath, userDataPath );
            //HACK FOR STABLE COMPATABILITY KILL ME PLS
            var methodInfo = typeof(MyFileSystem).GetMethod("Init", BindingFlags.Public | BindingFlags.Static);
            if (methodInfo == null)
                throw new MissingMethodException("MyFileSystem.Init");
            //BECAUSE LET'S JUST RUIN EVERYTHING IN DEV BRANCH MKAY
		    if (methodInfo.GetParameters().Length == 3)
		        methodInfo.Invoke(null, new object[] {contentPath, userDataPath, "Mods"});
		    else
		        methodInfo.Invoke(null, new object[] {contentPath, userDataPath, "Mods", null});
            MyFileSystem.InitUserSpecific( null );

			ExtenderOptions.InstanceName = instanceName;
		}

		public static string GetUserDataPath( string instanceName = "" )
		{
			string userDataPath;
			if ( ExtenderOptions.UseCommonProgramData && instanceName != string.Empty )
			{
				userDataPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), "SpaceEngineersDedicated", instanceName );
			}
			else
			{
				userDataPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ), "SpaceEngineersDedicated" );
			}

			return userDataPath;
		}
	}
}
