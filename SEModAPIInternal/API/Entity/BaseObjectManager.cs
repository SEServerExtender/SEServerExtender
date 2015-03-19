namespace SEModAPIInternal.API.Entity
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Xml;
	using System.Xml.Serialization;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Common.ObjectBuilders.Definitions;
	using Sandbox.Common.ObjectBuilders.Serializer;
	using SEModAPI.API;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;
	using VRage;

	[DataContract]
	[KnownType( typeof( SectorObjectManager ) )]
	[KnownType( typeof( InventoryItemManager ) )]
	[KnownType( typeof( CubeBlockManager ) )]
	public class BaseObjectManager
	{
		public enum InternalBackingType
		{
			Hashset,
			List,
			Dictionary,
		}

		#region "Attributes"

		private FileInfo _fileInfo;
		private readonly FieldInfo _definitionsContainerField;
		private Object _backingObject;
		private string _backingSourceMethod;
		private InternalBackingType _backingSourceType;
		private DateTime _lastLoadTime;
		private double _refreshInterval;

		private static double _averageRefreshDataTime;
		private static DateTime _lastProfilingOutput;
		private static DateTime _lastInternalProfilingOutput;

		private static int _staticRefreshCount;
		private static Dictionary<Type, int> _staticRefreshCountMap;

		protected FastResourceLock ResourceLock = new FastResourceLock( );
		protected FastResourceLock RawDataHashSetResourceLock = new FastResourceLock( );
		protected FastResourceLock RawDataListResourceLock = new FastResourceLock( );
		protected FastResourceLock RawDataObjectBuilderListResourceLock = new FastResourceLock( );

		//Flags

		private bool _changed;
		private bool _isDynamic;

		//Raw data stores
		protected HashSet<Object> RawDataHashSet = new HashSet<object>( );

		protected List<Object> RawDataList = new List<object>( );
		protected Dictionary<Object, MyObjectBuilder_Base> RawDataObjectBuilderList = new Dictionary<object, MyObjectBuilder_Base>( );

		//Clean data stores
		private readonly Dictionary<long, BaseObject> _definitions = new Dictionary<long, BaseObject>( );

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BaseObjectManager( )
		{
			_fileInfo = null;
			_changed = false;
			IsMutable = true;

			_definitionsContainerField = GetMatchingDefinitionsContainerField( );

			_backingSourceType = InternalBackingType.Hashset;

			_lastLoadTime = DateTime.Now;

			_lastProfilingOutput = DateTime.Now;
			_lastInternalProfilingOutput = DateTime.Now;

			if ( _staticRefreshCountMap == null )
				_staticRefreshCountMap = new Dictionary<Type, int>( );

			_refreshInterval = 250;
		}

		public BaseObjectManager( Object backingSource, string backingMethodName, InternalBackingType backingSourceType )
		{
			_fileInfo = null;
			_changed = false;
			IsMutable = true;
			_isDynamic = true;

			_definitionsContainerField = GetMatchingDefinitionsContainerField( );

			_backingObject = backingSource;
			_backingSourceMethod = backingMethodName;
			_backingSourceType = backingSourceType;

			_lastLoadTime = DateTime.Now;

			_lastProfilingOutput = DateTime.Now;
			_lastInternalProfilingOutput = DateTime.Now;

			if ( _staticRefreshCountMap == null )
				_staticRefreshCountMap = new Dictionary<Type, int>( );

			_refreshInterval = 250;
		}

		public BaseObjectManager( BaseObject[ ] baseDefinitions )
		{
			_fileInfo = null;
			_changed = false;
			IsMutable = true;

			_definitionsContainerField = GetMatchingDefinitionsContainerField( );

			Load( baseDefinitions );
		}

		public BaseObjectManager( List<BaseObject> baseDefinitions )
		{
			_fileInfo = null;
			_changed = false;
			IsMutable = true;

			_definitionsContainerField = GetMatchingDefinitionsContainerField( );

			Load( baseDefinitions );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public bool IsMutable { get; set; }

		protected bool Changed
		{
			get
			{
				return _changed || GetInternalData( ).Values.Any( def => def.Changed );
			}
		}

		public FileInfo FileInfo
		{
			get { return _fileInfo; }
			set { _fileInfo = value; }
		}

		public bool IsDynamic
		{
			get { return _isDynamic; }
			set { _isDynamic = value; }
		}

		public bool IsResourceLocked
		{
			get { return ResourceLock.Owned; }
		}

		public bool IsInternalResourceLocked
		{
			get { return ( RawDataHashSetResourceLock.Owned || RawDataListResourceLock.Owned || RawDataObjectBuilderListResourceLock.Owned ); }
		}

		public bool CanRefresh
		{
			get
			{
				if ( !IsDynamic )
					return false;
				if ( !IsMutable )
					return false;
				if ( IsResourceLocked )
					return false;
				if ( IsInternalResourceLocked )
					return false;
				if ( !SandboxGameAssemblyWrapper.Instance.IsGameStarted )
					return false;
				if ( WorldManager.Instance.IsWorldSaving )
					return false;
				if ( WorldManager.Instance.InternalGetResourceLock( ) == null )
					return false;
				if ( WorldManager.Instance.InternalGetResourceLock( ).Owned )
					return false;

				return true;
			}
		}

		public int Count
		{
			get { return _definitions.Count; }
		}

		#endregion "Properties"

		#region "Methods"

		public void SetBackingProperties( Object backingObject, string backingMethod, InternalBackingType backingType )
		{
			_isDynamic = true;

			_backingObject = backingObject;
			_backingSourceMethod = backingMethod;
			_backingSourceType = backingType;
		}

		private FieldInfo GetMatchingDefinitionsContainerField( )
		{
			//Find the the matching field in the container
			Type thisType = typeof( MyObjectBuilder_Base[ ] );
			Type defType = typeof( MyObjectBuilder_Definitions );
			FieldInfo matchingField = null;
			foreach ( FieldInfo field in defType.GetFields( ) )
			{
				Type fieldType = field.FieldType;
				if ( thisType.FullName == fieldType.FullName )
				{
					matchingField = field;
					break;
				}
			}

			return matchingField;
		}

		protected virtual bool IsValidEntity( Object entity )
		{
			return true;
		}

		#region "GetDataSource"

		protected Dictionary<long, BaseObject> GetInternalData( )
		{
			return _definitions;
		}

		protected HashSet<Object> GetBackingDataHashSet( )
		{
			return RawDataHashSet;
		}

		protected List<Object> GetBackingDataList( )
		{
			return RawDataList;
		}

		protected Dictionary<object, MyObjectBuilder_Base> GetObjectBuilderMap( )
		{
			return RawDataObjectBuilderList;
		}

		#endregion "GetDataSource"

		#region "RefreshDataSource"

		/// <exception cref="KeyNotFoundException">Thrown if GetType returns a type not found in the <see cref="_staticRefreshCountMap"/> dictionary.</exception>
		public void Refresh( )
		{
			if ( !CanRefresh )
				return;

			TimeSpan timeSinceLastLoad = DateTime.Now - _lastLoadTime;
			if ( timeSinceLastLoad.TotalMilliseconds < _refreshInterval )
				return;
			_lastLoadTime = DateTime.Now;

			//Run the refresh
			Action action = RefreshData;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );

			//Update the refresh counts
			if ( !_staticRefreshCountMap.ContainsKey( GetType( ) ) )
				_staticRefreshCountMap.Add( GetType( ), 1 );
			else
				_staticRefreshCountMap[ GetType( ) ]++;
			int typeRefreshCount = _staticRefreshCountMap[ GetType( ) ];
			_staticRefreshCount++;

			//Adjust the refresh interval based on percentage of total refreshes for this type
			_refreshInterval = ( typeRefreshCount / _staticRefreshCount ) * 850 + 250;
		}

		private void RefreshData( )
		{
			if ( !CanRefresh )
				return;

			try
			{
				DateTime startRefreshTime = DateTime.Now;

				if ( _backingSourceType == InternalBackingType.Hashset )
					InternalRefreshBackingDataHashSet( );
				if ( _backingSourceType == InternalBackingType.List )
					InternalRefreshBackingDataList( );

				//Lock the main data
				ResourceLock.AcquireExclusive( );

				//Lock all of the raw data
				if ( _backingSourceType == InternalBackingType.Hashset )
					RawDataHashSetResourceLock.AcquireShared( );
				if ( _backingSourceType == InternalBackingType.List )
					RawDataListResourceLock.AcquireShared( );
				RawDataObjectBuilderListResourceLock.AcquireShared( );

				//Refresh the main data
				LoadDynamic( );

				//Unlock the main data
				ResourceLock.ReleaseExclusive( );

				//Unlock all of the raw data
				if ( _backingSourceType == InternalBackingType.Hashset )
					RawDataHashSetResourceLock.ReleaseShared( );
				if ( _backingSourceType == InternalBackingType.List )
					RawDataListResourceLock.ReleaseShared( );
				RawDataObjectBuilderListResourceLock.ReleaseShared( );

				if ( SandboxGameAssemblyWrapper.IsDebugging )
				{
					TimeSpan timeToRefresh = DateTime.Now - startRefreshTime;
					_averageRefreshDataTime = ( _averageRefreshDataTime + timeToRefresh.TotalMilliseconds ) / 2;
					TimeSpan timeSinceLastProfilingOutput = DateTime.Now - _lastProfilingOutput;
					if ( timeSinceLastProfilingOutput.TotalSeconds > 30 )
					{
						_lastProfilingOutput = DateTime.Now;
						ApplicationLog.BaseLog.Debug( string.Format( "ObjectManager - Average of {0}ms to refresh API data", Math.Round( _averageRefreshDataTime, 2 ) ) );
					}
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected virtual void LoadDynamic( )
		{
			return;
		}

		protected virtual void InternalRefreshBackingDataHashSet( )
		{
			try
			{
				if ( !CanRefresh )
					return;

				RawDataHashSetResourceLock.AcquireExclusive( );

				if ( _backingObject == null )
					return;
				object rawValue = BaseObject.InvokeEntityMethod( _backingObject, _backingSourceMethod );
				if ( rawValue == null )
					return;

				//Create/Clear the hash set
				if ( RawDataHashSet == null )
					RawDataHashSet = new HashSet<object>( );
				else
					RawDataHashSet.Clear( );

				//Only allow valid entities in the hash set
				foreach ( object entry in UtilityFunctions.ConvertHashSet( rawValue ) )
				{
					if ( !IsValidEntity( entry ) )
						continue;

					RawDataHashSet.Add( entry );
				}

				RawDataHashSetResourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				if ( RawDataHashSetResourceLock.Owned )
					RawDataHashSetResourceLock.ReleaseExclusive( );
			}
		}

		protected virtual void InternalRefreshBackingDataList( )
		{
			try
			{
				if ( !CanRefresh )
					return;

				RawDataListResourceLock.AcquireExclusive( );

				if ( _backingObject == null )
					return;
				object rawValue = BaseObject.InvokeEntityMethod( _backingObject, _backingSourceMethod );
				if ( rawValue == null )
					return;

				//Create/Clear the list
				if ( RawDataList == null )
					RawDataList = new List<object>( );
				else
					RawDataList.Clear( );

				//Only allow valid entities in the list
				foreach ( object entry in UtilityFunctions.ConvertList( rawValue ) )
				{
					if ( !IsValidEntity( entry ) )
						continue;

					RawDataList.Add( entry );
				}

				RawDataListResourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				if ( RawDataListResourceLock.Owned )
					RawDataListResourceLock.ReleaseExclusive( );
			}
		}

		#endregion "RefreshDataSource"

		#region "Static"

		/// <exception cref="SecurityException">The caller does not have the required permission. </exception>
		/// <exception cref="UnauthorizedAccessException">Access to <paramref name="configName" /> is denied. </exception>
		/// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
		public static FileInfo GetContentDataFile( string configName )
		{
			string filePath = Path.Combine( Path.Combine( GameInstallationInfo.GamePath, @"Content\Data" ), configName );
			FileInfo saveFileInfo = new FileInfo( filePath );

			return saveFileInfo;
		}

		#endregion "Static"

		#region "Serializers"

		/// <exception cref="ConfigurationErrorsException">Configuration file empty, corrupted, or not found.</exception>
		public static T LoadContentFile<T, TS>( FileInfo fileInfo ) where TS : XmlSerializer1
		{
			object fileContent = null;

			string filePath = fileInfo.FullName;

			if ( !File.Exists( filePath ) )
			{
				throw new ConfigurationErrorsException( "Configuration file not found." );
			}

			try
			{
				fileContent = ReadSpaceEngineersFile<T, TS>( filePath );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				throw new ConfigurationErrorsException( "Configuration file corrupted.", ex );
			}

			if ( fileContent == null )
			{
				throw new ConfigurationErrorsException( "Configuration file empty." );
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.

			return (T)fileContent;
		}

		/// <exception cref="ConfigurationErrorsException">Error writing configuration file.  Configuration file possibly corrupted.</exception>
		public static void SaveContentFile<T, TS>( T fileContent, FileInfo fileInfo ) where TS : XmlSerializer1
		{
			string filePath = fileInfo.FullName;

			//if (!File.Exists(filePath))
			//{
			//	throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing, filePath);
			//}

			try
			{
				WriteSpaceEngineersFile<T, TS>( fileContent, filePath );
			}
			catch ( Exception exception )
			{
				throw new ConfigurationErrorsException( "Error writing configuration file.  Configuration file possibly corrupted.", exception );
			}

			if ( fileContent == null )
			{
				throw new ConfigurationErrorsException( "Configuration file empty." );
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.
		}

		/// <exception cref="FileNotFoundException">The file specified cannot be found.</exception>
		/// <exception cref="UriFormatException">The file location format is not correct.</exception>
		/// <exception cref="MethodAccessException">The caller does not have permission to call this constructor. </exception>
		/// <exception cref="TypeLoadException"><typeparamref name="T"/> is not a valid type. </exception>
		/// <exception cref="MissingMethodException">No matching public constructor was found. </exception>
		/// <exception cref="MemberAccessException">Cannot create an instance of an abstract class, or this member was invoked with a late-binding mechanism. </exception>
		/// <exception cref="TargetInvocationException">The constructor being called throws an exception. </exception>
		/// <exception cref="COMException"><typeparamref name="T" /> is a COM object but the class identifier used to obtain the type is invalid, or the identified class is not registered. </exception>
		/// <exception cref="InvalidComObjectException">The COM type was not obtained through <see cref="Overload:System.Type.GetTypeFromProgID" /> or <see cref="Overload:System.Type.GetTypeFromCLSID" />. </exception>
		/// <exception cref="NotSupportedException"><typeparamref name="T" /> cannot be a <see cref="T:System.Reflection.Emit.TypeBuilder" />.-or- Creation of <see cref="T:System.TypedReference" />, <see cref="T:System.ArgIterator" />, <see cref="T:System.Void" />, and <see cref="T:System.RuntimeArgumentHandle" /> types, or arrays of those types, is not supported.-or-The assembly that contains <typeparamref name="T" /> is a dynamic assembly that was created with <see cref="F:System.Reflection.Emit.AssemblyBuilderAccess.Save" />. </exception>
		/// <exception cref="ArgumentException"><typeparamref name="T" /> is not a RuntimeType. -or-<typeparamref name="T" /> is an open generic type (that is, the <see cref="P:System.Type.ContainsGenericParameters" /> property returns true).</exception>
		public static T ReadSpaceEngineersFile<T, TS>( string filename )
			where TS : XmlSerializer1
		{
			XmlReaderSettings settings = new XmlReaderSettings
			                             {
				                             IgnoreComments = true,
				                             IgnoreWhitespace = true,
			                             };

			object obj = null;

			if ( File.Exists( filename ) )
			{
				using ( XmlReader xmlReader = XmlReader.Create( filename, settings ) )
				{
					TS serializer = (TS)Activator.CreateInstance( typeof( TS ) );
					obj = serializer.Deserialize( xmlReader );
				}
			}

			return (T)obj;
		}

		/// <exception cref="InvalidOperationException">An error occurred during deserialization.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="xml" /> parameter is null. </exception>
		protected T Deserialize<T>( string xml )
		{
			using ( StringReader textReader = new StringReader( xml ) )
			{
				XmlSerializer xmlSerializer = new XmlSerializerContract( ).GetSerializer( typeof( T ) );
				if ( xmlSerializer != null )
				{
					return (T)( xmlSerializer.Deserialize( textReader ) );
				}
			}
			throw new InvalidOperationException("Failed to deserialize XML.");
		}

		protected string Serialize<T>( object item )
		{
			using ( StringWriter textWriter = new StringWriter( ) )
			{
				new XmlSerializerContract( ).GetSerializer( typeof( T ) ).Serialize( textWriter, item );
				return textWriter.ToString( );
			}
		}

		public static bool WriteSpaceEngineersFile<T, TS>( T sector, string filename )
			where TS : XmlSerializer1
		{
			// How they appear to be writing the files currently.
			try
			{
				using ( XmlTextWriter xmlTextWriter = new XmlTextWriter( filename, null ) )
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					TS serializer = (TS)Activator.CreateInstance( typeof( TS ) );
					serializer.Serialize( xmlTextWriter, sector );
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

		#endregion "Serializers"

		#region "GetContent"

		public BaseObject GetEntry( long key )
		{
			if ( !GetInternalData( ).ContainsKey( key ) )
				return null;

			return GetInternalData( )[ key ];
		}

		public List<T> GetTypedInternalData<T>( ) where T : BaseObject
		{
			try
			{
				ResourceLock.AcquireShared( );

				List<T> newList = GetInternalData( ).Values.OfType<T>( ).ToList( );

				ResourceLock.ReleaseShared( );

				Refresh( );

				return newList;
			}
			catch ( KeyNotFoundException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				if ( ResourceLock.Owned )
					ResourceLock.ReleaseShared( );
				return new List<T>( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				if ( ResourceLock.Owned )
					ResourceLock.ReleaseShared( );
				return new List<T>( );
			}
		}

		#endregion "GetContent"

		#region "NewContent"

		public T NewEntry<T>( ) where T : BaseObject
		{
			if ( !IsMutable ) return default( T );
			MyObjectBuilder_Base newBase = MyObjectBuilderSerializer.CreateNewObject( typeof( MyObjectBuilder_EntityBase ) );
			T newEntry = (T)Activator.CreateInstance( typeof( T ), newBase );
			GetInternalData( ).Add( _definitions.Count, newEntry );
			_changed = true;

			return newEntry;
		}

		[Obsolete("Will be removed in version 0.3.2.0")]
		public T NewEntry<T>( MyObjectBuilder_Base source ) where T : BaseObject
		{
			if ( !IsMutable ) return default( T );

			T newEntry = (T)Activator.CreateInstance( typeof( T ), source );
			GetInternalData( ).Add( _definitions.Count, newEntry );
			_changed = true;

			return newEntry;
		}

		public T NewEntry<T>( T source ) where T : BaseObject
		{
			if ( !IsMutable ) return default( T );

			T newEntry = (T)Activator.CreateInstance( typeof( T ), source.ObjectBuilder );
			GetInternalData( ).Add( _definitions.Count, newEntry );
			_changed = true;

			return newEntry;
		}

		public void AddEntry<T>( long key, T entry ) where T : BaseObject
		{
			if ( !IsMutable ) return;

			GetInternalData( ).Add( key, entry );
			_changed = true;
		}

		#endregion "NewContent"

		#region "DeleteContent"

		public bool DeleteEntry( long id )
		{
			if ( !IsMutable ) return false;

			if ( GetInternalData( ).ContainsKey( id ) )
			{
				BaseObject entry = GetInternalData( )[ id ];
				GetInternalData( ).Remove( id );
				entry.Dispose( );
				_changed = true;
				return true;
			}

			return false;
		}

		public bool DeleteEntry( BaseObject entry )
		{
			if ( !IsMutable ) return false;

			foreach ( KeyValuePair<long, BaseObject> def in _definitions )
			{
				if ( def.Value.Equals( entry ) )
				{
					DeleteEntry( def.Key );
					break;
				}
			}

			return false;
		}

		public bool DeleteEntries<T>( List<T> entries ) where T : BaseObject
		{
			if ( !IsMutable ) return false;

			foreach ( T entry in entries )
			{
				DeleteEntry( entry );
			}

			return true;
		}

		public bool DeleteEntries<T>( Dictionary<long, T> entries ) where T : BaseObject
		{
			if ( !IsMutable ) return false;

			foreach ( long entry in entries.Keys )
			{
				DeleteEntry( entry );
			}

			return true;
		}

		#endregion "DeleteContent"

		#region "LoadContent"

		public void Load<T>( T[ ] source ) where T : BaseObject
		{
			//Copy the data into the manager
			GetInternalData( ).Clear( );
			foreach ( T definition in source )
			{
				GetInternalData( ).Add( GetInternalData( ).Count, definition );
			}
		}

		public void Load<T>( List<T> source ) where T : BaseObject
		{
			Load( source.ToArray( ) );
		}

		#endregion "LoadContent"

		#region "SaveContent"

		/// <exception cref="ConfigurationErrorsException">Error saving configuration.</exception>
		/// <exception cref="FieldAccessException">The caller does not have permission to access a field on a reflected object.</exception>
		public bool Save( )
		{
			if ( !Changed ) return false;
			if ( !IsMutable ) return false;
			if ( FileInfo == null ) return false;

			MyObjectBuilder_Definitions definitionsContainer = new MyObjectBuilder_Definitions( );

			if ( _definitionsContainerField == null )
				throw new ConfigurationErrorsException( "Error saving configuration.", new ConfigurationErrorsException( "Failed to find matching definitions field in the given file." ) );

			List<MyObjectBuilder_Base> baseDefs = new List<MyObjectBuilder_Base>( );
			foreach ( BaseObject baseObject in GetInternalData( ).Values )
			{
				baseDefs.Add( baseObject.ObjectBuilder );
			}

			//Save the source data into the definitions container
			_definitionsContainerField.SetValue( definitionsContainer, baseDefs.ToArray( ) );

			//Save the definitions container out to the file
			SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>( definitionsContainer, _fileInfo );

			return true;
		}

		#endregion "SaveContent"

		#endregion "Methods"
	}
}