namespace SEModAPIInternal.API.Utility
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.Support;
	using VRage;
	using VRage.ObjectBuilders;
	using VRage.Utils;
	using VRageMath;

	public class UtilityFunctions
	{
		#region "Attributes"

		public static string UtilityNamespace = "";
		//public static string UtilityClass = "226D9974B43A7269CDD3E322CC8110D5";
		//public static string UtilityGenerateEntityIdMethod = "3B4924802BEBD1AE13B29920376CE914";

		#endregion "Attributes"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				//Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(UtilityNamespace, UtilityClass);
				//if (type == null)
				//	throw new Exception("Could not find internal type for UtilityFunctions");
				//bool result = true;
				//result &= BaseObject.HasMethod(type, UtilityGenerateEntityIdMethod);
				//return result;
				return true;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public static HashSet<Object> ConvertHashSet( Object source )
		{
			try
			{
				Type rawType = source.GetType( );
				Type[ ] genericArgs = rawType.GetGenericArguments( );
				MethodInfo conversion = typeof( UtilityFunctions ).GetMethod( "ConvertEntityHashSet", BindingFlags.Public | BindingFlags.Static );
				conversion = conversion.MakeGenericMethod( genericArgs[ 0 ] );
				HashSet<Object> result = (HashSet<Object>)conversion.Invoke( null, new[ ] { source } );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new HashSet<object>( );
			}
		}

		public static List<Object> ConvertList( Object source )
		{
			try
			{
				Type rawType = source.GetType( );
				Type[ ] genericArgs = rawType.GetGenericArguments( );
				MethodInfo conversion = typeof( UtilityFunctions ).GetMethod( "ConvertEntityList", BindingFlags.Public | BindingFlags.Static );
				conversion = conversion.MakeGenericMethod( genericArgs[ 0 ] );
				List<Object> result = (List<Object>)conversion.Invoke( null, new[ ] { source } );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new List<object>( );
			}
		}

		public static Dictionary<T, Object> ConvertDictionary<T>( Object source )
		{
			try
			{
				Type rawType = source.GetType( );
				Type[ ] genericArgs = rawType.GetGenericArguments( );
				MethodInfo conversion = typeof( UtilityFunctions ).GetMethod( "ConvertEntityDictionary", BindingFlags.Public | BindingFlags.Static );
				conversion = conversion.MakeGenericMethod( genericArgs );
				Dictionary<T, Object> result = (Dictionary<T, Object>)conversion.Invoke( null, new[ ] { source } );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new Dictionary<T, Object>( );
			}
		}

		public static Dictionary<Object, T> ConvertDictionaryReverse<T>( Object source )
		{
			try
			{
				Type rawType = source.GetType( );
				Type[ ] genericArgs = rawType.GetGenericArguments( );
				MethodInfo conversion = typeof( UtilityFunctions ).GetMethod( "ConvertEntityDictionaryReverse", BindingFlags.Public | BindingFlags.Static );
				conversion = conversion.MakeGenericMethod( genericArgs );
				Dictionary<Object, T> result = (Dictionary<Object, T>)conversion.Invoke( null, new[ ] { source } );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new Dictionary<Object, T>( );
			}
		}

		public static HashSet<Object> ConvertEntityHashSet<T>( IEnumerable<T> source )
		{
			HashSet<Object> dataSet = new HashSet<Object>( );

			try
			{
				foreach ( T rawEntity in source )
				{
					dataSet.Add( rawEntity );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			return dataSet;
		}

		public static List<Object> ConvertEntityList<T>( IEnumerable<T> source )
		{
			List<Object> dataSet = new List<Object>( );

			try
			{
				foreach ( T rawEntity in source )
				{
					dataSet.Add( rawEntity );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			return dataSet;
		}

		public static Dictionary<T, Object> ConvertEntityDictionary<T, TU>( IEnumerable<KeyValuePair<T, TU>> source )
		{
			Dictionary<T, Object> dataSet = new Dictionary<T, Object>( );

			try
			{
				foreach ( KeyValuePair<T, TU> rawEntity in source )
				{
					dataSet.Add( rawEntity.Key, rawEntity.Value );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			return dataSet;
		}

		public static Dictionary<Object, TU1> ConvertEntityDictionaryReverse<T1, TU1>( IEnumerable<KeyValuePair<T1, TU1>> source )
		{
			Dictionary<Object, TU1> dataSet = new Dictionary<Object, TU1>( );

			try
			{
				foreach ( KeyValuePair<T1, TU1> rawEntity in source )
				{
					dataSet.Add( rawEntity.Key, rawEntity.Value );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			return dataSet;
		}

		public static Object ChangeObjectType( Object source, Type newType )
		{
			try
			{
				MethodInfo conversion = typeof( UtilityFunctions ).GetMethod( "CastObject", BindingFlags.Public | BindingFlags.Static );
				conversion = conversion.MakeGenericMethod( newType );
				Object result = conversion.Invoke( null, new[ ] { source } );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return source;
			}
		}

		public static T CastObject<T>( Object source )
		{
			return (T)source;
		}

		public static Object ChangeObjectGeneric( Object source, Type newGenericType )
		{
			try
			{
				Type newType = source.GetType( ).MakeGenericType( newGenericType );
				Object result = ChangeObjectType( source, newType );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return source;
			}
		}

		public static long GenerateEntityId( )
		{
			try
			{
				return MyEntityIdentifier.AllocateId( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex, "Failed to generate entity id", ex );
				return 0;
			}
		}

		public static Vector3D GenerateRandomBorderPosition( Vector3 borderStart, Vector3 borderEnd )
		{
			BoundingBoxD box = new BoundingBoxD( borderStart, borderEnd );
			Vector3D result = MyUtils.GetRandomBorderPosition( ref box );

			return result;
		}

		public static List<Type> GetObjectBuilderTypes( )
		{
			List<Type> types = new List<Type>( );

			Assembly assembly = Assembly.GetAssembly( typeof( MyObjectBuilder_Base ) );
			foreach ( Type type in assembly.GetTypes( ) )
			{
				if ( typeof( MyObjectBuilder_Base ).IsAssignableFrom( type ) )
					types.Add( type );
			}

			return types;
		}

		public static List<Type> GetCubeBlockTypes( )
		{
			List<Type> types = new List<Type>( );

			Assembly assembly = Assembly.GetAssembly( typeof( CubeBlockEntity ) );
			foreach ( Type type in assembly.GetTypes( ) )
			{
				if ( typeof( CubeBlockEntity ).IsAssignableFrom( type ) )
					types.Add( type );
			}
			return types;
		}

		public static class DelegateUtility
		{
			public static T Cast<T>( Delegate source ) where T : class
			{
				return Cast( source, typeof( T ) ) as T;
			}

			public static Delegate Cast( Delegate source, Type type )
			{
				if ( source == null )
					return null;

				Delegate[ ] delegates = source.GetInvocationList( );

				if ( delegates.Length == 1 )
					return Delegate.CreateDelegate( type,
						delegates[ 0 ].Target, delegates[ 0 ].Method );

				Delegate[ ] delegatesDest = new Delegate[ delegates.Length ];
				for ( int nDelegate = 0; nDelegate < delegates.Length; nDelegate++ )
					delegatesDest[ nDelegate ] = Delegate.CreateDelegate( type,
						delegates[ nDelegate ].Target, delegates[ nDelegate ].Method );

				return Delegate.Combine( delegatesDest );
			}
		}

		#endregion "Methods"
	}
}