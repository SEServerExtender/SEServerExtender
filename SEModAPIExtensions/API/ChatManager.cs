using SteamSDK;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRage;
using VRage.Common.Utils;
using VRageMath;

using Sandbox.ModAPI;

namespace SEModAPIExtensions.API
{
	using System.Linq;

	public class ChatManager
	{
		public struct ChatCommand
		{
			public ChatCommand( string command, Action<ChatEvent> callback, bool requiresAdmin )
			{
				Command = command;
				Callback = callback;
				RequiresAdmin = requiresAdmin;
			}
			public string Command;
			public Action<ChatEvent> Callback;
			public bool RequiresAdmin;
		}

		public enum ChatEventType
		{
			OnChatReceived,
			OnChatSent,
		}

		public struct ChatEvent
		{
			public ChatEvent( ChatEventType type, DateTime timestamp, ulong sourceUserId, ulong remoteUserId, string message, ushort priority )
			{
				Type = type;
				Timestamp = timestamp;
				SourceUserId = sourceUserId;
				RemoteUserId = remoteUserId;
				Message = message;
				Priority = priority;
			}

			public ChatEventType Type;
			public DateTime Timestamp;
			public ulong SourceUserId;
			public ulong RemoteUserId;
			public string Message;
			public ushort Priority;

			public ChatEvent( DateTime timestamp, ulong remoteUserId, string message )
			{
				Timestamp = timestamp;
				RemoteUserId = remoteUserId;
				Message = message;

				//Defaults
				Type = ChatEventType.OnChatReceived;
				SourceUserId = 0;
				Priority = 0;
			}
		}

		#region "Attributes"

		private static ChatManager m_instance;

		private static List<string> m_chatMessages;
		private static List<ChatEvent> m_chatHistory;
		private static bool m_chatHandlerSetup;
		private static FastResourceLock m_resourceLock;

		private List<ChatEvent> m_chatEvents;
		private Dictionary<ChatCommand, Guid> m_chatCommands;

		/////////////////////////////////////////////////////////////////////////////

		public static string ChatMessageStructNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string ChatMessageStructClass = "12AEE9CB08C9FC64151B8A094D6BB668";

		public static string ChatMessageMessageField = "EDCBEBB604B287DFA90A5A46DC7AD28D";

		#endregion

		#region "Constructors and Initializers"

		protected ChatManager( )
		{
			m_instance = this;

			m_chatMessages = new List<string>( );
			m_chatHistory = new List<ChatEvent>( );
			m_chatHandlerSetup = false;
			m_resourceLock = new FastResourceLock( );
			m_chatEvents = new List<ChatEvent>( );
			m_chatCommands = new Dictionary<ChatCommand, Guid>( );

			ChatCommand deleteCommand = new ChatCommand( "delete", Command_Delete, true );

			ChatCommand tpCommand = new ChatCommand( "tp", Command_Teleport, true );

			ChatCommand stopCommand = new ChatCommand( "stop", Command_Stop, true );

			ChatCommand getIdCommand = new ChatCommand( "getid", Command_GetId, true );

			ChatCommand saveCommand = new ChatCommand( "save", Command_Save, true );

			ChatCommand ownerCommand = new ChatCommand( "owner", Command_Owner, true );

			ChatCommand exportCommand = new ChatCommand( "export", Command_Export, true );

			ChatCommand importCommand = new ChatCommand( "import", Command_Import, true );

			ChatCommand spawnCommand = new ChatCommand( "spawn", Command_Spawn, true );

			ChatCommand clearCommand = new ChatCommand( "clear", Command_Clear, true );

			ChatCommand listCommand = new ChatCommand( "list", Command_List, true );

			ChatCommand kickCommand = new ChatCommand( "kick", Command_Kick, true );

			ChatCommand onCommand = new ChatCommand( "on", Command_On, true );

			ChatCommand offCommand = new ChatCommand( "off", Command_Off, true );

			ChatCommand banCommand = new ChatCommand( "ban", Command_Ban, true );

			ChatCommand unbanCommand = new ChatCommand( "unban", Command_Unban, true );

			ChatCommand asyncSaveCommand = new ChatCommand( "savesync", Command_SyncSave, true );

			RegisterChatCommand( offCommand );
			RegisterChatCommand( onCommand );
			RegisterChatCommand( deleteCommand );
			RegisterChatCommand( tpCommand );
			RegisterChatCommand( stopCommand );
			RegisterChatCommand( getIdCommand );
			RegisterChatCommand( saveCommand );
			RegisterChatCommand( ownerCommand );
			RegisterChatCommand( exportCommand );
			RegisterChatCommand( importCommand );
			RegisterChatCommand( spawnCommand );
			RegisterChatCommand( clearCommand );
			RegisterChatCommand( listCommand );
			RegisterChatCommand( kickCommand );
			RegisterChatCommand( banCommand );
			RegisterChatCommand( unbanCommand );
			RegisterChatCommand( asyncSaveCommand );

			SetupWcfService( );

			Console.WriteLine( "Finished loading ChatManager" );
		}

