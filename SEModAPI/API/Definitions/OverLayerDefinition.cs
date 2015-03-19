using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Reflection;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
	/// <summary>
	/// This class is only intended for easy data access and management. It is a wrapper around all MyObjectBuilder_Definitions children sub type
	/// </summary>
	public abstract class OverLayerDefinition<T>
	{
		#region "Attributes"

		protected T m_baseDefinition;
		protected string m_name;

		#endregion

		#region "Constructors and Initializers"

		protected OverLayerDefinition(T baseDefinition)
		{
			m_baseDefinition = baseDefinition;
			m_name = GetNameFrom(m_baseDefinition);
			Changed = false;
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Gets the changed status of the object
		/// </summary>
		[Browsable(false)]
		public bool Changed { get; protected set; }

		/// <summary>
		/// Obtain a nicely formated name of the object
		/// </summary>
		[Browsable(false)]
		public string Name
		{
			get { return GetNameFrom(m_baseDefinition); }
		}

		/// <summary>
		/// Gets the internal data of the object
		/// </summary>
		[Browsable(false)]
		public T BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// This template method should return the representative name of the sub type underlayed by a children
		/// </summary>
		/// <param name="definition">The definition from which to get the information</param>
		/// <returns></returns>
		protected abstract string GetNameFrom(T definition);

		#endregion
	}

	public abstract class ObjectOverLayerDefinition<TMyObjectBuilder_Definitions_SubType> : OverLayerDefinition<TMyObjectBuilder_Definitions_SubType> where TMyObjectBuilder_Definitions_SubType : MyObjectBuilder_DefinitionBase
	{
		#region "Constructors and Initializers"

		protected ObjectOverLayerDefinition(TMyObjectBuilder_Definitions_SubType baseDefinition)
			: base(baseDefinition)
		{ }

		#endregion

		#region "Properties"

		public SerializableDefinitionId Id
		{
			get { return m_baseDefinition.Id; }
			set
			{
				if (m_baseDefinition.Id.ToString() == value.ToString()) return;
				m_baseDefinition.Id = value;
				Changed = true;
			}
		}

		public string Description
		{
			get { return m_baseDefinition.Description; }
			set
			{
				if (m_baseDefinition.Description == value) return;
				m_baseDefinition.Description = value;
				Changed = true;
			}
		}

		#endregion
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class OverLayerDefinitionsManager<T, TU> where TU : OverLayerDefinition<T>
	{
		#region "Attributes"

		private bool m_isMutable = true;
		private bool m_changed = false;

		//Use Long (key) as Id and OverLayerDefinition sub type (value) as Name
		//For entity objects (saved game data) we use EntityId as the long key
		private readonly Dictionary<long, TU> m_definitions = new Dictionary<long, TU>();

		#endregion

		#region "Constructors and Initializers"

		protected OverLayerDefinitionsManager()
		{
			m_changed = false;
			m_isMutable = true;
		}

		protected OverLayerDefinitionsManager(IEnumerable<T> baseDefinitions)
		{
			m_changed = false;
			m_isMutable = true;

			foreach (T definition in baseDefinitions)
			{
				NewEntry(definition);
			}
		}

		#endregion

		#region "Properties"

		public bool IsMutable
		{
			get { return m_isMutable; }
			set { m_isMutable = value; }
		}

		protected bool Changed
		{
			get
			{
				if (m_changed) return true;
				return GetInternalData( ).Any( def => GetChangedState( def.Value ) );
			}
		}

		public TU[] Definitions
		{
			get
			{
				return GetInternalData().Values.ToArray();
			}
		}

		#endregion

		#region "Methods"

		protected virtual Dictionary<long, TU> GetInternalData()
		{
			return m_definitions;
		}

		/// <summary>
		/// This method is used to extract all instances of MyObjectBuilder_Definitions sub type encapsulated in the manager
		/// This method is slow and should only be used to extract the underlying type
		/// </summary>
		/// <returns>All instances of the MyObjectBuilder_Definitions sub type sub type in the manager</returns>
		public List<T> ExtractBaseDefinitions()
		{
			return GetInternalData( ).Values.Select( GetBaseTypeOf ).ToList( );
		}

		private bool IsIdValid(long id)
		{
			return GetInternalData().ContainsKey(id);
		}

		private bool IsIndexValid(int index)
		{
			return (index < GetInternalData().Keys.Count && index >= 0);
		}

		public TU DefinitionOf(long id)
		{
			TU result = default(TU);
			if (IsIdValid(id))
				GetInternalData().TryGetValue(id, out result);

			return result;
		}

		public TU DefinitionOf(int index)
		{
			return IsIndexValid(index) ? GetInternalData().Values.ToArray()[index] : default(TU);
		}

		#region "Abstract Methods"

		/// <summary>
		/// Template method that create the instance of the children of OverLayerDefinition sub type
		/// </summary>
		/// <param name="definition">MyObjectBuilder_Definitions object</param>
		/// <returns>An instance representing OverLayerDefinition sub type</returns>
		protected abstract TU CreateOverLayerSubTypeInstance(T definition);

		/// <summary>
		/// This template method is intended to extact the BaseObject inside the overlayer
		/// </summary>
		/// <param name="overLayer">the over layer from which to extract the base object</param>
		/// <returns>MyObjectBuilder_Definitions Sub Type</returns>
		protected abstract T GetBaseTypeOf(TU overLayer);

		/// <summary>
		/// This template method is intended to know if the state of the object insate the overlayer has changed
		/// </summary>
		/// <param name="overLayer">the overlayer from which to know if the base type has changed</param>
		/// <returns>if the underlying object has changed</returns>
		protected abstract bool GetChangedState(TU overLayer);

		#endregion

		public TU NewEntry()
		{
			if (!IsMutable) return default(TU);

			T sourceInstance = (T)Activator.CreateInstance(typeof(T), new object[] { });
			return NewEntry(sourceInstance);
		}

		public TU NewEntry(long id)
		{
			if (!IsMutable) return default(TU);

			TU newEntry = CreateOverLayerSubTypeInstance((T)Activator.CreateInstance(typeof(T), new object[] { }));
			GetInternalData().Add(id, newEntry);
			m_changed = true;

			return newEntry;
		}

		public TU NewEntry(T source)
		{
			if (!IsMutable) return default(TU);

			TU newEntry = CreateOverLayerSubTypeInstance(source);
			GetInternalData().Add(m_definitions.Count, newEntry);
			m_changed = true;

			return newEntry;
		}

		public TU NewEntry(TU source)
		{
			if (!IsMutable) return default(TU);

			//Create the new object
			Type entryType = typeof(T);
			T newEntry = (T)Activator.CreateInstance(entryType, new object[] { });

			//Copy the field data
			//TODO - Find a way to fully copy complex data structures in fields instead of just copying reference
			foreach (FieldInfo field in entryType.GetFields())
			{
				field.SetValue(newEntry, field.GetValue(source.BaseDefinition));
			}

			//Add the new object to the manager as a new entry
			return NewEntry(newEntry);
		}

		public bool DeleteEntry(long id)
		{
			if (!IsMutable) return false;

			if (GetInternalData().ContainsKey(id))
			{
				GetInternalData().Remove(id);
				m_changed = true;
				return true;
			}

			return false;
		}

		public bool DeleteEntry(T entry)
		{
			if (!IsMutable) return false;

			foreach (KeyValuePair<long, TU> def in m_definitions)
			{
				if (def.Value.BaseDefinition.Equals(entry))
				{
					GetInternalData().Remove(def.Key);
					m_changed = true;
					return true;
				}
			}

			return false;
		}

		public bool DeleteEntry(TU entry)
		{
			if (!IsMutable) return false;

			foreach (KeyValuePair<long, TU> def in m_definitions)
			{
				if (def.Value.Equals(entry))
				{
					GetInternalData().Remove(def.Key);
					m_changed = true;
					return true;
				}
			}

			return false;
		}

		#endregion
	}

	public class SerializableDefinitionsManager<T, TU> : OverLayerDefinitionsManager<T, TU> where TU : OverLayerDefinition<T>
	{
		#region "Attributes"

		private FileInfo m_fileInfo;
		private readonly FieldInfo m_definitionsContainerField;

		#endregion

		#region "Constructors and Initializers"

		protected SerializableDefinitionsManager()
		{
			m_fileInfo = null;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();
		}

		protected SerializableDefinitionsManager(IEnumerable<T> baseDefinitions)
			: base(baseDefinitions)
		{
			m_fileInfo = null;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();
		}

		#endregion

		#region "Properties"

		public FileInfo FileInfo
		{
			get { return m_fileInfo; }
			set { m_fileInfo = value; }
		}

		#endregion

		#region "Methods"

		#region "Static"

		public static FileInfo GetContentDataFile(string configName)
		{
			string filePath = Path.Combine(Path.Combine(GameInstallationInfo.GamePath, @"Content\Data"), configName);
			FileInfo saveFileInfo = new FileInfo(filePath);

			return saveFileInfo;
		}

		#endregion

		#region "Serializers"

		public static T1 LoadContentFile<T1, TS>(FileInfo fileInfo) where TS : XmlSerializer1
		{
			object fileContent;

			string filePath = fileInfo.FullName;

			if (!File.Exists(filePath))
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing, filePath);
			}

			try
			{
				fileContent = ReadSpaceEngineersFile<T1, TS>(filePath);
			}
			catch
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted, filePath);
			}

			if (fileContent == null)
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty, filePath);
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.

			return (T1)fileContent;
		}

		public static void SaveContentFile<T1, TS>(T1 fileContent, FileInfo fileInfo) where TS : XmlSerializer1
		{

			string filePath = fileInfo.FullName;

			//if (!File.Exists(filePath))
			//{
			//	throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing, filePath);
			//}

			try
			{
				WriteSpaceEngineersFile<T1, TS>(fileContent, filePath);
			}
			catch
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted, filePath);
			}

			if (fileContent == null)
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty, filePath);
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.
		}

		public static T1 ReadSpaceEngineersFile<T1, TS>(string filename)
			where TS : XmlSerializer1
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = true,
				IgnoreWhitespace = true,
			};

			object obj = null;

			if (File.Exists(filename))
			{
				using (XmlReader xmlReader = XmlReader.Create(filename, settings))
				{
					TS serializer = (TS)Activator.CreateInstance(typeof(TS));
					obj = serializer.Deserialize(xmlReader);
				}
			}

			return (T1)obj;
		}

		protected T1 Deserialize<T1>(string xml)
		{
			using (StringReader textReader = new StringReader(xml))
			{
				return (T1)(new XmlSerializerContract().GetSerializer(typeof(T1)).Deserialize(textReader));
			}
		}

		protected string Serialize<T1>(object item)
		{
			using (StringWriter textWriter = new StringWriter())
			{
				new XmlSerializerContract().GetSerializer(typeof(T1)).Serialize(textWriter, item);
				return textWriter.ToString();
			}
		}

		public static bool WriteSpaceEngineersFile<T1, TS>(T1 sector, string filename)
			where TS : XmlSerializer1
		{
			// How they appear to be writing the files currently.
			try
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(filename, null))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					TS serializer = (TS)Activator.CreateInstance(typeof(TS));
					serializer.Serialize(xmlTextWriter, sector);
				}
			}
			catch
			{
				return false;
			}

			//// How they should be doing it to support Unicode.
			//var settingsDestination = new XmlWriterSettings()
			//{
			//    Indent = true, // Set indent to false to compress.
			//    Encoding = new UTF8Encoding(false)   // codepage 65001 without signature. Removes the Byte Order Mark from the start of the file.
			//};

			//try
			//{
			//    using (var xmlWriter = XmlWriter.Create(filename, settingsDestination))
			//    {
			//        S serializer = (S)Activator.CreateInstance(typeof(S));
			//        serializer.Serialize(xmlWriter, sector);
			//    }
			//}
			//catch (Exception ex)
			//{
			//    return false;
			//}

			return true;
		}

		#endregion

		protected override TU CreateOverLayerSubTypeInstance(T definition)
		{
			return (TU)Activator.CreateInstance(typeof(TU), new object[] { definition });
		}

		protected override T GetBaseTypeOf(TU overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(TU overLayer)
		{
			return overLayer.Changed;
		}

		private static FieldInfo GetMatchingDefinitionsContainerField()
		{
			//Find the the matching field in the container
			Type thisType = typeof(T[]);
			Type defType = typeof(MyObjectBuilder_Definitions);
			FieldInfo matchingField = null;
			foreach (FieldInfo field in defType.GetFields())
			{
				Type fieldType = field.FieldType;
				if (thisType.FullName == fieldType.FullName)
				{
					matchingField = field;
					break;
				}
			}

			return matchingField;
		}

		public void Load(FileInfo sourceFile)
		{
			m_fileInfo = sourceFile;

			//Get the definitions content from the file
			MyObjectBuilder_Definitions definitionsContainer = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(m_fileInfo);

			if (m_definitionsContainerField == null)
				throw new SEConfigurationException(SEConfigurationExceptionState.InvalidConfigurationFile, "Failed to find matching definitions field in the specified file.");

			//Get the data from the definitions container
			T[] baseDefinitions = (T[])m_definitionsContainerField.GetValue(definitionsContainer);

			//Copy the data into the manager
			GetInternalData().Clear();
			foreach (T definition in baseDefinitions)
			{
				NewEntry(definition);
			}
		}

		public void Load(T[] source)
		{
			//Copy the data into the manager
			GetInternalData().Clear();
			foreach (T definition in source)
			{
				NewEntry(definition);
			}
		}

		public void Load(TU[] source)
		{
			//Copy the data into the manager
			GetInternalData().Clear();
			foreach (TU definition in source)
			{
				NewEntry(definition.BaseDefinition);
			}
		}

		public bool Save()
		{
			if (!Changed) return false;
			if (!IsMutable) return false;
			if (FileInfo == null) return false;

			MyObjectBuilder_Definitions definitionsContainer = new MyObjectBuilder_Definitions();

			if (m_definitionsContainerField == null)
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.Invalid, "Failed to find matching definitions field in the given file.");

			//Save the source data into the definitions container
			m_definitionsContainerField.SetValue(definitionsContainer, ExtractBaseDefinitions().ToArray());

			//Save the definitions container out to the file
			SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(definitionsContainer, m_fileInfo);

			return true;
		}

		#endregion
	}
}
