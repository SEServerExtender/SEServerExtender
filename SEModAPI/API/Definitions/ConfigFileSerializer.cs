namespace SEModAPI.API.Definitions
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Xml;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.ObjectBuilders.Definitions;

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
		/// <exception cref="FileNotFoundException">Config file specified does not exist.</exception>
		/// <exception cref="PathTooLongException">The fully qualified path and file name is 260 or more characters.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission. </exception>
		/// <exception cref="TargetInvocationException">The constructor being called throws an exception. </exception>
		/// <exception cref="MethodAccessException">The caller does not have permission to call the specified constructor. </exception>
		/// <exception cref="MemberAccessException">Cannot create an instance of an <see langword="abstract"/> class, or this member was invoked with a late-binding mechanism. </exception>
		/// <exception cref="MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="SerializationException">Unable to de-serialize file.</exception>
		public MyObjectBuilder_Definitions Deserialize()
		{
			if (!_configFileInfo.Exists)
			{
				throw new FileNotFoundException( string.Format( "The file specified in configFileInfo does not exists.\r\nCannot deserialize: {0}", _configFileInfo.FullName ), _configFileInfo.FullName );
			}

			XmlReaderSettings settings = new XmlReaderSettings();
			XmlReader reader = XmlReader.Create(_configFileInfo.FullName, settings);
			MyObjectBuilder_DefinitionsSerializer serializer = (MyObjectBuilder_DefinitionsSerializer)Activator.CreateInstance(typeof(MyObjectBuilder_DefinitionsSerializer));
			if (!serializer.CanDeserialize(reader))
			{
				throw new SerializationException( string.Format( "The file specified in configFileInfo cannot be deserialized: {0}", _configFileInfo.FullName ));
			}
			MyObjectBuilder_Definitions definitions = (MyObjectBuilder_Definitions) serializer.Deserialize(reader);
			reader.Close();
			return definitions;
		}
	}
}
