using VRage.Game;

namespace SEModAPI.API.Definitions
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Security;
	using global::Sandbox.Common.ObjectBuilders.Definitions;
	using VRage.ObjectBuilders;

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
		/// This method is intended to verify of the given <paramref name="configFileInfo"/> is valid
		/// </summary>
		/// <param name="configFileInfo">The valid <see cref="FileInfo"/> that points to a valid [config file name].sbc file</param>
		/// <param name="defaultName">Defines if the file has the <paramref name="defaultName"/>: [config file name].sbc</param>
		/// <exception cref="ArgumentNullException">The value of <paramref name="configFileInfo"/> cannot be null. </exception>
		/// <exception cref="ConfigurationErrorsException">The given file name does not match the default configuration name pattern.</exception>
		private static void EnsureFileInfoValidity(FileSystemInfo configFileInfo, bool defaultName = true)
		{
			if (configFileInfo == null)
			{
				throw new ArgumentNullException( "configFileInfo", "The given configFileInfo is null." );
			}
			if (defaultName)
			{
				if (configFileInfo.Extension != DefaultExtension)
				{
					throw new ConfigurationErrorsException("The given file name does not match the default configuration name pattern.");
				}
			}
		}

		/// <summary>
		/// Method to serialize a configuration file.
		/// </summary>
		/// <param name="definitions">The definition to serialize.</param>
		public void Serialize(MyObjectBuilder_Definitions definitions)
		{
			MyObjectBuilderSerializer.SerializeXML( _configFileInfo.FullName, false, definitions );
		}

		/// <summary>
		/// Method to deserialize a configuration file.
		/// </summary>
		/// <returns>The deserialized definition.</returns>
		/// <exception cref="FileNotFoundException">Config file specified does not exist.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission. </exception>
		public MyObjectBuilder_Definitions Deserialize()
		{
			if (!_configFileInfo.Exists)
			{
				throw new FileNotFoundException( string.Format( "The file specified in configFileInfo does not exists.\r\nCannot deserialize: {0}", _configFileInfo.FullName ), _configFileInfo.FullName );
			}

			MyObjectBuilder_Definitions definitions;
			MyObjectBuilderSerializer.DeserializeXML( _configFileInfo.FullName, out definitions );
			return definitions;
		}
	}
}
