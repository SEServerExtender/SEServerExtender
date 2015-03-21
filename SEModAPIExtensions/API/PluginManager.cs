namespace SEModAPIExtensions.API
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Threading;
	using SEModAPIExtensions.API.Plugin;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.Support;

	public class PluginManager
	{
		private static PluginManager m_instance;

		private readonly Dictionary<Guid, Assembly> m_pluginAssemblies;
		private DateTime m_lastUpdate;
		private TimeSpan m_lastUpdateTime;
		private double m_averageUpdateInterval;
		private double m_averageUpdateTime;
		private DateTime m_lastAverageOutput;
		private double m_averageEvents;
		private List<ulong> m_lastConnectedPlayerList;
		private readonly Dictionary<Guid, String> m_pluginPaths;

		#region "Constructors and Initializers"

		protected PluginManager( )
		{
			m_instance = this;

			Plugins = new Dictionary<Guid, IPlugin>( );
			PluginStates = new Dictionary<Guid, bool>( );
			m_pluginAssemblies = new Dictionary<Guid, Assembly>( );
			Initialized = false;
			m_lastUpdate = DateTime.Now;
			m_lastUpdateTime = DateTime.Now - m_lastUpdate;
			m_averageUpdateInterval = 0;
			m_averageUpdateTime = 0;
			m_lastAverageOutput = DateTime.Now;
			m_averageEvents = 0;
			m_lastConnectedPlayerList = new List<ulong>( );
			m_pluginPaths = new Dictionary<Guid, string>( );

			ApplicationLog.BaseLog.Info( "Finished loading PluginManager" );
		}

		#endregion

		#region "Properties"

		public static PluginManager Instance
		{
			get { return m_instance ?? ( m_instance = new PluginManager( ) ); }
		}

		public bool Loaded { get; private set; }

		public bool Initialized { get; private set; }

		public Dictionary<Guid, IPlugin> Plugins { get; private set; }

		public Dictionary<Guid, bool> PluginStates { get; private set; }

		#endregion

		#region "Methods"

		public void LoadPlugins( bool forceLoad = false )
		{
			if ( Loaded && !forceLoad )
				return;

			ApplicationLog.BaseLog.Info( "Loading plugins ..." );

			try
			{
				Loaded = true;
				//				m_initialized = false;   // THIS CAUSES all plugins to stop working 

				string modsPath = Path.Combine( Server.Instance.Path, "Mods" );
				ApplicationLog.BaseLog.Info( "Scanning: {0}", modsPath );
				if ( !Directory.Exists( modsPath ) )
					return;

				string[ ] subDirectories = Directory.GetDirectories( modsPath );
				foreach ( string path in subDirectories )
				{
					string[ ] files = Directory.GetFiles( path );
					foreach ( string file in files )
					{
						try
						{
							FileInfo fileInfo = new FileInfo( file );
							if ( !fileInfo.Extension.ToLower( ).Equals( ".dll" ) )
								continue;

							// Load assembly from file into memory, so we can hotswap it if we want
							byte[ ] b = File.ReadAllBytes( file );
							Assembly pluginAssembly = Assembly.Load( b );
							if ( IsOldPlugin( pluginAssembly ) )
							{
								if ( IsValidPlugin( pluginAssembly ) )
									pluginAssembly = Assembly.UnsafeLoadFrom( file );
								else
									continue;
							}

							//Get the assembly GUID
							GuidAttribute guid = (GuidAttribute)pluginAssembly.GetCustomAttributes( typeof( GuidAttribute ), true )[ 0 ];
							Guid guidValue = new Guid( guid.Value );

							if ( m_pluginPaths.ContainsKey( guidValue ) )
								m_pluginPaths[ guidValue ] = file;
							else
								m_pluginPaths.Add( guidValue, file );

							if ( m_pluginAssemblies.ContainsKey( guidValue ) )
								m_pluginAssemblies[ guidValue ] = pluginAssembly;
							else
								m_pluginAssemblies.Add( guidValue, pluginAssembly );

							//Look through the exported types to find the one that implements PluginBase
							Type[ ] types = pluginAssembly.GetExportedTypes( );
							foreach ( Type type in types )
							{
								//Check that we don't have an entry already for this GUID
								if ( Plugins.ContainsKey( guidValue ) )
									break;

								//if (type.BaseType == null)
								//                                    continue;

								//Type[] filteredTypes = type.BaseType.GetInterfaces();
								Type[ ] filteredTypes = type.GetInterfaces( );
								foreach ( Type interfaceType in filteredTypes )
								{
									if ( interfaceType.Name == typeof( IPlugin ).Name )
									{
										try
										{
											//Create an instance of the plugin object
											IPlugin pluginObject = (IPlugin)Activator.CreateInstance( type );

											//And add it to the dictionary
											Plugins.Add( guidValue, pluginObject );

											break;
										}
										catch ( Exception ex )
										{
											ApplicationLog.BaseLog.Error( ex );
										}
									}
								}
							}

							break;
						}
						catch ( Exception ex )
						{
							ApplicationLog.BaseLog.Error( ex );
						}
					}
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			ApplicationLog.BaseLog.Info( "Finished loading plugins" );
		}

		private bool IsOldPlugin( Assembly assembly )
		{
			Type[ ] types = assembly.GetExportedTypes( );

			foreach ( Type type in types )
			{
				if ( type.GetInterface( typeof( IPlugin ).FullName ) != null )
				{
					if ( type.GetMethod( "InitWithPath" ) != null )
						return false;
				}

				if ( type.BaseType == null )
					continue;

				if ( type.BaseType.GetInterface( typeof( IPlugin ).FullName ) != null )
				{
					if ( type.GetMethod( "InitWithPath" ) != null )
						return false;
				}
			}

			return true;
		}

		private bool IsValidPlugin( Assembly assembly )
		{
			Type[ ] types = assembly.GetExportedTypes( );

			return types.Any( type => type.GetInterface( typeof ( IPlugin ).FullName ) != null );
		}

		public void Init( )
		{
			ApplicationLog.BaseLog.Info( "Initializing plugins ..." );
			Initialized = true;

			foreach ( Guid key in Plugins.Keys )
			{
				InitPlugin( key );
			}

			ApplicationLog.BaseLog.Info( "Finished initializing plugins" );
		}

		public void Update( )
		{
			if ( !Loaded )
				return;
			if ( !Initialized )
				return;
			if ( !SandboxGameAssemblyWrapper.Instance.IsGameStarted )
				return;

			m_lastUpdateTime = DateTime.Now - m_lastUpdate;
			m_averageUpdateInterval = ( m_averageUpdateTime + m_lastUpdateTime.TotalMilliseconds ) / 2;
			m_lastUpdate = DateTime.Now;

			EntityEventManager.Instance.ResourceLocked = true;

			List<EntityEventManager.EntityEvent> events = EntityEventManager.Instance.EntityEvents;
			List<ChatManager.ChatEvent> chatEvents = ChatManager.Instance.ChatEvents;

			//Generate the player join/leave events here
			List<ulong> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
			try
			{
				foreach ( ulong steamId in connectedPlayers )
				{
					if ( !m_lastConnectedPlayerList.Contains( steamId ) )
					{
						EntityEventManager.EntityEvent playerEvent = new EntityEventManager.EntityEvent
						                                             {
							                                             priority = 1,
							                                             timestamp = DateTime.Now,
							                                             type = EntityEventManager.EntityEventType.OnPlayerJoined,
							                                             entity = steamId
						                                             };
						//TODO - Find a way to stall the event long enough for a linked character entity to exist - this is impossible because of cockpits and respawnships
						//For now, create a dummy entity just for passing the player's steam id along
						events.Add( playerEvent );
					}
				}
				foreach ( ulong steamId in m_lastConnectedPlayerList )
				{
					if ( !connectedPlayers.Contains( steamId ) )
					{
						EntityEventManager.EntityEvent playerEvent = new EntityEventManager.EntityEvent
						                                             {
							                                             priority = 1,
							                                             timestamp = DateTime.Now,
							                                             type = EntityEventManager.EntityEventType.OnPlayerLeft,
							                                             entity = steamId
						                                             };
						//TODO - Find a way to stall the event long enough for a linked character entity to exist - this is impossible because of cockpits and respawnships
						//For now, create a dummy entity just for passing the player's steam id along
						events.Add( playerEvent );
					}
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Info( "PluginManager.Update() Exception in player discovery: {0}", ex );
			}
			m_lastConnectedPlayerList = new List<ulong>( connectedPlayers );

			//Run the update threads on the plugins
			foreach ( Guid key in Plugins.Keys )
			{
				object plugin = Plugins[ key ];

				if ( !PluginStates.ContainsKey( key ) )
					continue;

				PluginManagerThreadParams parameters = new PluginManagerThreadParams
				                                       {
					                                       Plugin = plugin,
					                                       Key = key,
					                                       Plugins = Plugins,
					                                       PluginState = PluginStates,
					                                       Events = new List<EntityEventManager.EntityEvent>( events ),
					                                       ChatEvents = new List<ChatManager.ChatEvent>( chatEvents )
				                                       };

				ThreadPool.QueueUserWorkItem( DoUpdate, parameters );
				//				Thread pluginThread = new Thread(DoUpdate);
				//				pluginThread.Start(parameters);
			}

			//Capture profiling info if debugging is on
			if ( SandboxGameAssemblyWrapper.IsDebugging )
			{
				m_averageEvents = ( m_averageEvents + ( events.Count + chatEvents.Count ) ) / 2;

				TimeSpan updateTime = DateTime.Now - m_lastUpdate;
				m_averageUpdateTime = ( m_averageUpdateTime + updateTime.TotalMilliseconds ) / 2;

				TimeSpan timeSinceAverageOutput = DateTime.Now - m_lastAverageOutput;
				if ( timeSinceAverageOutput.TotalSeconds > 30 )
				{
					m_lastAverageOutput = DateTime.Now;

					ApplicationLog.BaseLog.Debug( "PluginManager - Update interval = {0}ms", m_averageUpdateInterval );
					ApplicationLog.BaseLog.Debug( "PluginManager - Update time = {0}ms", m_averageUpdateTime );
					ApplicationLog.BaseLog.Debug( "PluginManager - Events per update = {0}", m_averageEvents );
				}
			}

			//Clean up the event managers
			EntityEventManager.Instance.ClearEvents( );
			EntityEventManager.Instance.ResourceLocked = false;
			ChatManager.Instance.ClearEvents( );
		}

		public static void DoUpdate( object args )
		{
			try
			{
				if ( args == null )
					return;
				PluginManagerThreadParams parameters = (PluginManagerThreadParams)args;

				List<EntityEventManager.EntityEvent> events = parameters.Events;
				List<ChatManager.ChatEvent> chatEvents = parameters.ChatEvents;
				Object plugin = parameters.Plugin;
				Dictionary<Guid, IPlugin> plugins = parameters.Plugins;
				Dictionary<Guid, bool> pluginState = parameters.PluginState;

				//Run entity events
				foreach ( EntityEventManager.EntityEvent entityEvent in events )
				{
					//If this is a cube block created event and the parent cube grid is still loading then defer the event
					if ( entityEvent.type == EntityEventManager.EntityEventType.OnCubeBlockCreated )
					{
						CubeBlockEntity cubeBlock = (CubeBlockEntity)entityEvent.entity;
						if ( cubeBlock.Parent.IsLoading )
						{
							EntityEventManager.Instance.AddEvent( entityEvent );
							continue;
						}
					}

					switch ( entityEvent.type )
					{
						case EntityEventManager.EntityEventType.OnPlayerJoined:
							try
							{
								MethodInfo updateMethod = plugin.GetType( ).GetMethod( "OnPlayerJoined" );
								if ( updateMethod != null )
								{
									//FIXME - Temporary hack to pass along the player's steam id
									ulong steamId = (ulong)entityEvent.entity;
									updateMethod.Invoke( plugin, new object[ ] { steamId } );
								}
							}
							catch ( Exception ex )
							{
								ApplicationLog.BaseLog.Error( ex );
							}
							break;
						case EntityEventManager.EntityEventType.OnPlayerLeft:
							try
							{
								MethodInfo updateMethod = plugin.GetType( ).GetMethod( "OnPlayerLeft" );
								if ( updateMethod != null )
								{
									//FIXME - Temporary hack to pass along the player's steam id
									ulong steamId = (ulong)entityEvent.entity;
									updateMethod.Invoke( plugin, new object[ ] { steamId } );
								}
							}
							catch ( Exception ex )
							{
								ApplicationLog.BaseLog.Error( ex );
							}
							break;
						case EntityEventManager.EntityEventType.OnPlayerWorldSent:
							try
							{
								MethodInfo updateMethod = plugin.GetType( ).GetMethod( "OnPlayerWorldSent" );
								if ( updateMethod != null )
								{
									//FIXME - Temporary hack to pass along the player's steam id
									ulong steamId = (ulong)entityEvent.entity;
									updateMethod.Invoke( plugin, new object[ ] { steamId } );
								}
							}
							catch ( Exception ex )
							{
								ApplicationLog.BaseLog.Error( ex );
							}
							break;
						default:
							try
							{
								string methodName = entityEvent.type.ToString( );
								MethodInfo updateMethod = plugin.GetType( ).GetMethod( methodName );
								if ( updateMethod != null )
									updateMethod.Invoke( plugin, new[ ] { entityEvent.entity } );
							}
							catch ( Exception ex )
							{
								ApplicationLog.BaseLog.Error( ex );
							}
							break;
					}
				}

				//Run chat events
				foreach ( ChatManager.ChatEvent chatEvent in chatEvents )
				{
					try
					{
						bool discard = false;
						HookChatMessage( plugin, plugins, pluginState, chatEvent, out discard );

						if ( discard )
							continue;

						string methodName = chatEvent.Type.ToString( );
						MethodInfo updateMethod = plugin.GetType( ).GetMethod( methodName );
						if ( updateMethod != null )
							updateMethod.Invoke( plugin, new object[ ] { chatEvent } );
					}
					catch ( Exception ex )
					{
						ApplicationLog.BaseLog.Error( ex );
					}
				}

				//Run update
				try
				{
					MethodInfo updateMethod = plugin.GetType( ).GetMethod( "Update" );
					updateMethod.Invoke( plugin, new object[ ] { } );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public static void HookChatMessage( Object plugin, Dictionary<Guid, IPlugin> plugins, Dictionary<Guid, bool> pluginState, ChatManager.ChatEvent chatEvent, out bool discard )
		{
			discard = false;

			foreach ( Guid key in plugins.Keys )
			{
				object hookPlugin = plugins[ key ];
				if ( !pluginState.ContainsKey( key ) )
					continue;

				MethodInfo hookMethod = hookPlugin.GetType( ).GetMethod( "OnChatHook" );
				if ( hookMethod != null )
				{
					const bool hookDiscard = false;
					object[ ] args = { chatEvent, plugin, hookDiscard };
					hookMethod.Invoke( hookPlugin, args );
					discard = (bool)args[ 2 ];
				}
			}
		}


		public void Shutdown( )
		{
			for ( int r = Plugins.Count - 1; r >= 0; r-- )
			{
				Guid key = Plugins.ElementAt( r ).Key;
				UnloadPlugin( key );
			}
		}

		public void InitPlugin( Guid key )
		{
			if ( PluginStates.ContainsKey( key ) )
				return;

			String pluginPath = m_pluginPaths[ key ];
			ApplicationLog.BaseLog.Info( "Initializing plugin at {0} - {1}'", pluginPath, key );

			try
			{
				object plugin = Plugins[ key ];
				MethodInfo initMethod = plugin.GetType( ).GetMethod( "InitWithPath" );
				if ( initMethod != null )
				{
					initMethod.Invoke( plugin, new object[ ] { pluginPath } );
				}
				if ( initMethod == null )
				{
					initMethod = plugin.GetType( ).GetMethod( "Init" );
					initMethod.Invoke( plugin, new object[ ] { } );
				}

				PluginStates.Add( key, true );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public void UnloadPlugin( Guid key )
		{
			IPlugin plugin;
			//Skip if the plugin doesn't exist
			if ( !Plugins.TryGetValue( key, out plugin ) )
				return;

			//Skip if the plugin is already unloaded
			if ( !PluginStates.ContainsKey( key ) )
				return;

			ApplicationLog.BaseLog.Info( "Unloading plugin '{0}'", key );

			try
			{
				MethodInfo initMethod = plugin.GetType( ).GetMethod( "Shutdown" );
				initMethod.Invoke( plugin, new object[ ] { } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}

			m_pluginAssemblies.Remove( key );
			PluginStates.Remove( key );
			Plugins.Remove( key );
		}

		public bool GetPluginState( Guid key )
		{
			bool pluginState;
			return PluginStates.TryGetValue( key, out pluginState ) && pluginState;
		}

		#endregion
	}
}
