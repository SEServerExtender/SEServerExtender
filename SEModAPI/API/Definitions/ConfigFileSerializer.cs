using System;
using System.IO;
using System.Xml;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
	public class ConfigFileSerializer
	{
		private const string DefaultExtension = ".sbc";
		private static FileInfo _configFileInfo;

		public ConfigFileSerializer(FileInfo configFileInfo, bool useDefaultFileName = true)
		{
			EnsureFileInfoValidity(configFileInfo, useDefaultFileName);
			_configFileInfo = configFileInfo;
		}

		/// <summary>
		/// This method is intended to verify of the given configFileInfo is valid
		/// </summary>
		/// <param name="configFileInfo">The valid FileInfo that points to a valid [config file name].sbc file</param>
		/// <param name="defaultName">Defines if the file has the defaultName: [config file name].sbc</param>
		private static void EnsureFileInfoValidity(FileInfo configFileInfo, bool defaultName = true)
		{
			if (configFileInfo == null)
			{
				throw new SEConfigurationException(SEConfigurationExceptionState.InvalidFileInfo, "The given configFileInfo is null.");
			}
			if (defaultName)
			{
				if (configFileInfo.Extension != DefaultExtension)
				{
					throw new SEConfigurationException(SEConfigurationExceptionState.InvalidDefaultConfigFileName, "The given file name is not matching the default configuration name pattern.");
				}
			}
		}

		/// <summary>
		/// Method to serialize a configuration file.
		/// </summary>
		/// <param name="definitions">The definition to serialize.</param>
		public void Serialize(MyObjectBuilder_Definitions definitions)
		{
			XmlWriterSettings settings = new XmlWriterSettings()
			{
				CloseOutput = true,
				Indent = true,
				ConformanceLevel = ConformanceLevel.Auto,
				NewLineHandling = NewLineHandling.Entitize
			};
			XmlWriter writer = XmlWriter.Create(_configFileInfo.FullName, settings);
			MyObjectBuilder_DefinitionsSerializer serializer = (MyObjectBuilder_DefinitionsSerializer)Activator.CreateInstance(typeof(MyObjectBuilder_DefinitionsSerializer));
			serializer.Serialize(writer,definitions);
			writer.Close();
		}

		/// <summary>
		/// Method to deserialize a configuration file.
		/// </summary>
		/// <returns>The deserialized definition.</returns>
		public MyObjectBuilder_Definitions Deserialize()
		{
			if (!_configFileInfo.Exists){
				throw new SEConfigurationException(SEConfigurationExceptionState.InvalidFileInfo, "The file pointed by configFileInfo does not exists." + "\r\n" + "Cannot deserialize: " + _configFileInfo.FullName);
			}

			XmlReaderSettings settings = new XmlReaderSettings();
			XmlReader reader = XmlReader.Create(_configFileInfo.FullName, settings);
			MyObjectBuilder_DefinitionsSerializer serializer = (MyObjectBuilder_DefinitionsSerializer)Activator.CreateInstance(typeof(MyObjectBuilder_DefinitionsSerializer));
			if (!serializer.CanDeserialize(reader))
			{
				throw new SEConfigurationException(SEConfigurationExceptionState.InvalidConfigurationFile, "The file pointed by configFileInfo cannot be deserialized: " + _configFileInfo.FullName);
			}
			MyObjectBuilder_Definitions definitions = (MyObjectBuilder_Definitions) serializer.Deserialize(reader);
			reader.Close();
			return definitions;
		}
	}
}
