namespace SEModAPIInternal.API.Common
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;
	using VRage;

	public class GameEntityManager
	{
		#region "Attributes"

		private static FastResourceLock _resourceLock = new FastResourceLock( );
		private static Dictionary<long, BaseObject> _entityMap = new Dictionary<long, BaseObject>( );

		public static string GameEntityManagerNamespace = "";
		public static string GameEntityManagerClass = "Sandbox.Game.Entities.MyEntities";

		public static string GameEntityManagerGetEntityByIdTypeMethod = "TryGetEntityById";

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
				ApplicationLog.BaseLog.Error( ex );
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

		/// <summary>
		/// Adds the specified <paramref name="entity" /> to the <see cref="GameEntityManager"/>
		/// </summary>
		/// <param name="entityId">The numeric ID of the entity to add.</param>
		/// <param name="entity">The entity to add.</param>
		/// <exception cref="ArgumentNullException">The value of 'entity' cannot be null. </exception>
		/// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.Dictionary`2" />.</exception>
		/// <remarks>Until a locking mechanism is respected, duplicate keys are still technically possible.</remarks>
		internal static void AddEntity( long entityId, BaseObject entity )
		{
			//_resourceLock.AcquireExclusive();

			if ( entity == null )
				throw new ArgumentNullException( "entity", "Specified entity cannot be null." );

			if ( !_entityMap.ContainsKey( entityId ) )
				_entityMap.Add( entityId, entity );

			//_resourceLock.ReleaseExclusive();
		}

		/// <summary>
		/// Removes an entity from the <see cref="GameEntityManager"/> by numeric id.
		/// </summary>
		/// <param name="entityId">The numeric id of the entity to remove.</param>
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
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		#endregion "Methods"
	}
}