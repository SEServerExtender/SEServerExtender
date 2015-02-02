using System;
using System.Collections.Generic;
using System.Reflection;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

using VRage;

namespace SEModAPIInternal.API.Common
{
	public class GameEntityManager
	{
		#region "Attributes"

		private static FastResourceLock _resourceLock = new FastResourceLock( );
		private static Dictionary<long, BaseObject> _entityMap = new Dictionary<long, BaseObject>( );

		public static string GameEntityManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string GameEntityManagerClass = "CAF1EB435F77C7B77580E2E16F988BED";

		public static string GameEntityManagerGetEntityByIdTypeMethod = "EB43CD3B683033145620D0931BE5041C";

		#endregion "Attributes"

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( GameEntityManagerNamespace, GameEntityManagerClass );
				return type;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new TypeLoadException( "Could not find internal type for GameEntityManager" );
				//result &= BaseObject.HasMethod(type, GameEntityManagerGetEntityByIdTypeMethod);

				return result;
			}
			catch ( TypeLoadException ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		public static BaseObject GetEntity( long entityId )
		{
			//_resourceLock.AcquireShared();

			if ( !_entityMap.ContainsKey( entityId ) )
				return null;

			BaseObject result = _entityMap[ entityId ];

			//_resourceLock.ReleaseShared();

			return result;
		}

		internal static void AddEntity( long entityId, BaseObject entity )
		{
			//_resourceLock.AcquireExclusive();

			if ( _entityMap.ContainsKey( entityId ) )
				return;

			_entityMap.Add( entityId, entity );

			//_resourceLock.ReleaseExclusive();
		}

		internal static void RemoveEntity( long entityId )
		{
			//_resourceLock.AcquireExclusive();

			if ( !_entityMap.ContainsKey( entityId ) )
				return;

			_entityMap.Remove( entityId );

			//_resourceLock.ReleaseExclusive();
		}

		public static Object GetGameEntity( long entityId, Type entityType )
		{
			try
			{
				MethodInfo method = InternalType.GetMethod( GameEntityManagerGetEntityByIdTypeMethod, BindingFlags.Public | BindingFlags.Static );
				method = method.MakeGenericMethod( entityType );
				object[ ] parameters = new object[ ] { entityId, null };
				object result = method.Invoke( null, parameters );
				bool blResult = (bool)result;
				if ( blResult )
				{
					return parameters[ 1 ];
				}
				else
				{
					return null;
				}
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		#endregion "Methods"
	}
}