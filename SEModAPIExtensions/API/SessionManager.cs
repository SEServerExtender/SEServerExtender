using VRage.Game;

namespace SEModAPIExtensions.API
{
    using System;
    using System.IO;
    using Sandbox.Common.ObjectBuilders;
    using SEModAPI.API;
    using SEModAPIInternal.Support;
    using VRage.ObjectBuilders;

    public class SessionManager
	{
		private static MyObjectBuilder_Checkpoint m_checkPoint;
		private static SessionManager m_instance;

		public SessionManager()
		{
			m_instance = this;
		}

		public static SessionManager Instance
		{
			get { return m_instance ?? ( m_instance = new SessionManager( ) ); }
		}

		private static MyObjectBuilder_Checkpoint LoadSandbox( string savePath, out ulong fileSize )
		{
			fileSize = 0UL;
			string path = Path.Combine(savePath, "Sandbox.sbc");

			FileInfo sandboxFile = new FileInfo( path );

			if (!sandboxFile.Exists || sandboxFile.Length == 0)
				return null;

			MyObjectBuilder_Checkpoint objectBuilder;

			MyObjectBuilderSerializer.DeserializeXML(path, out objectBuilder, out fileSize);

			if ( objectBuilder != null && string.IsNullOrEmpty( objectBuilder.SessionName ) )
				objectBuilder.SessionName = Path.GetFileNameWithoutExtension( path );

			return objectBuilder;
		}

		private static void SaveSandbox( MyObjectBuilder_Base objectBuilder, string savePath, out ulong fileSize )
		{
			string path = Path.Combine(savePath, "Sandbox.sbc");
			MyObjectBuilderSerializer.SerializeXML(path, false, objectBuilder, out fileSize);
		}

		public void UpdateSessionSettings( )
		{
			MyConfigDedicatedData<MyObjectBuilder_SessionSettings> config = Server.Instance.LoadServerConfig( );

            ApplicationLog.BaseLog.Info( "Loading Session Settings" );
			try
			{
				string worldPath = config.LoadWorld;

				ulong fileSize;
				m_checkPoint = LoadSandbox(worldPath, out fileSize);

				if (m_checkPoint == null)
					return;

				m_checkPoint.Settings = config.SessionSettings;
				//m_checkPoint.Scenario = config.Scenario;

				m_checkPoint.Mods.Clear( );
				foreach ( ulong modid in config.Mods )
					m_checkPoint.Mods.Add( new MyObjectBuilder_Checkpoint.ModItem( modid ) );

				File.Copy( Path.Combine( worldPath, "Sandbox.sbc" ), Path.Combine( worldPath, "Sandbox.sbc.bak" ), true );

				SaveSandbox(m_checkPoint, worldPath, out fileSize);

				ApplicationLog.BaseLog.Info( "{0}Max Players: {1}", Environment.NewLine, m_checkPoint.Settings.MaxPlayers );
				ApplicationLog.BaseLog.Info( "OnlineMode: {0}", m_checkPoint.Settings.OnlineMode );
				ApplicationLog.BaseLog.Info( "GameMode: {0}", m_checkPoint.Settings.GameMode );
				ApplicationLog.BaseLog.Info( "Scenario: {0}", m_checkPoint.Scenario.SubtypeId );
				ApplicationLog.BaseLog.Info( "World Size: {0}{1}", m_checkPoint.Settings.WorldSizeKm, Environment.NewLine );
			}
			catch (Exception ex)
			{
                if ( ExtenderOptions.IsDebugging )
                    ApplicationLog.BaseLog.Error( ex, "Session Manager Exception: {0}" );
            }
        }
	}
}
