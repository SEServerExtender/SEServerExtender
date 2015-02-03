namespace SEModAPIInternal.API.Entity
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Microsoft.Xml.Serialization.GeneratedAssembly;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Definitions;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.Support;

	[KnownType( typeof( BaseEntity ) )]
	[KnownType( typeof( BlockGroup ) )]
	[KnownType( typeof( ConveyorLine ) )]
	[KnownType( typeof( CubeBlockEntity ) )]
	[KnownType( typeof( Event ) )]
	[KnownType( typeof( InventoryEntity ) )]
	[KnownType( typeof( InventoryItemEntity ) )]
	[KnownType( typeof( SectorEntity ) )]
	[DataContract]
	public class BaseObject : IDisposable
	{
		#region "Attributes"

		protected MyObjectBuilder_Base MObjectBuilder;
		protected MyDefinitionId MDefinitionId;
		protected MyDefinitionBase MDefinition;
		protected Object MBackingObject;

		protected bool MIsDisposed = false;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public BaseObject( )
		{
		}

		public BaseObject( MyObjectBuilder_Base baseEntity )
		{
			MObjectBuilder = baseEntity;

			MDefinitionId = new MyDefinitionId( MObjectBuilder.TypeId, MObjectBuilder.SubtypeId.ToString( ) );
			if ( !string.IsNullOrEmpty( MObjectBuilder.SubtypeName ) )
			{
				try
				{
					MDefinition = MyDefinitionManager.Static.GetDefinition( MDefinitionId );
				}
				catch ( Exception )
				{
					//Do nothing!
				}
			}
		}

		public BaseObject( MyObjectBuilder_Base baseEntity, Object backingObject )
		{
			MObjectBuilder = baseEntity;
			MBackingObject = backingObject;

			MDefinitionId = new MyDefinitionId( MObjectBuilder.TypeId, MObjectBuilder.SubtypeId.ToString( ) );
			if ( !string.IsNullOrEmpty( MObjectBuilder.SubtypeName ) )
			{
				try
				{
					MDefinition = MyDefinitionManager.Static.GetDefinition( MDefinitionId );
				}
				catch ( Exception )
				{
					//Do nothing!
				}
			}
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		/// <summary>
		/// Changed status of the object
		/// </summary>
		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[Description( "Represent the state of changes in the object" )]
		public virtual bool Changed { get; protected set; }

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[DataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[Description( "The formatted name of the object" )]
		public virtual string Name { get; private set; }

		/// <summary>
		/// Object builder data of the object
		/// </summary>
		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[Description( "Object builder data of the object" )]
		internal MyObjectBuilder_Base ObjectBuilder
		{
			get { return MObjectBuilder; }
			set
			{
				if ( MObjectBuilder == value ) return;
				MObjectBuilder = value;

				Changed = true;
			}
		}

		/// <summary>
		/// Internal, in-game object that matches to this object
		/// </summary>
		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[Description( "Internal, in-game object that matches to this object" )]
		public Object BackingObject
		{
			get { return MBackingObject; }
			set
			{
				MBackingObject = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Full type of the object
		/// </summary>
		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The ID representing the definition type of the object" )]
		public MyDefinitionId Id
		{
			get
			{
				return MDefinitionId;
			}
		}

		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The definition type of the object" )]
		public MyDefinitionBase Definition
		{
			get
			{
				return MDefinition;
			}
		}

		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[Description( "The value ID representing the type of the object" )]
		[Obsolete]
		public MyObjectBuilderType TypeId
		{
			get { return MObjectBuilder.TypeId; }
		}

		[IgnoreDataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[Description( "The value ID representing the sub-type of the object" )]
		[Obsolete]
		public string Subtype { get { return MObjectBuilder.SubtypeName; } }

		[DataMember]
		[Category( "Object" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public bool IsDisposed
		{
			get { return MIsDisposed; }
		}

		#endregion "Properties"

		#region "Methods"

		public virtual void Dispose( )
		{
			if ( IsDisposed )
				return;

			if ( BackingObject != null )
			{
				//Do stuff
			}

			MIsDisposed = true;
		}

		public virtual void Export( FileInfo fileInfo )
		{
			BaseObjectManager.SaveContentFile<MyObjectBuilder_Base, MyObjectBuilder_BaseSerializer>( ObjectBuilder, fileInfo );
		}

		public MyObjectBuilder_Base Export( )
		{
			return ObjectBuilder;
		}

		public static bool ReflectionUnitTest( )
		{
			return true;
		}

		#region "Internal"

		public static bool HasField( Type objectType, string fieldName )
		{
			try
			{
				if ( string.IsNullOrEmpty( fieldName ) )
					return false;
				FieldInfo field = ( objectType.GetField( fieldName ) ??
				                    objectType.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy ) ) ??
				                  objectType.BaseType.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );
				if ( field == null )
				{
					if ( SandboxGameAssemblyWrapper.IsDebugging )
						LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find field '{0}' in type '{1}'", fieldName, objectType.FullName ) );
					return false;
				}
				return true;
			}
			catch ( Exception ex )
			{
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find field '{0}' in type '{1}': {2}", fieldName, objectType.FullName, ex.Message ) );
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		public static bool HasMethod( Type objectType, string methodName )
		{
			return HasMethod( objectType, methodName, null );
		}

		public static bool HasMethod( Type objectType, string methodName, Type[ ] argTypes )
		{
			try
			{
				if ( string.IsNullOrEmpty( methodName ) )
					return false;

				if ( argTypes == null )
				{
					MethodInfo method = objectType.GetMethod( methodName ) ??
					                    objectType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );
					if ( method == null && objectType.BaseType != null )
						method = objectType.BaseType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );
					if ( method == null )
					{
						if ( SandboxGameAssemblyWrapper.IsDebugging )
							LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find method '{0}' in type '{1}'", methodName, objectType.FullName ) );
						return false;
					}
				}
				else
				{
					MethodInfo method = objectType.GetMethod( methodName, argTypes ) ??
					                    objectType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null );
					if ( method == null && objectType.BaseType != null )
						method = objectType.BaseType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null );
					if ( method == null )
					{
						if ( SandboxGameAssemblyWrapper.IsDebugging )
							LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find method '{0}' in type '{1}'", methodName, objectType.FullName ) );
						return false;
					}
				}

				return true;
			}
			catch ( AmbiguousMatchException aex )
			{
				return true;
			}
			catch ( Exception ex )
			{
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find method '{0}' in type '{1}': {2}", methodName, objectType.FullName, ex.Message ) );
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		public static bool HasProperty( Type objectType, string propertyName )
		{
			try
			{
				if ( string.IsNullOrEmpty( propertyName ) )
					return false;
				PropertyInfo property = ( objectType.GetProperty( propertyName ) ??
				                          objectType.GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy ) ) ??
				                        objectType.BaseType.GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );
				if ( property == null )
				{
					if ( SandboxGameAssemblyWrapper.IsDebugging )
						LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find property '{0}' in type '{1}'", propertyName, objectType.FullName ) );
					return false;
				}
				return true;
			}
			catch ( Exception ex )
			{
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find property '{0}' in type '{1}': {2}", propertyName, objectType.FullName, ex.Message ) );
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		public static bool HasNestedType( Type objectType, string nestedTypeName )
		{
			try
			{
				if ( string.IsNullOrEmpty( nestedTypeName ) )
					return false;

				Type type = objectType.GetNestedType( nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic );
				return true;
			}
			catch ( Exception ex )
			{
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLineAndConsole( string.Format( "Failed to find nested type '{0}' in type '{1}': {2}", nestedTypeName, objectType.FullName, ex.Message ) );

				LogManager.ErrorLog.WriteLine( ex );
				return false;

			}
		}

		public static FieldInfo GetStaticField( Type objectType, string fieldName )
		{
			try
			{
				FieldInfo field = objectType.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy ) ??
				                  objectType.BaseType.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy );
				return field;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get static field '{0}'", fieldName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static FieldInfo GetEntityField( Object gameEntity, string fieldName )
		{
			try
			{
				FieldInfo field = gameEntity.GetType( ).GetField( fieldName );
				if ( field == null )
				{
					//Recurse up through the class heirarchy to try to find the field
					Type type = gameEntity.GetType( );
					while ( type != typeof( Object ) )
					{
						field = type.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );
						if ( field != null )
							break;

						type = type.BaseType;
					}
				}
				return field;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get entity field '{0}'", fieldName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static MethodInfo GetStaticMethod( Type objectType, string methodName )
		{
			try
			{
				if ( string.IsNullOrEmpty( methodName ) )
					throw new ArgumentException( "Method name was empty", "methodName" );
				MethodInfo method = objectType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy );
				if ( method == null )
				{
					//Recurse up through the class heirarchy to try to find the method
					Type type = objectType;
					while ( type != typeof( Object ) )
					{
						method = type.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy );
						if ( method != null )
							break;

						type = type.BaseType;
					}
				}
				if ( method == null )
					throw new ArgumentException( "Method not found", "methodName" );
				return method;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get static method '{0}'", methodName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static MethodInfo GetStaticMethod( Type objectType, string methodName, Type[ ] argTypes )
		{
			try
			{
				if ( argTypes == null || argTypes.Length == 0 )
					return GetStaticMethod( objectType, methodName );

				if ( string.IsNullOrEmpty( methodName ) )
					throw new ArgumentException( "Method name was empty", "methodName" );
				MethodInfo method = objectType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null );
				if ( method == null )
				{
					//Recurse up through the class hierarchy to try to find the method
					Type type = objectType;
					while ( type != typeof( Object ) )
					{
						method = type.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null );
						if ( method != null )
							break;

						type = type.BaseType;
					}
				}
				if ( method == null )
					throw new ArgumentException( "Method not found", "methodName" );
				return method;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get static method '{0}'", methodName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static MethodInfo GetEntityMethod( Object gameEntity, string methodName )
		{
			try
			{
				if ( gameEntity == null )
					throw new ArgumentException( "Game entity was null", "gameEntity" );
				if ( string.IsNullOrEmpty( methodName ) )
					throw new ArgumentException( "Method name was empty", "methodName" );
				MethodInfo method = gameEntity.GetType( ).GetMethod( methodName );
				if ( method == null )
				{
					//Recurse up through the class hierarchy to try to find the method
					Type type = gameEntity.GetType( );
					while ( type != typeof( Object ) )
					{
						method = type.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );
						if ( method != null )
							break;

						type = type.BaseType;
					}
				}
				if ( method == null )
					throw new ArgumentException( "Method not found", "methodName" );
				return method;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get entity method '{0}': {1}", methodName, ex.Message ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static MethodInfo GetEntityMethod( Object gameEntity, string methodName, Type[ ] argTypes )
		{
			try
			{
				if ( argTypes == null || argTypes.Length == 0 )
					return GetEntityMethod( gameEntity, methodName );

				if ( gameEntity == null )
					throw new ArgumentException( "Game entity was null", "gameEntity" );
				if ( string.IsNullOrEmpty( methodName ) )
					throw new ArgumentException( "Method name was empty", "methodName" );
				MethodInfo method = gameEntity.GetType( ).GetMethod( methodName, argTypes );
				if ( method == null )
				{
					//Recurse up through the class heirarchy to try to find the method
					Type type = gameEntity.GetType( );
					while ( type != typeof( Object ) )
					{
						method = type.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null );
						if ( method != null )
							break;

						type = type.BaseType;
					}
				}
				if ( method == null )
					throw new ArgumentException( "Method not found", "methodName" );
				return method;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get entity method '{0}': {1}", methodName, ex.Message ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static Object GetStaticFieldValue( Type objectType, string fieldName )
		{
			try
			{
				FieldInfo field = GetStaticField( objectType, fieldName );
				if ( field == null )
					return null;
				Object value = field.GetValue( null );
				return value;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static void SetStaticFieldValue( Type objectType, string fieldName, Object value )
		{
			try
			{
				FieldInfo field = GetStaticField( objectType, fieldName );
				if ( field == null )
					return;
				field.SetValue( null, value );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		public static object GetEntityFieldValue( Object gameEntity, string fieldName )
		{
			try
			{
				FieldInfo field = GetEntityField( gameEntity, fieldName );
				if ( field == null )
					return null;
				Object value = field.GetValue( gameEntity );
				return value;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static void SetEntityFieldValue( Object gameEntity, string fieldName, Object value )
		{
			try
			{
				FieldInfo field = GetEntityField( gameEntity, fieldName );
				if ( field == null )
					return;
				field.SetValue( gameEntity, value );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		public static Object InvokeStaticMethod( Type objectType, string methodName )
		{
			return InvokeStaticMethod( objectType, methodName, new object[ ] { } );
		}

		public static Object InvokeStaticMethod( Type objectType, string methodName, Object[ ] parameters )
		{
			try
			{
				MethodInfo method = GetStaticMethod( objectType, methodName );
				if ( method == null )
					throw new ArgumentException( "Method is empty", "methodName" );
				Object result = method.Invoke( null, parameters );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to invoke static method '{0}': {1}", methodName, ex.Message ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static Object InvokeEntityMethod( Object gameEntity, string methodName )
		{
			return InvokeEntityMethod( gameEntity, methodName, new object[ ] { } );
		}

		public static Object InvokeEntityMethod( Object gameEntity, string methodName, Object[ ] parameters )
		{
			return InvokeEntityMethod( gameEntity, methodName, parameters, null );
		}

		public static Object InvokeEntityMethod( Object gameEntity, string methodName, Object[ ] parameters, Type[ ] argTypes )
		{
			try
			{
				MethodInfo method = GetEntityMethod( gameEntity, methodName, argTypes );
				if ( method == null )
					throw new ArgumentException( "Method is empty", "methodName" );
				Object result = method.Invoke( gameEntity, parameters );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to invoke entity method '{0}' on type '{1}': {2}", methodName, gameEntity.GetType( ).FullName, ex.Message ) );

				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );

				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static PropertyInfo GetEntityProperty( Object gameEntity, string propertyName )
		{
			try
			{
				PropertyInfo property = gameEntity.GetType( ).GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy ) ??
				                        gameEntity.GetType( ).BaseType.GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy );

				return property;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get entity property '{0}'", propertyName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static Object GetEntityPropertyValue( Object gameEntity, string propertyName )
		{
			try
			{
				PropertyInfo property = GetEntityProperty( gameEntity, propertyName );
				if ( property == null )
					return null;

				Object result = property.GetValue( gameEntity, null );
				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to get entity property value '{0}'", propertyName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		public static void SetEntityPropertyValue( Object gameEntity, string propertyName, Object value )
		{
			try
			{
				PropertyInfo property = GetEntityProperty( gameEntity, propertyName );
				if ( property == null )
					return;

				property.SetValue( gameEntity, value, null );
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( string.Format( "Failed to set entity property value '{0}'", propertyName ) );
				if ( SandboxGameAssemblyWrapper.IsDebugging )
					LogManager.ErrorLog.WriteLine( Environment.StackTrace );
				LogManager.ErrorLog.WriteLine( ex );
				return;
			}
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}