		private static void SetupWcfService( )
		{
			if ( !Server.Instance.IsWCFEnabled )
				return;

			ServiceHost selfHost = null;
			try
			{
				selfHost = Server.CreateServiceHost( typeof( ChatService ), typeof( IChatServiceContract ), "Chat/", "ChatService" );
				selfHost.Open( );
			}
			catch ( CommunicationException ex )
			{
				LogManager.ErrorLog.WriteLineAndConsole( string.Format( "An exception occurred: {0}", ex.Message ) );
				if ( selfHost != null )
					selfHost.Abort( );
			}
		}

		#endregion

		#region "Properties"

		public static ChatManager Instance
		{
			get { return m_instance ?? ( m_instance = new ChatManager( ) ); }
		}

		public List<string> ChatMessages
		{
			get
			{
				SetupChatHandlers( );

				return m_chatMessages;
			}
		}

		public List<ChatEvent> ChatHistory
		{
			get
			{
				SetupChatHandlers( );

				m_resourceLock.AcquireShared( );

				List<ChatEvent> history = new List<ChatEvent>( m_chatHistory );

				m_resourceLock.ReleaseShared( );

				return history;
			}
		}

		public List<ChatEvent> ChatEvents
		{
			get
			{
				SetupChatHandlers( );

				List<ChatEvent> copy = new List<ChatEvent>( m_chatEvents.ToArray( ) );
				return copy;
			}
		}

		#endregion

		#region "Methods"

