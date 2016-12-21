using System.Threading.Tasks;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Game.Weapons;
using Sandbox.Game.World;
using SEModAPI.API.Definitions;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Network;

namespace SEModAPIExtensions.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using Sandbox;
    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Engine.Multiplayer;
    using Sandbox.Game.Replication;
    using Sandbox.ModAPI;
    using SEModAPI.API;
    using SEModAPI.API.Sandbox;
    using SEModAPI.API.Utility;
    using SEModAPIInternal.API.Common;
    using SEModAPIInternal.API.Entity;
    using SEModAPIInternal.API.Entity.Sector.SectorObject;
    using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
    using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
    using SEModAPIInternal.API.Server;
    using SEModAPIInternal.Support;
    using SteamSDK;
    using VRage;
    using VRage.FileSystem;
    using VRage.ModAPI;
    using VRage.ObjectBuilders;
    using VRageMath;

    public delegate void ChatEventDelegate( ulong steamId, string playerName, string message );
    public class ChatManager
    {
        private static bool _enableData;
        private Random _random = new Random();
        private List<Guid> _messageUniqueIds = new List<Guid>(50);
        private byte[] _lastMessage;
        private DateTime _lastReceived = DateTime.Now;

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

        public static string ChatMessageStructNamespace = "Sandbox.Engine.Multiplayer";
        public static string ChatMessageStructClass = "ChatMsg";

        public static string ChatMessageMessageField = "Text";

        public event ChatEventDelegate ChatMessage;

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
            RegisterChatCommand( clearCommand );
            RegisterChatCommand( listCommand );
            RegisterChatCommand( kickCommand );
            RegisterChatCommand( banCommand );
            RegisterChatCommand( unbanCommand );
            RegisterChatCommand( asyncSaveCommand );

            ApplicationLog.BaseLog.Info( "Finished loading ChatManager" );
        }

        #endregion

        #region "Properties"

        public static ChatManager Instance
        {
            get { return m_instance ?? (m_instance = new ChatManager( )); }
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

        public void AddChatHistory( ChatEvent chatItem )
        {
            m_resourceLock.AcquireExclusive( );
            m_chatHistory.Add( chatItem );
            m_resourceLock.ReleaseExclusive( );
            if ( chatItem.RemoteUserId == 0 )
                ApplicationLog.ChatLog.Info( "Chat - Server: " + chatItem.Message );
            else
                ApplicationLog.ChatLog.Info( string.Format( "Chat - Client '{0}': {1}", PlayerMap.Instance.GetFastPlayerNameFromSteamId( chatItem.RemoteUserId ), chatItem.Message ) );
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
                Type type = typeof( ChatMsg );
                bool result = true;
                result &= Reflection.HasField( type, ChatMessageMessageField );

                return result;
            }
            catch ( Exception ex )
            {
                ApplicationLog.BaseLog.Error( ex );
                return false;
            }
        }

        private void SetupChatHandlers( )
        {
            if ( m_chatHandlerSetup )
                return;

            if ( !MySandboxGameWrapper.IsGameStarted )
                return;

            //check if we have the Essentials client mod installed so we can use dataMessages instead of chat messages
            if ( !_enableData )
            {
                if ( Server.Instance.Config.Mods.Contains( "559202083" ) || Server.Instance.Config.Mods.Contains( "558596580" ) )
                {
                    _enableData = true;
                    ApplicationLog.Info( "Found Essentials client mod, enabling data messages" );
                }
            }
                
            try
            {
                object netManager = NetworkManager.GetNetworkManager( );
                if ( netManager == null )
                    return;

                Action<ulong, string, ChatEntryTypeEnum> chatHook = ReceiveChatMessage;
                ServerNetworkManager.Instance.RegisterChatReceiver( chatHook );
                MyAPIGateway.Multiplayer.RegisterMessageHandler( 9001, ReceiveDataMessage );
                
                m_chatHandlerSetup = true;
            }
            catch ( Exception ex )
            {
                ApplicationLog.BaseLog.Error( ex );
            }
        }

        [Obsolete("Create the ChatMsg struct directly")]
        protected Object CreateChatMessageStruct( string message )
        {
            Type chatMessageStructType = typeof( ChatMsg );
            FieldInfo messageField = chatMessageStructType.GetField( ChatMessageMessageField );

            Object chatMessageStruct = Activator.CreateInstance( chatMessageStructType );
            messageField.SetValue( chatMessageStruct, message );

            return chatMessageStruct;
        }

        public static bool EnableData
        {
            get
            {
                return _enableData;
            }
        }

        public class MessageRecieveItem
        {
            public ulong fromID { get; set; }
            public long msgID { get; set; }
            public string message { get; set; }
        }


        public class ServerMessageItem
        {
            public string From { get; set; }
            public string Message { get; set; }
        }

        protected void ReceiveDataMessage( byte[] fullData )
        {
            Task.Run(() =>
                     {
                         string text;
                         MessageRecieveItem item;
                         byte[] guidBytes = new byte[16];
                         Array.Copy(fullData, guidBytes, 16);

                         Guid uniqueId = new Guid(guidBytes);

                         if (_messageUniqueIds.Contains(uniqueId))
                         {
                             ApplicationLog.BaseLog.Debug("Received duplicate chat message (hash).");
                             return;
                         }

                         if (_messageUniqueIds.Count >= 50)
                             _messageUniqueIds.RemoveAt(0);

                         _messageUniqueIds.Add(uniqueId);

                         byte[] data = new byte[fullData.Length - 16];
                         Array.Copy(fullData, 16, data, 0, data.Length);

                         if (DateTime.Now - _lastReceived < TimeSpan.FromMilliseconds(100) && CompareBytes(data, _lastMessage))
                         {
                             ApplicationLog.BaseLog.Debug("Received duplicate chat message (value).");
                             _lastReceived = DateTime.Now;
                             return;
                         }

                         _lastReceived = DateTime.Now;
                         _lastMessage = data;

                         try
                         {
                             text = Encoding.UTF8.GetString(data);

                             item = MyAPIGateway.Utilities.SerializeFromXML<MessageRecieveItem>(text);
                         }
                         catch (Exception ex)
                         {
                             ApplicationLog.BaseLog.Error(ex, "Failed to deserialize data message.");
                             return;
                         }

                         if (ExtenderOptions.IsDebugging)
                             ApplicationLog.BaseLog.Debug(text);

                         if (item.msgID == 5010)
                         {
                             string playerName = PlayerMap.Instance.GetPlayerNameFromSteamId(item.fromID);

                             bool commandParsed = ParseChatCommands(item.message, item.fromID);

                             m_chatMessages.Add(string.Format("{0}: {1}", playerName, item.message));
                             ApplicationLog.ChatLog.Info("Chat - Client '{0}': {1}", playerName, item.message);

                             ChatEvent chatEvent = new ChatEvent(ChatEventType.OnChatReceived, DateTime.Now, item.fromID, 0, item.message, 0);
                             if (!commandParsed)
                                 Instance.AddEvent(chatEvent);

                             m_resourceLock.AcquireExclusive();
                             m_chatHistory.Add(chatEvent);
                             m_resourceLock.ReleaseExclusive();
                         }
                         else if (item.msgID == 5011)
                         {
                             ApplicationLog.Info($"Sending chat override to {item.fromID}");
                             SendDataMessage(new byte[0], 5007, item.fromID);
                         }
                         else if (item.msgID == 6000)
                         {
                             if (MySession.Static.IsUserAdmin(item.fromID))
                             {
                                 ScriptedChatMsg msg = new ScriptedChatMsg
                                                       {
                                                           Author = PlayerMap.Instance.GetPlayerNameFromSteamId(item.fromID),
                                                           Font = MyFontEnum.Green,
                                                           Text = item.message,
                                                       };

                                 var messageMethod = typeof(MyMultiplayerBase).GetMethod("OnScriptedChatMessageRecieved", BindingFlags.NonPublic | BindingFlags.Static);
                                 ServerNetworkManager.Instance.RaiseStaticEvent(messageMethod, msg);
                             }
                             else
                             {
                                 ChatMsg msg = new ChatMsg
                                               {
                                                   Author = item.fromID,
                                                   Text = item.message
                                               };
                                 var messageMethod = typeof(MyMultiplayerBase).GetMethod("OnChatMessageRecieved", BindingFlags.NonPublic | BindingFlags.Static);
                                 ServerNetworkManager.Instance.RaiseStaticEvent(messageMethod, msg);
                             }
                         }
                         else
                             ApplicationLog.Info("Unknown data message type: " + item.msgID);
                     });
        }

        private static bool CompareBytes(byte[] byteA, byte[] byteB)
        {
            if ( byteA == null || byteB == null )
                return false;

            if (byteA.Length != byteB.Length)
                return false;

            for (int i = 0; i < byteA.Length; ++i)
            {
                if (byteA[i] != byteB[i])
                    return true;
            }

            return false;
        }

        public void SendDataMessage( string message, ulong userId = 0 )
        {
            ServerMessageItem item = new ServerMessageItem( );
            item.From = Server.Instance.Config.ServerChatName;
            item.Message = message;
            
            string messageString = MyAPIGateway.Utilities.SerializeToXML( item );
            byte[ ] data = Encoding.UTF8.GetBytes( messageString );
            long msgId = 5003;

            byte[] guidBytes = Guid.NewGuid().ToByteArray();

            //this block adds the length and message id to the outside of the message packet
            //so the mod can quickly determine where the message should go
            byte[] newData = new byte[sizeof(long) + data.Length + guidBytes.Length];
            guidBytes.CopyTo(newData, 0);
            BitConverter.GetBytes(msgId).CopyTo(newData, guidBytes.Length);
            data.CopyTo(newData, sizeof(long) + guidBytes.Length);

            SandboxGameAssemblyWrapper.Instance.GameAction(() =>
            {
                if (userId == 0)
                    ServerNetworkManager.Instance.BroadcastModMessage(9000, newData);
                else
                    ServerNetworkManager.Instance.SendModMessageTo(9000, newData, userId);
            });
        }

        public void SendDataMessage(byte[] data, long msgId, ulong userId)
        {
            byte[] guidBytes = Guid.NewGuid().ToByteArray();

            //this block adds the length and message id to the outside of the message packet
            //so the mod can quickly determine where the message should go
            byte[] newData = new byte[sizeof(long) + data.Length + guidBytes.Length];
            guidBytes.CopyTo(newData, 0);
            BitConverter.GetBytes(msgId).CopyTo(newData, guidBytes.Length);
            data.CopyTo(newData, sizeof(long) + guidBytes.Length);

            SandboxGameAssemblyWrapper.Instance.GameAction(() =>
            {
                if (userId == 0)
                    ServerNetworkManager.Instance.BroadcastModMessage(9000, newData);
                else
                    ServerNetworkManager.Instance.SendModMessageTo(9000, newData, userId);
            });
        }
        
        protected void ReceiveChatMessage( ulong remoteUserId, string message, ChatEntryTypeEnum entryType )
        {
            Task.Run( () =>
            {
                string playerName = PlayerMap.Instance.GetPlayerNameFromSteamId( remoteUserId );

                bool commandParsed = ParseChatCommands( message, remoteUserId );

                if ( !commandParsed && entryType == ChatEntryTypeEnum.ChatMsg )
                {
                    m_chatMessages.Add( string.Format( "{0}: {1}", playerName, message ) );
                    ApplicationLog.ChatLog.Info( "Chat - Client '{0}': {1}", playerName, message );
                }

                ChatEvent chatEvent = new ChatEvent( ChatEventType.OnChatReceived, DateTime.Now, remoteUserId, 0, message, 0 );
                Instance.AddEvent( chatEvent );

                m_resourceLock.AcquireExclusive();
                m_chatHistory.Add( chatEvent );
                OnChatMessage( remoteUserId, playerName, message );
                m_resourceLock.ReleaseExclusive();
            } );
        }

		public void SendPrivateChatMessage( ulong remoteUserId, string message )
		{
			if ( !MySandboxGameWrapper.IsGameStarted )
				return;
			if ( string.IsNullOrEmpty( message ) )
				return;

			try
			{
			    if (remoteUserId != 0)
			    {
			        ScriptedChatMsg msg = new ScriptedChatMsg
			                              {
			                                  Author = Server.Instance.Config.ServerChatName,
			                                  Font = MyFontEnum.Red,
			                                  Text = message,
			                                  Target = PlayerMap.Instance.GetFastPlayerIdFromSteamId(remoteUserId),
			                              };

			        var messageMethod = typeof(MyMultiplayerBase).GetMethod("OnScriptedChatMessageRecieved", BindingFlags.NonPublic | BindingFlags.Static);
			        ServerNetworkManager.Instance.RaiseStaticEvent(messageMethod, remoteUserId, msg);

			        ScanGPSAndAdd(message, msg.Target);
			    }

			    m_chatMessages.Add( string.Format( "Server: {0}", message ) );

				ApplicationLog.ChatLog.Info( string.Format( "Chat - Server: {0}", message ) );

				ChatEvent chatEvent = new ChatEvent( ChatEventType.OnChatSent, DateTime.Now, 0, remoteUserId, message, 0 );
				Instance.AddEvent( chatEvent );

				m_resourceLock.AcquireExclusive( );
				m_chatHistory.Add( chatEvent );
				OnChatMessage( 0, "Server", message );
				m_resourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public void SendPublicChatMessage( string message )
		{
			if ( !MySandboxGameWrapper.IsGameStarted )
				return;
			if ( string.IsNullOrEmpty( message ) )
				return;

			bool commandParsed = ParseChatCommands( message );

			try
			{
                if ( !commandParsed && message[0] != '/' )
                {
                    ScriptedChatMsg msg = new ScriptedChatMsg
                    {
                        Author = Server.Instance.Config.ServerChatName,
                        Font = MyFontEnum.Red,
                        Text = message,
                    };

                    var messageMethod = typeof(MyMultiplayerBase).GetMethod("OnScriptedChatMessageRecieved", BindingFlags.NonPublic | BindingFlags.Static);
                    ServerNetworkManager.Instance.RaiseStaticEvent(messageMethod, msg);

                    ScanGPSAndAdd(message);
                }

				//Send a loopback chat event for server-sent messages
				ChatEvent selfChatEvent = new ChatEvent( ChatEventType.OnChatReceived, DateTime.Now, 0, 0, message, 0 );
				Instance.AddEvent( selfChatEvent );

				m_resourceLock.AcquireExclusive( );
				m_chatHistory.Add( selfChatEvent );
				OnChatMessage( 0, "Server", message );
				m_resourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

        public int ScanGPSAndAdd(string input, long playerId = -1)
        {
            int count = 0;
            foreach (Match match in Regex.Matches(input, @"GPS:([^:]{0,32}):([\d\.-]*):([\d\.-]*):([\d\.-]*):"))
            {
                String name = match.Groups[1].Value;
                double x, y, z;
                try
                {
                    x = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
                    x = Math.Round(x, 2);
                    y = double.Parse(match.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);
                    y = Math.Round(y, 2);
                    z = double.Parse(match.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture);
                    z = Math.Round(z, 2);
                }
                catch (SystemException)
                {
                    continue;//search for next GPS in the input
                }

                MyGps newGps = new MyGps()
                {
                    Name = name,
                    Description = null,
                    Coords = new Vector3D(x, y, z),
                    ShowOnHud = false
                };
                newGps.UpdateHash();
                if (playerId > -1)
                    MyAPIGateway.Session.GPS.AddGps(playerId, newGps);
                else
                {
                    foreach(var player in MySession.Static.Players.GetOnlinePlayers())
                        MyAPIGateway.Session.GPS.AddGps(player.Identity.IdentityId, newGps);
                }
                //MySession.Static.Gpss.SendAddGps(MySession.Static.LocalPlayerId, ref newGps);
                ++count;
            }
            return count;
        }

		protected bool ParseChatCommands( string message, ulong remoteUserId = 0 )
		{
			try
			{
				if ( string.IsNullOrEmpty( message ) )
					return false;

				//Skip if message doesn't have leading forward slash
				if ( message[ 0 ] != '/' )
					return false;

				List<string> commandParts = CommandParser.GetCommandParts( message );
				if ( commandParts.Count == 0 )
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
						ApplicationLog.BaseLog.Error( ex );
					}
				}

				return foundMatch;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
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
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;
		    MyEntity[] entities = new MyEntity[0];
		    SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities().ToArray());
            var toRemove = new HashSet<MyEntity>();
            
			//All entities
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "all" ) )
			{
				//All cube grids that have no beacon or only a beacon with no name
				if ( commandParts[ 2 ].ToLower( ).Equals( "nobeacon" ) )
				{
				    int removeCount = 0;
				    Parallel.ForEach(MyCubeGridGroups.Static.Logical.Groups, group =>
				                                                             {
				                                                                 bool found = false;
				                                                                 foreach (var node in group.Nodes)
				                                                                 {
				                                                                     var grid = node.NodeData;

				                                                                     if (grid.MarkedForClose || grid.Closed)
				                                                                         continue;

				                                                                     if (grid.CubeBlocks.OfType<MyBeacon>().Any())
				                                                                     {
				                                                                         found = true;
				                                                                         break;
				                                                                     }
				                                                                 }

				                                                                 if (!found)
				                                                                     return;

				                                                                 removeCount++;
				                                                                 foreach (var closeNode in group.Nodes)
				                                                                 {
				                                                                     SandboxGameAssemblyWrapper.Instance.BeginGameAction(()=>closeNode.NodeData.Close(), null, null);
				                                                                 }
				                                                             });

				    SendPrivateChatMessage(remoteUserId, $"{removeCount} cube grids have been removed");
				}
				//All cube grids that have no power
				else if ( commandParts[ 2 ].ToLower( ).Equals( "nopower" ) )
				{
					//List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>( );
					//List<CubeGridEntity> entitiesToDispose = entities.Where( entity => entity.TotalPower <= 0 ).ToList( );

					//foreach ( CubeGridEntity entity in entitiesToDispose )
					//{
					//	entity.Dispose( );
					//}

					//SendPrivateChatMessage( remoteUserId, string.Format( "{0} cube grids have been removed", entitiesToDispose.Count ) );
					SendPrivateChatMessage( remoteUserId, "Unpowered grids removal temporarily unavailable in this version." );
				}
				else if ( commandParts[ 2 ].ToLower( ).Equals( "floatingobjects" ) )	//All floating objects
				{
				    int count = 0;
				    foreach (var floating in entities.Where(e => e is MyFloatingObject))
				    {
				        count++;
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => floating.Close(), null, null);
				    }
                    SendPrivateChatMessage(remoteUserId, $"Removed {count} floating objects");

                }
				else
				{
					string entityName = commandParts[ 2 ];
					if ( commandParts.Count > 3 )
					{
						for ( int i = 3; i < commandParts.Count; i++ )
						{
							entityName += " " + commandParts[ i ];
						}
					}

					int matchingEntitiesCount = 0;
					foreach ( var entity in entities )
					{
					    if (entity.MarkedForClose || entity.Closed)
					        continue;

						bool isMatch = Regex.IsMatch( entity.Name, entityName, RegexOptions.IgnoreCase );
						if ( !isMatch )
							continue;

					    SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => entity.Close(), null, null);

						matchingEntitiesCount++;
					}

					SendPrivateChatMessage( remoteUserId, $"{matchingEntitiesCount} objects have been removed");
				}
			}

			//All non-static cube grids
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "ship" ) )
			{
				//That have no beacon or only a beacon with no name
				if ( commandParts[ 2 ].ToLower( ).Equals( "nobeacon" ) )
				{
                    int removeCount = 0;
                    Parallel.ForEach(MyCubeGridGroups.Static.Logical.Groups, group =>
                                                                             {
                                                                                 bool found = false;
                                                                                 foreach (var node in group.Nodes)
                                                                                 {
                                                                                     var grid = node.NodeData;

                                                                                     if (grid.MarkedForClose || grid.Closed)
                                                                                         continue;

                                                                                     if (grid.IsStatic)
                                                                                         continue;

                                                                                     if (grid.CubeBlocks.OfType<MyBeacon>().Any())
                                                                                     {
                                                                                         found = true;
                                                                                         break;
                                                                                     }
                                                                                 }

                                                                                 if (!found)
                                                                                     return;

                                                                                 removeCount++;
                                                                                 foreach (var closeNode in group.Nodes)
                                                                                 {
                                                                                     SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => closeNode.NodeData.Close(), null, null);
                                                                                 }
                                                                             });

                    SendPrivateChatMessage(remoteUserId, $"{removeCount} cube grids have been removed");
                }

			//All static cube grids
			if ( paramCount > 1 && commandParts[ 1 ].ToLower( ).Equals( "station" ) )
			{
                    int removeCount = 0;
                    Parallel.ForEach(MyCubeGridGroups.Static.Logical.Groups, group =>
                                                                             {
                                                                                 bool found = false;
                                                                                 foreach (var node in group.Nodes)
                                                                                 {
                                                                                     var grid = node.NodeData;

                                                                                     if (grid.MarkedForClose || grid.Closed)
                                                                                         continue;

                                                                                     if(!grid.IsStatic)

                                                                                     if (grid.CubeBlocks.OfType<MyBeacon>().Any())
                                                                                     {
                                                                                         found = true;
                                                                                         break;
                                                                                     }
                                                                                 }

                                                                                 if (!found)
                                                                                     return;

                                                                                 removeCount++;
                                                                                 foreach (var closeNode in group.Nodes)
                                                                                 {
                                                                                     SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => closeNode.NodeData.Close(), null, null);
                                                                                 }
                                                                             });

                    SendPrivateChatMessage(remoteUserId, $"{removeCount} cube grids have been removed");
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
				List<MyFaction> factionsToRemove = new List<MyFaction>( );
				if ( commandParts[ 2 ].ToLower( ).Equals( "empty" ) )
				{
                    factionsToRemove.AddRange( MySession.Static.Factions.Select( x => x.Value ).Where( v => !v.Members.Any() ) );
				}
				if ( commandParts[ 2 ].ToLower( ).Equals( "nofounder" ) )
				{
					foreach ( var entry in MySession.Static.Factions.Select( x => x.Value ) )
					{
					    bool founderMatch = entry.Members.Any( m => m.Value.IsFounder );

						if ( !founderMatch )
							factionsToRemove.Add( entry );
					}
				}
				if ( commandParts[ 2 ].ToLower( ).Equals( "noleader" ) )
				{
                    foreach (var entry in MySession.Static.Factions.Select(x => x.Value))
					{
						bool founderMatch = entry.Members.Any( member => member.Value.IsFounder || member.Value.IsLeader );

						if ( !founderMatch )
							factionsToRemove.Add( entry );
					}
				}

				foreach ( var entry in factionsToRemove )
				{
                    MyFactionCollection.RemoveFaction( entry.FactionId );
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
                    
					foreach ( var entity in entities )
					{
						if ( entity.EntityId != entityId )
							continue;

					    SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => entity.Close(), null, null);
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}
		}

		protected void Command_Teleport( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

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

					var entities = new HashSet<MyEntity>();
				    SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
					foreach ( var entity in entities )
					{
						if ( entity.EntityId != entityId )
							continue;

						Vector3D newPosition = new Vector3D( x, y, z );
						SandboxGameAssemblyWrapper.Instance.GameAction(()=>entity.PositionComp.SetPosition(newPosition));

						SendPrivateChatMessage( remoteUserId, $"Entity '{entity.EntityId}' has been moved to '{newPosition}'");
					}
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
				}
			}
		}

        protected void Command_Stop(ChatEvent chatEvent)
        {
            ulong remoteUserId = chatEvent.RemoteUserId;
            List<string> commandParts = CommandParser.GetCommandParts(chatEvent.Message);
            int paramCount = commandParts.Count - 1;

            if (paramCount != 1)
                return;
            long entityId;
            if (commandParts[1].ToLower().Equals("all"))
            {
                int entitiesStoppedCount = 0;
                HashSet<MyEntity> entities = new HashSet<MyEntity>();
                SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
                foreach (var entity in entities)
                {
                    if (entity.Physics == null)
                        continue;

                    if (Vector3D.IsZero(entity.Physics.LinearVelocity) && Vector3D.IsZero(entity.Physics.AngularVelocity))
                        continue;

                    SandboxGameAssemblyWrapper.Instance.BeginGameAction(() =>
                                                                        {
                                                                            try
                                                                            {
                                                                                entity.Physics.Clear();
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                ApplicationLog.BaseLog.Info(ex);
                                                                            }
                                                                        }, null, null);
                    entitiesStoppedCount++;
                }
                SendPrivateChatMessage(remoteUserId, $"{entitiesStoppedCount} entities are no longer moving or rotating");
            }
            else if (long.TryParse(commandParts[1], out entityId))
            {
                MyEntity entity = null;
                SandboxGameAssemblyWrapper.Instance.GameAction(() => entity = MyEntities.GetEntityByIdOrDefault(entityId));
                if (entity?.Physics == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find entity with ID {entityId}");
                    return;
                }

                SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => entity.Physics.Clear(), null, null);
                SendPrivateChatMessage(remoteUserId, $"Entity '{entity.EntityId}: {entity.DisplayName}' is no longer moving or rotating");
            }
            else
            {
                HashSet<MyEntity> entities = new HashSet<MyEntity>();
                SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
                string nameLower = commandParts[1].ToLower();

                MyEntity entity = entities.FirstOrDefault((e) => e.DisplayName!=null && e.DisplayName.ToLower().Equals(nameLower));

                if (entity?.Physics == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find entity named {commandParts[1]}");
                    return;
                }
                SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => entity.Physics.Clear(), null, null);
                SendPrivateChatMessage(remoteUserId, $"Entity '{entity.EntityId}: {entity.DisplayName}' is no longer moving or rotating");
            }
        }

        protected void Command_GetId(ChatEvent chatEvent)
        {
            ulong remoteUserId = chatEvent.RemoteUserId;
            List<string> commandParts = CommandParser.GetCommandParts(chatEvent.Message);
            int paramCount = commandParts.Count - 1;

            if (paramCount != 1)
                return;
            
            HashSet<MyEntity> entities = new HashSet<MyEntity>();
            SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
            string nameLower = commandParts[1].ToLower();

            MyEntity entity = entities.FirstOrDefault((e) => e.DisplayName != null && e.DisplayName.ToLower().Equals(nameLower));

            if (entity == null)
            {
                SendPrivateChatMessage(remoteUserId, $"Couldn't find entity with name {commandParts[1]}");
                return;
            }

            SendPrivateChatMessage(remoteUserId, $"Entity ID is '{entity.EntityId}'");
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

        protected void Command_Owner(ChatEvent chatEvent)
        {
            ulong remoteUserId = chatEvent.RemoteUserId;
            List<string> commandParts = CommandParser.GetCommandParts(chatEvent.Message);
            int paramCount = commandParts.Count - 1;

            if (paramCount != 2)
                return;

            long entityId;
            long ownerId;

            MyEntity entity = null;
            MyPlayer player;
            if (!long.TryParse(commandParts[1], out entityId))
            {
                HashSet<MyEntity> entities = new HashSet<MyEntity>();
                SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
                string nameLower = commandParts[1].ToLower();

                entity = entities.FirstOrDefault((e) => e.DisplayName.ToLower().Equals(nameLower));

                if (entity == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find entity with name {commandParts[1]}");
                    return;
                }
            }
            else
            {
                SandboxGameAssemblyWrapper.Instance.GameAction(() => entity = MyEntities.GetEntityByIdOrDefault(entityId));
                if (entity == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find entity with ID {entityId}");
                    return;
                }
            }

            if (!long.TryParse(commandParts[2], out ownerId))
            {
                string nameLower = commandParts[2].ToLower();
                player = MySession.Static.Players.GetOnlinePlayers().FirstOrDefault((p) => p.DisplayName.ToLower().Equals(nameLower));

                if (player == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find player with name {commandParts[2]}");
                    return;
                }
            }
            else
            {
                player = MySession.Static.Players.GetOnlinePlayers().FirstOrDefault(p => p.Identity.IdentityId == ownerId);
                if (player == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find player with name {commandParts[2]}");
                    return;
                }
            }

            var grid = entity as MyCubeGrid;
            if (grid == null)
            {
                SendPrivateChatMessage(remoteUserId, "Found an entity, but it isn't a grid!");
                return;
            }

            SandboxGameAssemblyWrapper.Instance.GameAction(() => grid.ChangeGridOwner(player.Identity.IdentityId, MyOwnershipShareModeEnum.Faction));
            SendPrivateChatMessage(remoteUserId, $"Changed ownership of grid {entity.DisplayName} to {player.DisplayName}");
        }

        protected void Command_Export( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

            if (paramCount != 1)
                return;

            int count = 0;
            try
            {
                HashSet<MyEntity> entities = new HashSet<MyEntity>();
                SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
                string nameLower = commandParts[1].ToLower();
                count = 1;
                MyEntity entity = entities.FirstOrDefault((e) => e?.DisplayName!=null && e.DisplayName.ToLower().Equals(nameLower));

                if (entity == null)
                {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find entity with name {commandParts[1]}");
                    return;
                }
                count = 3;
                var grid = entity as MyCubeGrid;
                if (grid == null)
                {
                    SendPrivateChatMessage(remoteUserId, "Failed to export grid");
                    return;
                }
                count = 4;
                string modPath = Path.GetFullPath(Path.Combine(MySession.Static.CurrentPath, @"..\..\Mods"));
                if (!Directory.Exists(modPath))
                {
                    SendPrivateChatMessage(remoteUserId, "Failed to export grid");
                    return;
                }

                string fileName = grid.DisplayName.ToLower();
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                string cleanFileName = rgx.Replace(fileName, string.Empty);
                count = 5;
                string exportPath = Path.Combine(modPath, "Exports");
                if (!Directory.Exists(exportPath))
                    Directory.CreateDirectory(exportPath);
                FileInfo exportFile = new FileInfo(Path.Combine(exportPath, cleanFileName + ".sbc"));
                count = 6;
                MyObjectBuilderSerializer.SerializeXML(exportFile.FullName, false, grid.GetObjectBuilder());

                SendPrivateChatMessage(remoteUserId, string.Format("Entity '{0}' has been exported to Mods/Exports", entity.EntityId));

            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Warn(count);
                ApplicationLog.BaseLog.Error(ex);
            }
		}

		protected void Command_Import( ChatEvent chatEvent )
        {
            ulong remoteUserId = chatEvent.RemoteUserId;
            List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

		    if (paramCount < 1 || paramCount > 2)
                return;

		    double x = 0;
		    double y = 0;
		    double z = 0;
		    bool validPosition = false;
            
		    if (paramCount == 2)
		    {
		        string[] splits = commandParts[2].Split(',');
		        if (splits.Length == 3)
		        {
		            validPosition = true;
		            if (!double.TryParse(splits[0], out x))
		                validPosition = false;
		            if (!double.TryParse(splits[1], out y))
		                validPosition = false;
		            if (!double.TryParse(splits[2], out z))
		                validPosition = false;
		        }
		    }

		    try
		    {
		        string fileName = commandParts[ 1 ];
		        Regex rgx = new Regex( "[^a-zA-Z0-9]" );
		        string cleanFileName = rgx.Replace( fileName, string.Empty );
                    
		        string modPath = Path.GetFullPath(Path.Combine(MySession.Static.CurrentPath, @"..\..\Mods"));
		        if (!Directory.Exists(modPath))
		        {
                    SendPrivateChatMessage(remoteUserId,"Couldn't find Mods directory");
                    return;
		        }
		        string exportPath = Path.Combine( modPath, "Exports" );
		        if (!Directory.Exists(exportPath))
		        {
                    SendPrivateChatMessage(remoteUserId, "Couldn't find Mods\\Exports directory");
                    return;
		        }
		        FileInfo importFile = new FileInfo( Path.GetFullPath(Path.Combine( exportPath, cleanFileName + ".sbc" )) );
		        if (!importFile.Exists)
		        {
                    SendPrivateChatMessage(remoteUserId, $"Couldn't find {cleanFileName}.sbc");
                    return;
		        }
		        MyObjectBuilder_CubeGrid builder;
		        if (!MyObjectBuilderSerializer.DeserializeXML(importFile.FullName, out builder))
		        {
                    SendPrivateChatMessage(remoteUserId, "Failed to import grid");
                    return;
		        }
		        MyEntity entity = null;
		        SandboxGameAssemblyWrapper.Instance.GameAction(() =>
		                                                       {
		                                                           MyEntities.RemapObjectBuilder(builder);
		                                                           entity = MyEntities.CreateFromObjectBuilder(builder);
		                                                           if (validPosition)
		                                                           {
		                                                               var pos=  new Vector3D(x,y,z);
		                                                               entity.PositionComp.SetPosition(pos);
		                                                           }
		                                                           MyEntities.Add(entity);
		                                                       });

		        SendPrivateChatMessage(remoteUserId, $"Imported entity {entity.DisplayName ?? commandParts[1]} at location {entity.PositionComp.GetPosition()}");
		    }
		    catch ( Exception ex )
		    {
		        ApplicationLog.BaseLog.Error( ex );
		    }
		}

		protected void Command_Clear( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

			if ( paramCount != 1 )
				return;

		    var entities = new HashSet<MyEntity>();
		    SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());
			int queueCount = 0;
			foreach ( var entity in entities )
			{
			    var grid = entity as MyCubeGrid;
			    if (grid == null)
			        continue;

				foreach ( var slimBlock in grid.CubeBlocks.ToArray() )
				{
				    var cubeBlock = slimBlock?.FatBlock;
				    if (cubeBlock == null)
				        continue;

					if ( commandParts[ 1 ].ToLower( ).Equals( "productionqueue" ) && cubeBlock is MyProductionBlock )
					{
						var block = (MyProductionBlock)cubeBlock;
						block.ClearQueue( );
						queueCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "refineryqueue" ) && cubeBlock is MyRefinery )
					{
						var block = (MyRefinery)cubeBlock;
						block.ClearQueue( );
						queueCount++;
					}
					if ( commandParts[ 1 ].ToLower( ).Equals( "assemblerqueue" ) && cubeBlock is MyAssembler)
					{
						var block = (MyAssembler)cubeBlock;
						block.ClearQueue( );
						queueCount++;
					}
				    if (commandParts[1].ToLower().Equals("projection") && cubeBlock is MyProjectorBase)
				    {
				        var block = (MyProjectorBase)cubeBlock;
                        block.SendRemoveProjection();
				        queueCount++;
				    }
				}
			}
		    if (!commandParts[1].ToLower().Equals("projection"))
		        SendPrivateChatMessage(remoteUserId, $"Cleared the production queue of {queueCount} blocks");
		    else
		        SendPrivateChatMessage(remoteUserId, $"Cleared projection from {queueCount} blocks");
		}

		protected void Command_List( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

			if ( paramCount != 1 )
				return;

            var entities = new HashSet<MyEntity>();
		    SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities());

			if ( commandParts[ 1 ].ToLower( ).Equals( "all" ) )
			{
				SendPrivateChatMessage( remoteUserId, "Total entities: '" + entities.Count + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "cubegrid" ) )
			{
				SendPrivateChatMessage( remoteUserId, "Cubegrid entities: '" + entities.Count(e => e is MyCubeGrid) + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "character" ) )
			{
				SendPrivateChatMessage( remoteUserId, "Character entities: '" + entities.Count(e => e is MyCharacter) + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "voxel" ) )
			{
				SendPrivateChatMessage( remoteUserId, "Voxelmap entities: '" + entities.Count(e => e is MyVoxelBase) + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "meteor" ) )
			{
				SendPrivateChatMessage( remoteUserId, "Meteor entities: '" + entities.Count(e => e is MyMeteor) + "'" );
			}
			if ( commandParts[ 1 ].ToLower( ).Equals( "floatingobject" ) )
			{
				SendPrivateChatMessage( remoteUserId, "Floating object entities: '" + entities.Count(e => e is MyFloatingObject) + "'" );
			}
		}

		protected void Command_Off( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

			if ( paramCount != 1 )
				return;

			MyEntity[] entities = new MyEntity[0];
		    SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities().ToArray());
			int poweredOffCount = 0;
			foreach ( var entity in entities )
			{
			    var grid = entity as MyCubeGrid;
			    if (grid?.Physics == null || grid.MarkedForClose)
			        continue;
			    foreach (var cubeBlock in grid.GetFatBlocks().ToArray())
			    {
			        if (!(cubeBlock is MyFunctionalBlock))
			            continue;

			        var functionalBlock = (MyFunctionalBlock)cubeBlock;

			        if (commandParts[1].ToLower().Equals("all"))
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            poweredOffCount++;
			        }
			        if (commandParts[1].ToLower().Equals("production") && cubeBlock is MyProductionBlock)
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            poweredOffCount++;
			        }
			        if (commandParts[1].ToLower().Equals("beacon") && cubeBlock is MyBeacon)
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            //BeaconEntity beacon = (BeaconEntity)cubeBlock;
			            //beacon.BroadcastRadius = 1;
			            poweredOffCount++;
			        }
			        if (commandParts[1].ToLower().Equals("tools") && (cubeBlock is MyShipToolBase || cubeBlock is MyShipDrill))
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            poweredOffCount++;
			        }
			        if (commandParts[1].ToLower().Equals("turrets") && (cubeBlock is MyLargeTurretBase))
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            poweredOffCount++;
			        }
			        if (commandParts[1].ToLower().Equals("projectors") && (cubeBlock is MyProjectorBase))
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            poweredOffCount++;
                    }
                    if (commandParts[1].ToLower().Equals("idlerotation") && (cubeBlock is MyLargeTurretBase))
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => ((MyLargeTurretBase)cubeBlock).ChangeIdleRotation(false), null, null);
                        poweredOffCount++;
                    }

                    if (commandParts[1].ToLower().Equals(cubeBlock.BlockDefinition.Id.SubtypeName.ToLower()))
			        {
			            SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = false, null, null);
			            poweredOffCount++;
			        }
			    }
			}

			SendPrivateChatMessage( remoteUserId, "Turned off " + poweredOffCount + " blocks" );
		}

		protected void Command_On( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

			if ( paramCount != 1 )
				return;

            MyEntity[] entities = new MyEntity[0];
            SandboxGameAssemblyWrapper.Instance.GameAction(() => entities = MyEntities.GetEntities().ToArray());
            int poweredOffCount = 0;
            foreach (var entity in entities)
            {
                var grid = entity as MyCubeGrid;
                if (grid?.Physics == null || grid.MarkedForClose)
                    continue;
                foreach (var cubeBlock in grid.GetFatBlocks().ToArray())
                {
                    if (!(cubeBlock is MyFunctionalBlock))
                        continue;

                    var functionalBlock = (MyFunctionalBlock)cubeBlock;

                    if (commandParts[1].ToLower().Equals("all"))
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        poweredOffCount++;
                    }
                    if (commandParts[1].ToLower().Equals("production") && cubeBlock is MyProductionBlock)
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        poweredOffCount++;
                    }
                    if (commandParts[1].ToLower().Equals("beacon") && cubeBlock is MyBeacon)
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        //BeaconEntity beacon = (BeaconEntity)cubeBlock;
                        //beacon.BroadcastRadius = 1;
                        poweredOffCount++;
                    }
                    if (commandParts[1].ToLower().Equals("tools") && (cubeBlock is MyShipToolBase || cubeBlock is MyShipDrill))
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        poweredOffCount++;
                    }
                    if (commandParts[1].ToLower().Equals("turrets") && (cubeBlock is MyLargeTurretBase))
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        poweredOffCount++;
                    }
                    if (commandParts[1].ToLower().Equals("projectors") && (cubeBlock is MyProjectorBase))
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        poweredOffCount++;
                    }

                    if (commandParts[1].ToLower().Equals(cubeBlock.BlockDefinition.Id.SubtypeName.ToLower()))
                    {
                        SandboxGameAssemblyWrapper.Instance.BeginGameAction(() => functionalBlock.Enabled = true, null, null);
                        poweredOffCount++;
                    }
                }
            }

            SendPrivateChatMessage( remoteUserId, "Turned on " + poweredOffCount + " blocks" );
		}

		protected void Command_Kick( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

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

					string playersString = playerItems.Aggregate( string.Empty, ( current, playeritem ) => string.Format( "{0}{1} ", current, playeritem.Name ) );

					SendPrivateChatMessage( remoteUserId, playersString );
					return;
				}

				steamId = playerItems[ 0 ].SteamId;
				if ( steamId == 0 )
					return;
			}

			if ( steamId.ToString( ).StartsWith( "9009" ) )
			{
				SendPrivateChatMessage( remoteUserId, string.Format( "Unable to kick player '{0}'.  This player is the server.", steamId ) );
				return;
			}

			PlayerManager.Instance.KickPlayer( steamId );
			SendPrivateChatMessage( remoteUserId, string.Format( "Kicked player '{0}' off of the server", ( playerItems.Count == 0 ? rawSteamId : playerItems[ 0 ].Name ) ) );
		}

		protected void Command_Ban( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

			if ( paramCount != 1 )
				return;

			//Get the steam id of the player
			string playerName = commandParts[ 1 ];

			if ( playerName.Length < 3 )
			{
				SendPrivateChatMessage( remoteUserId, "3 or more characters required to ban." );
				return;
			}

			ulong steamId;

			List<PlayerMap.InternalPlayerItem> playerItems = PlayerManager.Instance.PlayerMap.GetPlayerItemsFromPlayerName( playerName );

			if ( playerItems.Count == 0 )
			{
				steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName( playerName );
				if ( steamId == 0 )
					return;
			}
			else
			{
				if ( playerItems.Count > 1 )
				{
					SendPrivateChatMessage( remoteUserId, "There is more than one player with the specified name;" );

					string playersString = playerItems.Aggregate( string.Empty, ( current, playeritem ) => string.Format( "{0}{1} ", current, playeritem.Name ) );

					SendPrivateChatMessage( remoteUserId, playersString );
					return;
				}

				steamId = playerItems[ 0 ].SteamId;
				if ( steamId == 0 )
					return;
			}

			if ( steamId.ToString( ).StartsWith( "9009" ) )
			{
				SendPrivateChatMessage( remoteUserId, string.Format( "Unable to ban player '{0}'.  This player is the server.", steamId ) );
				return;
			}

			PlayerManager.Instance.BanPlayer( steamId );

			SendPrivateChatMessage( remoteUserId, string.Format( "Banned '{0}' and kicked them off of the server", ( playerItems.Count == 0 ? playerName : playerItems[ 0 ].Name ) ) );
		}

		protected void Command_Unban( ChatEvent chatEvent )
		{
			ulong remoteUserId = chatEvent.RemoteUserId;
			List<string> commandParts = CommandParser.GetCommandParts( chatEvent.Message );
			int paramCount = commandParts.Count - 1;

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

					string playersString = playerItems.Aggregate( string.Empty, ( current, playeritem ) => string.Format( "{0}{1} ", current, playeritem.Name ) );

					SendPrivateChatMessage( remoteUserId, playersString );
					return;
				}

				steamId = playerItems[ 0 ].SteamId;
				if ( steamId == 0 )
					return;
			}

			PlayerManager.Instance.UnBanPlayer( steamId );

			SendPrivateChatMessage( remoteUserId, string.Format( "Unbanned '{0}'", ( playerItems.Count == 0 ? rawSteamId : playerItems[ 0 ].Name ) ) );
		}

		#endregion

		#endregion

		protected virtual void OnChatMessage( ulong steamid, string playername, string message )
		{
			if ( ChatMessage != null )
			{
				ChatMessage( steamid, playername, message );
			}
		}
	}
}