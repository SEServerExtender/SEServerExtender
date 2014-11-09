using System;
using System.IO;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Serializer;

using SEModAPI.API.Definitions;
using SEModAPIInternal.Support;

namespace SEModAPIExtensions.API
{
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
			get
			{
				if (m_instance == null)
					m_instance = new SessionManager();

				return m_instance;
			}
		}

		private MyObjectBuilder_Checkpoint LoadSandbox(string savePath, out ulong fileSize)
		{
			fileSize = 0UL;
			string path = Path.Combine(savePath, "Sandbox.sbc");

			var sandboxFile = new FileInfo(path);

			if (!sandboxFile.Exists || sandboxFile.Length == 0)
				return (MyObjectBuilder_Checkpoint)null;

			MyObjectBuilder_Checkpoint objectBuilder = (MyObjectBuilder_Checkpoint)null;

			MyObjectBuilderSerializer.DeserializeXML<MyObjectBuilder_Checkpoint>(path, out objectBuilder, out fileSize);

			if (objectBuilder != null && string.IsNullOrEmpty(objectBuilder.SessionName))
				objectBuilder.SessionName = Path.GetFileNameWithoutExtension(path);

			return objectBuilder;
		}

		private bool SaveSandbox(MyObjectBuilder_Checkpoint objectBuilder, string savePath, out ulong fileSize)
		{
			string path = Path.Combine(savePath, "Sandbox.sbc");
			return MyObjectBuilderSerializer.SerializeXML(path, false, (MyObjectBuilder_Base)objectBuilder, out fileSize, (Type)null);
		}

		public void UpdateSessionSettings()
		{
			ulong fileSize = 0UL;

			MyConfigDedicatedData config = Server.Instance.LoadServerConfig();

			Console.WriteLine("Loading Session Settings");
			try
			{
				var worldPath = config.LoadWorld;

				m_checkPoint = LoadSandbox(worldPath, out fileSize);

				if (m_checkPoint == null)
					return;

				m_checkPoint.Settings = config.SessionSettings;
				m_checkPoint.Scenario = config.Scenario;

				m_checkPoint.Mods.Clear();
				foreach (ulong modid in config.Mods)
					m_checkPoint.Mods.Add(new MyObjectBuilder_Checkpoint.ModItem(modid));

				File.Copy(worldPath + "\\Sandbox.sbc", worldPath + "\\Sandbox.sbc.bak", true);

				SaveSandbox(m_checkPoint, worldPath, out fileSize);

				Console.WriteLine(Environment.NewLine + "Max Players: " + m_checkPoint.Settings.MaxPlayers);
				Console.WriteLine("OnlineMode: " + m_checkPoint.Settings.OnlineMode);
				Console.WriteLine("GameMode: " + m_checkPoint.Settings.GameMode);
				Console.WriteLine("Scenario: " + m_checkPoint.Scenario.SubtypeId);
				Console.WriteLine("World Size: " + m_checkPoint.Settings.WorldSizeKm + Environment.NewLine);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("Session Manager Exception: " + ex.ToString());
				return;
			}
		}
	}
}