		#region "General"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ChatMessageStructNamespace, ChatMessageStructClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for ChatMessageStruct" );
				bool result = true;
				result &= BaseObject.HasField( type, ChatMessageMessageField );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		private void SetupChatHandlers( )
		{
			if ( m_chatHandlerSetup )
				return;

			if ( !SandboxGameAssemblyWrapper.Instance.IsGameStarted )
				return;

			try
			{
				object netManager = NetworkManager.GetNetworkManager( );
				if ( netManager == null )
					return;

				Action<ulong, string, ChatEntryTypeEnum> chatHook = ReceiveChatMessage;
				ServerNetworkManager.Instance.RegisterChatReceiver( chatHook );

				m_chatHandlerSetup = true;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		protected Object CreateChatMessageStruct( string message )
		{
			Type chatMessageStructType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ChatMessageStructNamespace, ChatMessageStructClass );
			FieldInfo messageField = chatMessageStructType.GetField( ChatMessageMessageField );

			Object chatMessageStruct = Activator.CreateInstance( chatMessageStructType );
			messageField.SetValue( chatMessageStruct, message );

			return chatMessageStruct;
		}

		protected void ReceiveChatMessage( ulong remoteUserId, string message, ChatEntryTypeEnum entryType )
		{
			string playerName = PlayerMap.Instance.GetPlayerNameFromSteamId( remoteUserId );

			bool commandParsed = ParseChatCommands( message, remoteUserId );

			if ( !commandParsed && entryType == ChatEntryTypeEnum.ChatMsg )
			{
				m_chatMessages.Add( string.Format( "{0}: {1}", playerName, message ) );
				LogManager.ChatLog.WriteLineAndConsole( string.Format( "Chat - Client '{0}': {1}", playerName, message ) );
			}

			ChatEvent chatEvent = new ChatEvent( ChatEventType.OnChatReceived, DateTime.Now, remoteUserId, 0, message, 0 );
			Instance.AddEvent( chatEvent );

			m_resourceLock.AcquireExclusive( );
			m_chatHistory.Add( chatEvent );
			m_resourceLock.ReleaseExclusive( );
		}

		public void SendPrivateChatMessage( ulong remoteUserId, string message )
		{
			if ( !SandboxGameAssemblyWrapper.Instance.IsGameStarted )
				return;
			if ( string.IsNullOrEmpty( message ) )
				return;

			try
			{
				if ( remoteUserId != 0 )
				{
					Object chatMessageStruct = CreateChatMessageStruct( message );
					ServerNetworkManager.Instance.SendStruct( remoteUserId, chatMessageStruct, chatMessageStruct.GetType( ) );
				}

				m_chatMessages.Add( string.Format( "Server: {0}", message ) );

				LogManager.ChatLog.WriteLineAndConsole( string.Format( "Chat - Server: {0}", message ) );

				ChatEvent chatEvent = new ChatEvent( ChatEventType.OnChatSent, DateTime.Now, 0, remoteUserId, message, 0 );
				Instance.AddEvent( chatEvent );

				m_resourceLock.AcquireExclusive( );
				m_chatHistory.Add( chatEvent );
				m_resourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		public void SendPublicChatMessage( string message )
		{
			if ( !SandboxGameAssemblyWrapper.Instance.IsGameStarted )
				return;
			if ( string.IsNullOrEmpty( message ) )
				return;

			bool commandParsed = ParseChatCommands( message );

			try
			{
				if ( !commandParsed && message[ 0 ] != '/' )
				{
					Object chatMessageStruct = CreateChatMessageStruct( message );
					List<ulong> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
					foreach ( ulong remoteUserId in connectedPlayers )
					{
						if ( !remoteUserId.ToString( ).StartsWith( "9009" ) )
							ServerNetworkManager.Instance.SendStruct( remoteUserId, chatMessageStruct, chatMessageStruct.GetType( ) );

						ChatEvent chatEvent = new ChatEvent( ChatEventType.OnChatSent, DateTime.Now, 0, remoteUserId, message, 0 );
						Instance.AddEvent( chatEvent );
					}
					m_chatMessages.Add( string.Format( "Server: {0}", message ) );
					LogManager.ChatLog.WriteLineAndConsole( string.Format( "Chat - Server: {0}", message ) );
				}

				//Send a loopback chat event for server-sent messages
				ChatEvent selfChatEvent = new ChatEvent( ChatEventType.OnChatReceived, DateTime.Now, 0, 0, message, 0 );
				Instance.AddEvent( selfChatEvent );

				m_resourceLock.AcquireExclusive( );
				m_chatHistory.Add( selfChatEvent );
				m_resourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		protected bool ParseChatCommands( string message, ulong remoteUserId = 0 )
		{
			try
			{
				if ( string.IsNullOrEmpty( message ) )
					return false;

				string[ ] commandParts = message.Split( ' ' );
				if ( commandParts.Length == 0 )
					return false;

				//Skip if message doesn't have leading forward slash
				if ( !message.Substring( 0, 1 ).Equals( "/" ) )
					return false;

				//Get the base command and strip off the leading slash
				string command = commandParts[ 0 ].ToLower( ).Substring( 1 );
				if ( string.IsNullOrEmpty( command ) )
					return false;

				//Search for a matching, registered command
				bool foundMatch = false;
				foreach ( ChatCommand chatCommand in m_chatCommands.Keys )
				{
					try
					{
						if ( chatCommand.RequiresAdmin && remoteUserId != 0 && !PlayerManager.Instance.IsUserAdmin( remoteUserId ) )
							continue;

						if ( command.Equals( chatCommand.Command.ToLower( ) ) )
						{
							ChatEvent chatEvent = new ChatEvent( DateTime.Now, remoteUserId, message );

							bool discard;
							PluginManager.HookChatMessage( null, PluginManager.Instance.Plugins, PluginManager.Instance.PluginStates, chatEvent, out discard );

							if ( !discard )
								chatCommand.Callback( chatEvent );

							foundMatch = true;
							break;
						}
					}
					catch ( Exception ex )
					{
						LogManager.ErrorLog.WriteLine( ex );
					}
				}

				return foundMatch;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		public void RegisterChatCommand( ChatCommand command )
		{
			//Check if the given command already is registered
			if ( m_chatCommands.Keys.Any( chatCommand => chatCommand.Command.ToLower( ).Equals( command.Command.ToLower( ) ) ) )
			{
				return;
			}

			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly( ).GetCustomAttributes( typeof( GuidAttribute ), true )[ 0 ];
			Guid guidValue = new Guid( guid.Value );

			m_chatCommands.Add( command, guidValue );
		}

		public void UnregisterChatCommands( )
		{
			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly( ).GetCustomAttributes( typeof( GuidAttribute ), true )[ 0 ];
			Guid guidValue = new Guid( guid.Value );

			List<ChatCommand> commandsToRemove = ( from entry in m_chatCommands
												   where entry.Value.Equals( guidValue )
												   select entry.Key ).ToList( );
			foreach ( ChatCommand entry in commandsToRemove )
			{
				m_chatCommands.Remove( entry );
			}
		}

		public void AddEvent( ChatEvent newEvent )
		{
			m_chatEvents.Add( newEvent );
		}

		public void ClearEvents( )
		{
			m_chatEvents.Clear( );
		}

		#endregion

		#region "Chat Command Callbacks"

		protected void Command_Delete( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			//All entities
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "all" ) )
			{
				//All cube grids that have no beacon or only a beacon with no name
				if ( commandParts[ 2 ].ToLower( ).Equals( "nobeacon" ) )
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>( );
					foreach ( CubeGridEntity entity in entities )
					{
						while ( entity.CubeBlocks.Count == 0 )
						{
							Thread.Sleep( 20 );
						}
						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						if ( blocks.Count > 0 )
						{
							bool foundBeacon = blocks.OfType<BeaconEntity>( ).Any( );
							if ( !foundBeacon )
							{
								entitiesToDispose.Add( entity );
							}
						}
					}

					foreach ( CubeGridEntity entity in entitiesToDispose )
					{
						bool isLinkedShip = false;
						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						foreach ( CubeBlockEntity cubeBlock in blocks )
						{
							if ( cubeBlock is MergeBlockEntity )
							{
								MergeBlockEntity block = (MergeBlockEntity)cubeBlock;
								if ( block.IsAttached )
								{
									if ( !entitiesToDispose.Contains( block.AttachedCubeGrid ) )
									{
										isLinkedShip = true;
										break;
									}
								}
							}
							if ( cubeBlock is PistonEntity )
							{
								PistonEntity block = (PistonEntity)cubeBlock;
								CubeBlockEntity topBlock = block.TopBlock;
								if ( topBlock != null )
								{
									if ( !entitiesToDispose.Contains( topBlock.Parent ) )
									{
										isLinkedShip = true;
										break;
									}
								}
							}
							if ( cubeBlock is RotorEntity )
							{
								RotorEntity block = (RotorEntity)cubeBlock;
								CubeBlockEntity topBlock = block.TopBlock;
								if ( topBlock != null )
								{
									if ( !entitiesToDispose.Contains( topBlock.Parent ) )
									{
										isLinkedShip = true;
										break;
									}
								}
							}
						}
						if ( isLinkedShip )
							continue;

						entity.Dispose( );
					}

					SendPrivateChatMessage( remoteUserId, string.Format( "{0} cube grids have been removed", entitiesToDispose.Count ) );
				}
				//All cube grids that have no power
				else if ( commandParts[ 2 ].ToLower( ).Equals( "nopower" ) )
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
					List<CubeGridEntity> entitiesToDispose = entities.Where( entity => entity.TotalPower <= 0 ).ToList( );

					foreach ( CubeGridEntity entity in entitiesToDispose )
					{
						entity.Dispose( );
					}

					SendPrivateChatMessage( remoteUserId, string.Format( "{0} cube grids have been removed", entitiesToDispose.Count ) );
				}
				else if ( commandParts[ 2 ].ToLower( ).Equals( "floatingobjects" ) )	//All floating objects
				{
					/*
					List<FloatingObject> entities = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>();
					int floatingObjectCount = entities.Count;
					foreach (FloatingObject entity in entities)
					{
						entity.Dispose();
					}
					 */

					int count = 0;
					SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
					{
						HashSet<IMyEntity> entities = new HashSet<IMyEntity>( );
						MyAPIGateway.Entities.GetEntities( entities );
						List<IMyEntity> entitiesToRemove = new List<IMyEntity>( );

						foreach ( IMyEntity entity in entities )
						{
							MyObjectBuilder_Base objectBuilder;
							try
							{
								objectBuilder = entity.GetObjectBuilder( );
							}
							catch
							{
								continue;
							}

							if ( objectBuilder is MyObjectBuilder_FloatingObject )
								entitiesToRemove.Add( entity );
						}

						for ( int r = entitiesToRemove.Count - 1; r >= 0; r-- )
						{
							IMyEntity entity = entitiesToRemove[ r ];
							MyAPIGateway.Entities.RemoveEntity( entity );
							count++;
						}
					} );

					SendPrivateChatMessage( remoteUserId, count + " floating objects have been removed" );
				}
				else
				{
					string entityName = commandParts[ 2 ];
					if ( commandParts.Length > 3 )
					{
						for ( int i = 3; i < commandParts.Length; i++ )
						{
							entityName += " " + commandParts[ i ];
						}
					}

					int matchingEntitiesCount = 0;
					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
					foreach ( BaseEntity entity in entities )
					{
						bool isMatch = Regex.IsMatch( entity.Name, entityName, RegexOptions.IgnoreCase );
						if ( !isMatch )
							continue;

						entity.Dispose( );

						matchingEntitiesCount++;
					}

					SendPrivateChatMessage( remoteUserId, string.Format( "{0} objects have been removed", matchingEntitiesCount ) );
				}
			}

			//All non-static cube grids
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "ship" ) )
			{
				//That have no beacon or only a beacon with no name
				if ( commandParts[ 2 ].ToLower( ).Equals( "nobeacon" ) )
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>( );
					foreach ( CubeGridEntity entity in entities )
					{
						//Skip static cube grids
						if ( entity.IsStatic )
							continue;

						if ( entity.Name.Equals( entity.EntityId.ToString( ) ) )
						{
							entitiesToDispose.Add( entity );
							continue;
						}

						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						if ( blocks.Count > 0 )
						{
							bool foundBeacon = entity.CubeBlocks.OfType<BeaconEntity>( ).Any( );
							if ( !foundBeacon )
							{
								entitiesToDispose.Add( entity );
							}
						}
					}

					foreach ( CubeGridEntity entity in entitiesToDispose )
					{
						entity.Dispose( );
					}

					SendPrivateChatMessage( remoteUserId, string.Format( "{0} ships have been removed", entitiesToDispose.Count ) );
				}
			}

