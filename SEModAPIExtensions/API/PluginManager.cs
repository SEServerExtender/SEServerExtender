using System.Windows.Forms;
using SteamSDK;

namespace SEModAPIExtensions.API
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Threading;
	using Sandbox;
	using SEModAPI.API;
	using SEModAPI.API.Sandbox;
	using SEModAPIExtensions.API.Plugin;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.Support;

	public class PluginManager
	{
		private static PluginManager _instance;

		private readonly Dictionary<Guid, Assembly> _pluginAssemblies;
		private DateTime _lastUpdate;
		private TimeSpan _lastUpdateTime;
		private double _averageUpdateInterval;
		private double _averageUpdateTime;
		private DateTime _lastAverageOutput;
		private double _averageEvents;
		private List<ulong> _lastConnectedPlayerList;
		private readonly Dictionary<Guid, string> _pluginPaths;
	    public static bool IsStable;

		#region "Constructors and Initializers"

		protected PluginManager( )
		{
			_instance = this;

			Plugins = new Dictionary<Guid, IPlugin>( );
			PluginStates = new Dictionary<Guid, bool>( );
			_pluginAssemblies = new Dictionary<Guid, Assembly>( );
			Initialized = false;
			_lastUpdate = DateTime.Now;
			_lastUpdateTime = DateTime.Now - _lastUpdate;
			_averageUpdateInterval = 0;
			_averageUpdateTime = 0;
			_lastAverageOutput = DateTime.Now;
			_averageEvents = 0;
			_lastConnectedPlayerList = new List<ulong>( );
			_pluginPaths = new Dictionary<Guid, string>( );

			ApplicationLog.BaseLog.Info( "Finished loading PluginManager" );
		}

		#endregion

		#region "Properties"

		public static PluginManager Instance
		{
			get { return _instance ?? ( _instance = new PluginManager( ) ); }
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
			    {
                    ApplicationLog.BaseLog.Error( "Invalid directory" );
                    return;
			    }

				string[ ] files = Directory.GetFiles( modsPath, "*.dll", SearchOption.AllDirectories );
                if(files.Length==0)
                    ApplicationLog.BaseLog.Info( "Found no dll files" );

				foreach ( string file in files )
				{
					try
					{
                        ApplicationLog.BaseLog.Info( $"Trying to load {file}" );
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
                        ApplicationLog.BaseLog.Info( $"Loaded assembly {pluginAssembly.FullName}" );
						//Get the assembly GUID
						GuidAttribute guid = (GuidAttribute)pluginAssembly.GetCustomAttributes( typeof( GuidAttribute ), true )[ 0 ];
						Guid guidValue = new Guid( guid.Value );

						if ( _pluginPaths.ContainsKey( guidValue ) )
							_pluginPaths[ guidValue ] = file;
						else
							_pluginPaths.Add( guidValue, file );

						if ( _pluginAssemblies.ContainsKey( guidValue ) )
							_pluginAssemblies[ guidValue ] = pluginAssembly;
						else
							_pluginAssemblies.Add( guidValue, pluginAssembly );

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
					}
					catch ( Exception ex )
					{
						ApplicationLog.BaseLog.Error( ex );
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

			return types.Any( type => type.GetInterface( typeof( IPlugin ).FullName ) != null );
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
			if ( !MySandboxGameWrapper.IsGameStarted )
				return;

			_lastUpdateTime = DateTime.Now - _lastUpdate;
			_averageUpdateInterval = ( _averageUpdateTime + _lastUpdateTime.TotalMilliseconds ) / 2;
			_lastUpdate = DateTime.Now;

			EntityEventManager.Instance.ResourceLocked = true;

			List<EntityEventManager.EntityEvent> events = EntityEventManager.Instance.EntityEvents;
			List<ChatManager.ChatEvent> chatEvents = ChatManager.Instance.ChatEvents;

			//Generate the player join/leave events here
			List<ulong> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
			try
			{
				foreach ( ulong steamId in connectedPlayers )
				{
					if ( !_lastConnectedPlayerList.Contains( steamId ) )
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
				foreach ( ulong steamId in _lastConnectedPlayerList )
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
				ApplicationLog.BaseLog.Error( ex );
			}
			_lastConnectedPlayerList = new List<ulong>( connectedPlayers );

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
			if ( ExtenderOptions.IsDebugging )
			{
				_averageEvents = ( _averageEvents + ( events.Count + chatEvents.Count ) ) / 2;

				TimeSpan updateTime = DateTime.Now - _lastUpdate;
				_averageUpdateTime = ( _averageUpdateTime + updateTime.TotalMilliseconds ) / 2;

				TimeSpan timeSinceAverageOutput = DateTime.Now - _lastAverageOutput;
				if ( timeSinceAverageOutput.TotalSeconds > 30 )
				{
					_lastAverageOutput = DateTime.Now;

					ApplicationLog.BaseLog.Debug( "PluginManager - Update interval = {0}ms", _averageUpdateInterval );
					ApplicationLog.BaseLog.Debug( "PluginManager - Update time = {0}ms", _averageUpdateTime );
					ApplicationLog.BaseLog.Debug( "PluginManager - Events per update = {0}", _averageEvents );
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

			String pluginPath = _pluginPaths[ key ];
			ApplicationLog.BaseLog.Info( "Initializing plugin at {0} - {1}'", pluginPath, key );

			try
			{
				IPlugin plugin = Plugins[ key ];
			    if ( plugin.Name == "Dedicated Server Essentials" )
			    {
			        FieldInfo memberInfo = plugin.GetType().GetField( "StableBuild", BindingFlags.Static | BindingFlags.Public );
			        if (memberInfo != null)
			        {
			            bool pluginStable = (bool)memberInfo.GetValue(null);
			            if (!pluginStable && IsStable)
			            {
			                ApplicationLog.Error("WARNING: This version of Essentials is NOT compatible with \"stable\" branch!");
			                ApplicationLog.Error("Aborting plugin initialization!");
			                if (SystemInformation.UserInteractive)
			                {
			                    MessageBox.Show("WARNING: This version of Essentials is NOT compatible with \"stable\" branch!\r\n" +
			                                    "Essentials will not load!",
			                                    "FATAL ERROR", MessageBoxButtons.OK);
			                }
			                return;
			            }
			            else if (pluginStable && !IsStable)
			            {
			                ApplicationLog.Error("WARNING: This version of Essentials is NOT compatible with \"dev\" branch!");
			                ApplicationLog.Error("Aborting plugin initialization!");
			                if (SystemInformation.UserInteractive)
			                {
			                    MessageBox.Show("WARNING: This version of Essentials is NOT compatible with \"dev\" branch!\r\n" +
			                                    "Essentials will not load!",
			                                    "FATAL ERROR", MessageBoxButtons.OK);
			                }
			                return;
			            }
			        }
			    }

			    FieldInfo logField = plugin.GetType( ).GetField( "Log" );
				if ( logField != null )
				{
					logField.SetValue( plugin, ApplicationLog.PluginLog, BindingFlags.Static, null, CultureInfo.CurrentCulture );
				}
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

			_pluginAssemblies.Remove( key );
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
