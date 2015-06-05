namespace SEModAPI.API.Utility
{
	using System;
	using System.Reflection;
	using NLog;

	public static class Reflection
	{
		private static readonly Logger Log = LogManager.GetLogger( "BaseLog" );
		public static dynamic GetEntityFieldValue( Object gameEntity, string fieldName, bool suppressErrors = false )
		{
			try
			{
				FieldInfo field = GetEntityField( gameEntity, fieldName, suppressErrors );
				if ( field == null )
					return null;
				dynamic value = field.GetValue( gameEntity );
				return value;
			}
			catch ( Exception ex )
			{
				if ( !suppressErrors )
					Log.Error( ex );
				return null;
			}
		}

		public static FieldInfo GetEntityField( Object gameEntity, string fieldName, bool suppressErrors = false )
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
				if ( !suppressErrors )
				{
					Log.Error( "Failed to get entity field '{0}'", fieldName );
					Log.Error( ex );
				}
				return null;
			}
		}

	}
}