			//All static cube grids
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "station" ) )
			{
				//That have no beacon or only a beacon with no name
				if ( commandParts[ 2 ].ToLower( ).Equals( "nobeacon" ) )
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>( );
					foreach ( CubeGridEntity entity in entities )
					{
						//Skip non-static cube grids
						if ( !entity.IsStatic )
							continue;

						if ( entity.Name.Equals( entity.EntityId.ToString( ) ) )
						{
							entitiesToDispose.Add( entity );
							continue;
						}

						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						if ( blocks.Count > 0 )
						{
							bool foundBeacon = entity.CubeBlocks.OfType<BeaconEntity>( ).Any( );
							if ( !foundBeacon )
							{
								entitiesToDispose.Add( entity );
							}
						}
					}

					foreach ( CubeGridEntity entity in entitiesToDispose )
					{
						entity.Dispose( );
					}

					SendPrivateChatMessage( remoteUserId, entitiesToDispose.Count + " stations have been removed" );
				}
			}

			//Prunes defunct player entries in the faction data
			/*
			if (paramCount > 1 && commandParts[1].ToLower().Equals("player"))
			{
				List<MyObjectBuilder_Checkpoint.PlayerItem> playersToRemove = new List<MyObjectBuilder_Checkpoint.PlayerItem>();
				int playersRemovedCount = 0;
				if (commandParts[2].ToLower().Equals("dead"))
				{
					List<long> playerIds = PlayerMap.Instance.GetPlayerIds();
					foreach (long playerId in playerIds)
					{
						MyObjectBuilder_Checkpoint.PlayerItem item = PlayerMap.Instance.GetPlayerItemFromPlayerId(playerId);
						if (item.IsDead)
							playersToRemove.Add(item);
					}

					//TODO - This is VERY slow. Need to find a much faster way to do this
					//TODO - Need to find a way to remove the player entries from the main list, not just from the blocks and factions
					foreach (var item in playersToRemove)
					{
						bool playerRemoved = false;

						//Check if any of the players we're about to remove own blocks
						//If so, set the owner to 0 and set the share mode to None
						foreach (var cubeGrid in SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>())
						{
							foreach (var cubeBlock in cubeGrid.CubeBlocks)
							{
								if (cubeBlock.Owner == item.PlayerId)
								{
									cubeBlock.Owner = 0;
									cubeBlock.ShareMode = MyOwnershipShareModeEnum.None;

									playerRemoved = true;
								}
							}
						}

						foreach (var entry in FactionsManager.Instance.Factions)
						{
							foreach (var member in entry.Members)
							{
								if (member.PlayerId == item.PlayerId)
								{
									entry.RemoveMember(member.PlayerId);

									playerRemoved = true;
								}
							}
						}

						if (playerRemoved)
							playersRemovedCount++;
					}
				}

				SendPrivateChatMessage(remoteUserId, "Deleted " + playersRemovedCount.ToString() + " player entries");
			}
			*/
			//Prunes defunct faction entries in the faction data
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "faction" ) )
			{
				List<Faction> factionsToRemove = new List<Faction>( );
				if ( commandParts[ 2 ].ToLower( ).Equals( "empty" ) )
				{
					factionsToRemove.AddRange( FactionsManager.Instance.Factions.Where( entry => entry.Members.Count == 0 ) );
				}
				if ( commandParts[ 2 ].ToLower( ).Equals( "nofounder" ) )
				{
					foreach ( var entry in FactionsManager.Instance.Factions )
					{
						bool founderMatch = entry.Members.Any( member => member.IsFounder );

						if ( !founderMatch )
							factionsToRemove.Add( entry );
					}
				}
				if ( commandParts[ 2 ].ToLower( ).Equals( "noleader" ) )
				{
					foreach ( var entry in FactionsManager.Instance.Factions )
					{
						bool founderMatch = entry.Members.Any( member => member.IsFounder || member.IsLeader );

						if ( !founderMatch )
							factionsToRemove.Add( entry );
					}
				}

				foreach ( var entry in factionsToRemove )
				{
					FactionsManager.Instance.RemoveFaction( entry.Id );
				}

				SendPrivateChatMessage( remoteUserId, string.Format( "Deleted {0} factions", factionsToRemove.Count ) );
			}

			//Single entity
			if ( paramCount == 1 )
			{
				string rawEntityId = commandParts[ 1 ];

				try
				{
					long entityId = long.Parse( rawEntityId );

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
					foreach ( BaseEntity entity in entities )
					{
						if ( entity.EntityId != entityId )
							continue;

						entity.Dispose( );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		protected void Command_Teleport( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount == 2 )
			{
				string rawEntityId = commandParts[ 1 ];
				string rawPosition = commandParts[ 2 ];

				try
				{
					long entityId = long.Parse( rawEntityId );

					string[ ] rawCoordinateValues = rawPosition.Split( ',' );
					if ( rawCoordinateValues.Length < 3 )
						return;

					float x = float.Parse( rawCoordinateValues[ 0 ] );
					float y = float.Parse( rawCoordinateValues[ 1 ] );
					float z = float.Parse( rawCoordinateValues[ 2 ] );

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
					foreach ( BaseEntity entity in entities )
					{
						if ( entity.EntityId != entityId )
							continue;

						Vector3D newPosition = new Vector3D( x, y, z );
						entity.Position = newPosition;

						SendPrivateChatMessage( remoteUserId, string.Format( "Entity '{0}' has been moved to '{1}'", entity.EntityId, newPosition ) );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		protected void Command_Stop( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			if ( commandParts[ 1 ].ToLower( ).Equals( "all" ) )
			{
				List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
				int entitiesStoppedCount = 0;
				foreach ( BaseEntity entity in entities )
				{
					double linear = Math.Round( ( (Vector3)entity.LinearVelocity ).LengthSquared( ), 1 );
					double angular = Math.Round( ( (Vector3)entity.AngularVelocity ).LengthSquared( ), 1 );

					if ( linear > 0 || angular > 0 )
					{
						entity.LinearVelocity = Vector3.Zero;
						entity.AngularVelocity = Vector3.Zero;

						entitiesStoppedCount++;
					}
				}
				SendPrivateChatMessage( remoteUserId, string.Format( "{0} entities are no longer moving or rotating", entitiesStoppedCount ) );
			}
			else
			{
				string rawEntityId = commandParts[ 1 ];

				try
				{
					long entityId = long.Parse( rawEntityId );

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
					foreach ( BaseEntity entity in entities )
					{
						if ( entity.EntityId != entityId )
							continue;

						entity.LinearVelocity = Vector3.Zero;
						entity.AngularVelocity = Vector3.Zero;

						SendPrivateChatMessage( remoteUserId, string.Format( "Entity '{0}' is no longer moving or rotating", entity.EntityId ) );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		protected void Command_GetId( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount > 0 )
			{
				string entityName = commandParts[ 1 ];
				if ( commandParts.Length > 2 )
				{
					for ( int i = 2; i < commandParts.Length; i++ )
					{
						entityName += string.Format( " {0}", commandParts[ i ] );
					}
				}

				List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
				foreach ( BaseEntity entity in entities )
				{
					if ( !entity.Name.ToLower( ).Equals( entityName.ToLower( ) ) )
						continue;

					SendPrivateChatMessage( remoteUserId, string.Format( "Entity ID is '{0}'", entity.EntityId ) );
				}
			}
		}

		protected void Command_Save( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;

			WorldManager.Instance.AsynchronousSaveWorld( );
			SendPrivateChatMessage( remoteUserId, "Performing an asynchronous save." );
		}

		protected void Command_SyncSave( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;

			WorldManager.Instance.SaveWorld( );

			SendPrivateChatMessage( remoteUserId, "World has been saved!" );
		}

		protected void Command_Owner( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount == 2 )
			{
				string rawEntityId = commandParts[ 1 ];
				string rawOwnerId = commandParts[ 2 ];

				try
				{
					long entityId = long.Parse( rawEntityId );
					long ownerId = long.Parse( rawOwnerId );

					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
					foreach ( CubeGridEntity cubeGrid in entities )
					{
						if ( cubeGrid.EntityId != entityId )
							continue;

						//Update the owner of the blocks on the cube grid
						foreach ( CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks.Where( cubeBlock => cubeBlock.EntityId != 0 ) )
						{
							cubeBlock.Owner = ownerId;
						}

						SendPrivateChatMessage( remoteUserId, string.Format( "CubeGridEntity '{0}' owner has been changed to '{1}'", cubeGrid.EntityId, ownerId ) );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		protected void Command_Export( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount == 1 )
			{
				string rawEntityId = commandParts[ 1 ];

				try
				{
					long entityId = long.Parse( rawEntityId );

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
					foreach ( BaseEntity entity in entities )
					{
						if ( entity.EntityId != entityId )
							continue;

						string modPath = MyFileSystem.ModsPath;
						if ( !Directory.Exists( modPath ) )
							break;

						string fileName = entity.Name.ToLower( );
						Regex rgx = new Regex( "[^a-zA-Z0-9]" );
						string cleanFileName = rgx.Replace( fileName, string.Empty );

						string exportPath = Path.Combine( modPath, "Exports" );
						if ( !Directory.Exists( exportPath ) )
							Directory.CreateDirectory( exportPath );
						FileInfo exportFile = new FileInfo( Path.Combine( exportPath, cleanFileName + ".sbc" ) );
						entity.Export( exportFile );

						SendPrivateChatMessage( remoteUserId, string.Format( "Entity '{0}' has been exported to Mods/Exports", entity.EntityId ) );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		protected void Command_Import( ChatEvent chatEvent )
		{
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount == 1 )
			{
				try
				{
					string fileName = commandParts[ 1 ];
					Regex rgx = new Regex( "[^a-zA-Z0-9]" );
					string cleanFileName = rgx.Replace( fileName, string.Empty );

					string modPath = MyFileSystem.ModsPath;
					if ( Directory.Exists( modPath ) )
					{
						string exportPath = Path.Combine( modPath, "Exports" );
						if ( Directory.Exists( exportPath ) )
						{
							FileInfo importFile = new FileInfo( Path.Combine( exportPath, cleanFileName ) );
							if ( importFile.Exists )
							{
								string objectBuilderTypeName = string.Empty;
								using ( XmlReader reader = XmlReader.Create( importFile.OpenText( ) ) )
								{
									while ( reader.Read( ) )
									{
										if ( reader.NodeType == XmlNodeType.XmlDeclaration )
											continue;

										if ( reader.NodeType != XmlNodeType.Element )
											continue;

										objectBuilderTypeName = reader.Name;
										break;
									}
								}

								if ( string.IsNullOrEmpty( objectBuilderTypeName ) )
									return;

								switch ( objectBuilderTypeName )
								{
									case "MyObjectBuilder_CubeGrid":
										CubeGridEntity cubeGrid = new CubeGridEntity( importFile );
										SectorObjectManager.Instance.AddEntity( cubeGrid );
										break;
									default:
										break;
								}
							}
						}
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
			}
		}

		protected void Command_Spawn( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "ship" ) )
			{
				if ( commandParts[ 2 ].ToLower( ).Equals( "all" ) )
				{
				}
				if ( commandParts[ 2 ].ToLower( ).Equals( "exports" ) )
				{
				}
				if ( commandParts[ 2 ].ToLower( ).Equals( "cargo" ) )
				{
					CargoShipManager.Instance.SpawnCargoShipGroup( remoteUserId );
				}
			}
		}

		protected void Command_Clear( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			List<CubeGridEntity> cubeGrids = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
			int queueCount = 0;
			foreach ( var cubeGrid in cubeGrids )
			{
				foreach ( CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks )
				{
					if ( commandParts[ 1 ].ToLower( ).Equals( "productionqueue" ) && cubeBlock is ProductionBlockEntity )
					{
						ProductionBlockEntity block = (ProductionBlockEntity)cubeBlock;
						block.ClearQueue( );
						queueCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "refineryqueue" ) && cubeBlock is RefineryEntity )
					{
						RefineryEntity block = (RefineryEntity)cubeBlock;
						block.ClearQueue( );
						queueCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "assemblerqueue" ) && cubeBlock is AssemblerEntity )
					{
						AssemblerEntity block = (AssemblerEntity)cubeBlock;
						block.ClearQueue( );
						queueCount++;
					}
				}
			}

			SendPrivateChatMessage( remoteUserId, "Cleared the production queue of " + queueCount + " blocks" );
		}

		protected void Command_List( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			if ( commandParts[ 1 ].ToLower( ).Equals( "all" ) )
			{
				List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>( );
				LogManager.APILog.WriteLineAndConsole( "Total entities: '" + entities.Count + "'" );

				SendPrivateChatMessage( remoteUserId, "Total entities: '" + entities.Count + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "cubegrid" ) )
			{
				List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
				LogManager.APILog.WriteLineAndConsole( "Cubegrid entities: '" + entities.Count + "'" );

				SendPrivateChatMessage( remoteUserId, "Cubegrid entities: '" + entities.Count + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "character" ) )
			{
				List<CharacterEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CharacterEntity>( );
				LogManager.APILog.WriteLineAndConsole( "Character entities: '" + entities.Count + "'" );

				SendPrivateChatMessage( remoteUserId, "Character entities: '" + entities.Count + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "voxelmap" ) )
			{
				List<VoxelMap> entities = SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>( );
				LogManager.APILog.WriteLineAndConsole( "Voxelmap entities: '" + entities.Count + "'" );

				SendPrivateChatMessage( remoteUserId, "Voxelmap entities: '" + entities.Count + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "meteor" ) )
			{
				List<Meteor> entities = SectorObjectManager.Instance.GetTypedInternalData<Meteor>( );
				LogManager.APILog.WriteLineAndConsole( "Meteor entities: '" + entities.Count + "'" );

				SendPrivateChatMessage( remoteUserId, "Meteor entities: '" + entities.Count + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "floatingobject" ) )
			{
				List<FloatingObject> entities = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>( );
				LogManager.APILog.WriteLineAndConsole( "Floating object entities: '" + entities.Count + "'" );

				SendPrivateChatMessage( remoteUserId, "Floating object entities: '" + entities.Count + "'" );
			}
		}

		protected void Command_Off( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			List<CubeGridEntity> cubeGrids = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
			int poweredOffCount = 0;
			foreach ( var cubeGrid in cubeGrids )
			{
				foreach ( CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks )
				{
					if ( !( cubeBlock is FunctionalBlockEntity ) )
						continue;

					FunctionalBlockEntity functionalBlock = (FunctionalBlockEntity)cubeBlock;

					if ( commandParts[ 1 ].ToLower( ).Equals( "all" ) )
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "production" ) && cubeBlock is ProductionBlockEntity )
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "beacon" ) && cubeBlock is BeaconEntity )
					{
						functionalBlock.Enabled = false;
						BeaconEntity beacon = (BeaconEntity)cubeBlock;
						beacon.BroadcastRadius = 1;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "tools" ) && ( cubeBlock is ShipToolBaseEntity || cubeBlock is ShipDrillEntity ) )
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "turrets" ) && ( cubeBlock is TurretBaseEntity ) )
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}

					if ( commandParts[ 1 ].ToLower( ).Equals( cubeBlock.Id.SubtypeName.ToLower( ) ) )
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}

				}
			}

			SendPrivateChatMessage( remoteUserId, "Turned off " + poweredOffCount + " blocks" );
		}

		protected void Command_On( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			List<CubeGridEntity> cubeGrids = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
			int poweredOffCount = 0;
			foreach ( var cubeGrid in cubeGrids )
			{
				foreach ( CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks )
				{
					if ( !( cubeBlock is FunctionalBlockEntity ) )
						continue;

					FunctionalBlockEntity functionalBlock = (FunctionalBlockEntity)cubeBlock;

					if ( commandParts[ 1 ].ToLower( ).Equals( "all" ) )
					{
						functionalBlock.Enabled = true;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "production" ) && cubeBlock is ProductionBlockEntity )
					{
						functionalBlock.Enabled = true;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "beacon" ) && cubeBlock is BeaconEntity )
					{
						functionalBlock.Enabled = true;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "tools" ) && ( cubeBlock is ShipToolBaseEntity || cubeBlock is ShipDrillEntity ) )
					{
						functionalBlock.Enabled = true;
						poweredOffCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "turrets" ) && ( cubeBlock is TurretBaseEntity ) )
					{
						functionalBlock.Enabled = true;
						poweredOffCount++;
					}

					if ( commandParts[ 1 ].ToLower( ).Equals( cubeBlock.Id.SubtypeName.ToLower( ) ) )
					{
						functionalBlock.Enabled = true;
						poweredOffCount++;
					}
				}
			}

			SendPrivateChatMessage( remoteUserId, "Turned on " + poweredOffCount + " blocks" );
		}

		protected void Command_Kick( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			//Get the steam id of the player
			string rawSteamId = commandParts[ 1 ];

			if ( rawSteamId.Length < 3 )
			{
				SendPrivateChatMessage( remoteUserId, "3 or more characters required to kick." );
				return;
			}

			ulong steamId;
			var playerItems = PlayerManager.Instance.PlayerMap.GetPlayerItemsFromPlayerName( rawSteamId );
			if ( playerItems.Count == 0 )
			{
				steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName( rawSteamId );
				if ( steamId == 0 )
					return;
			}
			else
			{
				if ( playerItems.Count > 1 )
				{
					SendPrivateChatMessage( remoteUserId, "There is more than one player with the specified name;" );

					string playersString = playerItems.Aggregate( string.Empty, ( current, playeritem ) => string.Format( "{0}{1} ", current, playeritem.name  ) );

					SendPrivateChatMessage( remoteUserId, playersString );
					return;
				}

				steamId = playerItems[ 0 ].steamId;
				if ( steamId == 0 )
					return;
			}

			if ( steamId.ToString( ).StartsWith( "9009" ) )
			{
				SendPrivateChatMessage( remoteUserId, string.Format( "Unable to kick player '{0}'.  This player is the server.", steamId ) );
				return;
			}

			PlayerManager.Instance.KickPlayer( steamId );
			SendPrivateChatMessage( remoteUserId, string.Format( "Kicked player '{0}' off of the server", ( playerItems.Count == 0 ? rawSteamId : playerItems[ 0 ].name ) ) );
		}

		protected void Command_Ban( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			//Get the steam id of the player
			string rawSteamId = commandParts[ 1 ];

			if ( rawSteamId.Length < 3 )
			{
				SendPrivateChatMessage( remoteUserId, "3 or more characters required to ban." );
				return;
			}

			ulong steamId;

			var playerItems = PlayerManager.Instance.PlayerMap.GetPlayerItemsFromPlayerName( rawSteamId );

			if ( playerItems.Count == 0 )
			{
				steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName( rawSteamId );
				if ( steamId == 0 )
					return;
			}
			else
			{
				if ( playerItems.Count > 1 )
				{
					SendPrivateChatMessage( remoteUserId, "There is more than one player with the specified name;" );

					string playersString = playerItems.Aggregate( string.Empty, ( current, playeritem ) => string.Format( "{0}{1} ", current, playeritem.name ) );

					SendPrivateChatMessage( remoteUserId, playersString );
					return;
				}

				steamId = playerItems[ 0 ].steamId;
				if ( steamId == 0 )
					return;
			}

			if ( steamId.ToString( ).StartsWith( "9009" ) )
			{
				SendPrivateChatMessage( remoteUserId, string.Format( "Unable to ban player '{0}'.  This player is the server.", steamId ) );
				return;
			}

			PlayerManager.Instance.BanPlayer( steamId );

			SendPrivateChatMessage( remoteUserId, string.Format( "Banned '{0}' and kicked them off of the server", ( playerItems.Count == 0 ? rawSteamId : playerItems[ 0 ].name ) ) );
		}

		protected void Command_Unban( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			string[ ] commandParts = chatEvent.Message.Split( ' ' );
			int paramCount = commandParts.Length - 1;

			if ( paramCount != 1 )
				return;

			//Get the steam id of the player
			string rawSteamId = commandParts[ 1 ];

			if ( rawSteamId.Length < 3 )
			{
				SendPrivateChatMessage( remoteUserId, "3 or more characters required to unban." );
				return;
			}

			ulong steamId;

			var playerItems = PlayerManager.Instance.PlayerMap.GetPlayerItemsFromPlayerName( rawSteamId );

			if ( playerItems.Count == 0 )
			{
				steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName( rawSteamId );
				if ( steamId == 0 )
					return;
			}
			else
			{
				if ( playerItems.Count > 1 )
				{
					SendPrivateChatMessage( remoteUserId, "There is more than one player with the specified name;" );

					string playersString = playerItems.Aggregate( string.Empty, ( current, playeritem ) => string.Format( "{0}{1} ", current, playeritem.name ) );

					SendPrivateChatMessage( remoteUserId, playersString );
					return;
				}

				steamId = playerItems[ 0 ].steamId;
				if ( steamId == 0 )
					return;
			}

			PlayerManager.Instance.UnBanPlayer( steamId );

			SendPrivateChatMessage( remoteUserId, string.Format( "Unbanned '{0}'", ( playerItems.Count == 0 ? rawSteamId : playerItems[ 0 ].name ) ) );
		}

		#endregion

		#endregion
	}
}
