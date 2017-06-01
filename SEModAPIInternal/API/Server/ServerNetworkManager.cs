using System.Text;
using System.Windows.Forms;
using ParallelTasks;
using Sandbox.Game.World;
using SEModAPI.API;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Library.Collections;
using VRage.Network;

namespace SEModAPIInternal.API.Server
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.ExceptionServices;
	using System.Security;
	using System.Threading;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Common.ObjectBuilders.Definitions;
	using Sandbox.Engine.Multiplayer;
	using Sandbox.Game.Multiplayer;
	using Sandbox.ModAPI;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;
	using SteamSDK;
	using VRage;
	using VRage.ObjectBuilders;
	using VRageMath;

	public class ServerNetworkManager : NetworkManager
	{
		#region "Attributes"
		new protected static ServerNetworkManager m_instance;

		private static MulticastDelegate m_onWorldRequest;
		private static Type m_onWorldRequestType;
		private static bool replaceData = false;
		private static List<ulong> m_responded = new List<ulong>( );
		private static List<ulong> m_clearResponse = new List<ulong>( );
		private static List<ulong> m_inGame = new List<ulong>( );
		private static Dictionary<ulong, Tuple<DateTime, int>> m_slowDown = new Dictionary<ulong, Tuple<DateTime, int>>( );
        private MyTypeTable m_typeTable = new MyTypeTable();
        private HashSet<NetworkHandlerBase> _networkHandlers = new HashSet<NetworkHandlerBase>();

        private const string ServerNetworkManagerNamespace = "Sandbox.Engine.Multiplayer";
		private const string ServerNetworkManagerClass = "MyDedicatedServer";

		private const string ServerNetworkManagerConnectedPlayersField = "m_members";
        
		private const string NetworkingNamespace = "Sandbox.Engine.Networking";

		private const string MyMultipartMessageClass = "MyMultipartMessage";
		private const string MyMultipartMessagePreemble = "SendPreemble";
        
		private const string MySyncLayerField = "SyncLayer";

		private const string MyTransportLayerField = "TransportLayer";
		private const string MyTransportLayerClearMethod = "SendFlush";

		private const string MyMultipartSenderClass = "MyMultipartSender";
		private const string MyMultipartSenderSendPart = "SendPart";

	    private const string TypeTableField = "m_typeTable";
	    private const string TransportHandlersField = "m_handlers";
        
        #endregion

        #region "Properties"

        public static bool WorldVoxelModify { get; set; }

		new public static ServerNetworkManager Instance
		{
			get
			{
				if ( m_instance == null )
					m_instance = new ServerNetworkManager( );

				return m_instance;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ServerNetworkManagerNamespace, ServerNetworkManagerClass );
				if ( type1 == null )
					throw new TypeLoadException( "Could not find internal type for ServerNetworkManager" );
				
                Type myMultiplayerBaseType = typeof ( MyMultiplayerBase );
				if(!Reflection.HasField( myMultiplayerBaseType, MySyncLayerField))
                    throw new TypeLoadException("Could not find internal type for SyncLayer");
                
                if(!Reflection.HasField( typeof(MyDedicatedServerBase), ServerNetworkManagerConnectedPlayersField ))
                    throw new TypeLoadException( "Could not find ConnectedPlayers field");
                
                var syncLayerType = typeof(MySyncLayer);
			    var transportLayerField = syncLayerType.GetField(MyTransportLayerField, BindingFlags.NonPublic | BindingFlags.Instance);
			    
                if(transportLayerField==null)
                    throw new TypeLoadException("Could not find internal type for TransportLayer");

                Type transportLayerType = transportLayerField.FieldType;

			    Type replicationLayerType = typeof(MyReplicationLayerBase);
                if(!Reflection.HasField( replicationLayerType, TypeTableField ))
                    throw new TypeLoadException("Could not find TypeTable field");
                
                if (!Reflection.HasField(transportLayerType, TransportHandlersField))
                    throw new TypeLoadException("Could not find Handlers field");

			    return true;
			}
			catch ( TypeLoadException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

	    public void InitNetworkIntercept()
        {
            m_typeTable = typeof(MyReplicationLayerBase).GetField(TypeTableField, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(MyMultiplayer.ReplicationLayer) as MyTypeTable;
            //don't bother with nullchecks here, it was all handled in ReflectionUnitTest
            var transportType = typeof(MySyncLayer).GetField(MyTransportLayerField, BindingFlags.NonPublic | BindingFlags.Instance).FieldType;
            var transportInstance = typeof(MySyncLayer).GetField(MyTransportLayerField, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(MyMultiplayer.Static.SyncLayer);
            var handlers = (Dictionary<MyMessageId, Action<MyPacket>>)transportType.GetField(TransportHandlersField, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(transportInstance);
           
            //remove Keen's network listener
            handlers.Remove(MyMessageId.RPC);
            //replace it with our own
            handlers.Add(MyMessageId.RPC, ProcessEvent);

            //PrintDebug();

            ApplicationLog.Info("Initialized network intercept!");
        }

        /// <summary>
        /// Dumps *ALL* registered network events to the log
        /// </summary>
        private void PrintDebug()
        {
            var sb = new StringBuilder();
            var methodLookup = (Dictionary<MethodInfo, CallSite>)typeof(MyEventTable).GetField("m_methodInfoLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_typeTable.StaticEventTable);
            sb.Append("###Static events:");
            foreach (var entry in methodLookup)
            {
                sb.Append(entry.Key.DeclaringType?.FullName + "." + entry.Key.Name + "(");
                var parameters = entry.Key.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    sb.Append(parameters[i].ParameterType);
                    sb.Append( " " + parameters[i].Name );
                    if (i < parameters.Length - 1)
                        sb.Append(", ");
                }
                sb.AppendLine(")");
            }

            var typelookup = (Dictionary<Type, MySynchronizedTypeInfo>)typeof(MyTypeTable).GetField("m_typeLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_typeTable);
            foreach (var entry in typelookup)
            {
                var lookup = (Dictionary<MethodInfo, CallSite>)typeof(MyEventTable).GetField("m_methodInfoLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entry.Value.EventTable);
                if (!lookup.Any())
                    continue;

                sb.AppendLine();
                sb.AppendLine();
                sb.Append($"###{entry.Key} events:");

                foreach (var ent in lookup)
                {
                    sb.AppendLine();
                    sb.Append(ent.Key.DeclaringType?.FullName + "." + ent.Key.Name + "(");
                    var parameters = ent.Key.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        sb.Append(parameters[i].ParameterType);
                        sb.Append(" " + parameters[i].Name);
                        if (i < parameters.Length - 1)
                            sb.Append(", ");
                    }
                    sb.Append(")");
                }
            }

            ApplicationLog.Info(sb.ToString());
        }

	    private void ProcessEvent(MyPacket packet)
	    {
	        if (_networkHandlers.Count == 0)
	        {
	            //pass the message back to the game server
	            try
	            {
	                ((MyReplicationLayer)MyMultiplayer.ReplicationLayer).OnEvent(packet);
	            }
	            catch (Exception ex)
	            {
	                ApplicationLog.Error(ex, "~Error processing event!");
	                //crash after logging, bad things could happen if we continue on with bad data
	                throw;
	            }
	            return;
	        }

	        //magic: DO NOT TOUCH
	        BitStream stream = new BitStream();
	        stream.ResetRead(packet);

	        NetworkId networkId = stream.ReadNetworkId();
	        //this value is unused, but removing this line corrupts the rest of the stream
	        NetworkId blockedNetworkId = stream.ReadNetworkId();
	        uint eventId = (uint)stream.ReadUInt16();


	        CallSite site;
	        IMyNetObject sendAs;
	        object obj;
	        if (networkId.IsInvalid) // Static event
	        {
	            site = m_typeTable.StaticEventTable.Get(eventId);
	            sendAs = null;
	            obj = null;
	        }
	        else // Instance event
	        {
	            sendAs = ((MyReplicationLayer)MyMultiplayer.ReplicationLayer).GetObjectByNetworkId(networkId);
	            if (sendAs == null)
	            {
	                return;
	            }
	            var typeInfo = m_typeTable.Get(sendAs.GetType());
	            int eventCount = typeInfo.EventTable.Count;
	            if (eventId < eventCount) // Directly
	            {
	                obj = sendAs;
	                site = typeInfo.EventTable.Get(eventId);
	            }
	            else // Through proxy
	            {
	                obj = ((IMyProxyTarget)sendAs).Target;
	                typeInfo = m_typeTable.Get(obj.GetType());
	                site = typeInfo.EventTable.Get(eventId - (uint)eventCount); // Subtract max id of Proxy
	            }
	        }

#if DEBUG
            if ( ExtenderOptions.IsDebugging )
            {
                if ( !site.MethodInfo.Name.Contains( "SyncPropertyChanged" ) )// && !site.MethodInfo.Name.Contains( "OnSimulationInfo" ) )
                    ApplicationLog.Error( $"Caught event {site.MethodInfo.Name} from user {PlayerMap.Instance.GetFastPlayerNameFromSteamId( packet.Sender.Id.Value )}:{packet.Sender.Id.Value}. Length {stream.ByteLength}B" );
            }
#endif
	        //we're handling the network live in the game thread, this needs to go as fast as possible
	        bool handled = false;
	        Parallel.ForEach(_networkHandlers, handler =>
	                                           {
	                                               try
	                                               {
	                                                   if (handler.CanHandle(site))
	                                                       handled |= handler.Handle(packet.Sender.Id.Value, site, stream, obj);
	                                               }
	                                               catch (Exception ex)
	                                               {
	                                                   ApplicationLog.Error(ex.ToString());
	                                               }
	                                           });

	        //one of the handlers wants us to discard this packet
	        if (handled)
	            return;

	        //pass the message back to the game server
	        try
	        {
	            ((MyReplicationLayer)MyMultiplayer.ReplicationLayer).OnEvent(packet);
	        }
	        catch (Exception ex)
	        {
	            ApplicationLog.Error(ex, "Error when returning control to game server!");
	            //crash after logging, bad things could happen if we continue on with bad data
	            throw;
	        }
	    }

	    private void RegisterNetworkHandler( NetworkHandlerBase handler )
	    {
	        string handlerType = handler.GetType().FullName;
            List<NetworkHandlerBase> toRemove = new List<NetworkHandlerBase>();
	        foreach ( var item in _networkHandlers )
	        {
	            if ( item.GetType().FullName == handlerType )
	            {
	                if (ExtenderOptions.IsDebugging)
	                    ApplicationLog.BaseLog.Error("Network handler already registered! " + handlerType);
	                toRemove.Add(item);
	            }
	        }

	        foreach (var oldHandler in toRemove)
	            _networkHandlers.Remove(oldHandler);

	        _networkHandlers.Add( handler );
	    }

        [Obsolete("Use the params argument")]
	    public void RegisterNetworkHandlers( IEnumerable<NetworkHandlerBase> handlers )
	    {
	        foreach(var handler in handlers)
                RegisterNetworkHandler( handler );
	    }

        public void RegisterNetworkHandlers(params NetworkHandlerBase[] handlers)
        {
            foreach (var handler in handlers)
                RegisterNetworkHandler(handler);
        }

        /// <summary>
        /// Broadcasts an event to all connected clients
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj"></param>
        /// <param name="args"></param>
	    public void RaiseEvent(MethodInfo method, object obj, params object[] args)
        {
            //default(EndpointId) tells the network to broadcast the message
            RaiseEvent(method, obj, default(EndpointId), args);
	    }

        /// <summary>
        /// Sends an event to one client by SteamId
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj"></param>
        /// <param name="steamId"></param>
        /// <param name="args"></param>
	    public void RaiseEvent(MethodInfo method, object obj, ulong steamId, params object[] args)
	    {
	        RaiseEvent(method,obj,new EndpointId(steamId), args);
	    }

        /// <summary>
        /// Sends an event to one client
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj"></param>
        /// <param name="endpoint"></param>
        /// <param name="args"></param>
        public void RaiseEvent(MethodInfo method, object obj, EndpointId endpoint, params object[] args)
        {
            try
            {
                if (method == null)
                    throw new ArgumentNullException(nameof(method), "MethodInfo cannot be null!");

                if (args.Length > 6)
                    throw new ArgumentOutOfRangeException(nameof(args), "Cannot pass more than 6 arguments!");

                var owner = obj as IMyEventOwner;
                if (owner == null)
                    throw new InvalidCastException("Provided event target is not of type IMyEventOwner!");

                if (!method.HasAttribute<EventAttribute>())
                    throw new CustomAttributeFormatException("Provided event target does not have the Event attribute! Replication will not succeed!");

                //array to hold arguments to pass into DispatchEvent
                object[] arguments = new object[11];

                arguments[0] = TryGetCallSite(method, obj);
                arguments[1] = endpoint;
                arguments[2] = 1f;
                arguments[3] = owner;

                //copy supplied arguments into the reflection arguments
                for (int i = 0; i < args.Length; i++)
                    arguments[i + 4] = args[i];

                //pad the array out with DBNull
                for (int j = args.Length + 4; j < 10; j++)
                    arguments[j] = e;

                arguments[10] = (IMyEventOwner)null;

                //create an array of Types so we can create a generic method
                Type[] argTypes = new Type[8];

                for (int k = 3; k < 11; k++)
                    argTypes[k - 3] = arguments[k]?.GetType() ?? typeof(IMyEventOwner);

                //create a generic method of DispatchEvent and invoke to inject our data into the network
                var dispatch = typeof(MyReplicationLayerBase).GetMethod("DispatchEvent", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(argTypes);
                MySandboxGame.Static.Invoke(() =>
                                            {
                                                try
                                                {
                                                    dispatch.Invoke(MyMultiplayer.ReplicationLayer, arguments);
                                                }
                                                catch (Exception ex)
                                                {
                                                    ApplicationLog.BaseLog.Error(ex);
                                                }
                                            });
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
            }
        }

        private static DBNull e = DBNull.Value;

        /// <summary>
        /// Broadcasts a static event to all connected clients
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
	    public void RaiseStaticEvent(MethodInfo method, params object[] args)
	    {
            //default(EndpointId) tells the network to broadcast the message
            RaiseStaticEvent(method, default(EndpointId), args);
	    }

        /// <summary>
        /// Sends a static event to one client by SteamId
        /// </summary>
        /// <param name="method"></param>
        /// <param name="steamId"></param>
        /// <param name="args"></param>
	    public void RaiseStaticEvent(MethodInfo method, ulong steamId, params object[] args)
	    {
	        RaiseStaticEvent(method, new EndpointId(steamId), args);
	    }

        /// <summary>
        /// Sends a static event to one client
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="args"></param>
        public void RaiseStaticEvent(MethodInfo method, EndpointId endpoint, params object[] args)
        {
            try
            {
                if (method == null)
                    throw new ArgumentNullException(nameof(method), "MethodInfo cannot be null!");

                if (args.Length > 6)
                    throw new ArgumentOutOfRangeException(nameof(args), "Cannot pass more than 6 arguments!");

                if (!method.HasAttribute<EventAttribute>())
                    throw new CustomAttributeFormatException("Provided event target does not have the Event attribute! Replication will not succeed!");

                //array to hold arguments to pass into DispatchEvent
                object[] arguments = new object[11];

                arguments[0] = TryGetStaticCallSite(method);
                arguments[1] = endpoint;
                arguments[2] = 1f;
                arguments[3] = (IMyEventOwner)null;

                //copy supplied arguments into the reflection arguments
                for (int i = 0; i < args.Length; i++)
                    arguments[i + 4] = args[i];

                //pad the array out with DBNull
                for (int j = args.Length + 4; j < 10; j++)
                    arguments[j] = e;

                arguments[10] = (IMyEventOwner)null;

                //create an array of Types so we can create a generic method
                Type[] argTypes = new Type[8];

                for (int k = 3; k < 11; k++)
                    argTypes[k - 3] = arguments[k]?.GetType() ?? typeof(IMyEventOwner);

                //create a generic method of DispatchEvent and invoke to inject our data into the network
                var dispatch = typeof(MyReplicationLayerBase).GetMethod("DispatchEvent", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(argTypes);
                MySandboxGame.Static.Invoke(() =>
                                            {
                                                try
                                                {
                                                    dispatch.Invoke(MyMultiplayer.ReplicationLayer, arguments);
                                                }
                                                catch (Exception ex)
                                                {
                                                    ApplicationLog.BaseLog.Error(ex);
                                                }
                                            });
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
            }
        }

        private CallSite TryGetStaticCallSite(MethodInfo method)
	    {
            var methodLookup = (Dictionary<MethodInfo, CallSite>)typeof(MyEventTable).GetField("m_methodInfoLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_typeTable.StaticEventTable);
	        CallSite result;
            if(!methodLookup.TryGetValue(method, out result))
                throw new MissingMemberException("Provided event target not found!");
            return result;
	    }

	    private CallSite TryGetCallSite(MethodInfo method, object arg)
	    {
	        var typeInfo = m_typeTable.Get(arg.GetType());
            var methodLookup = (Dictionary<MethodInfo, CallSite>)typeof(MyEventTable).GetField("m_methodInfoLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(typeInfo.EventTable);
            CallSite result;
            if (!methodLookup.TryGetValue(method, out result))
                throw new MissingMemberException("Provided event target not found!");
            return result;
        }

	    public void SendModMessageTo( ushort id, byte[] message, ulong recipient, bool reliable = true )
	    {
	        if ( reliable )
	        {
	            var method = typeof(MyModAPIHelper.MyMultiplayer).GetMethod( "ModMessageClientReliable", BindingFlags.NonPublic | BindingFlags.Static );
	            RaiseStaticEvent( method, recipient, id, message, recipient );
	        }
            else
            {
                var method = typeof(MyModAPIHelper.MyMultiplayer).GetMethod("ModMessageClientUneliable", BindingFlags.NonPublic | BindingFlags.Static);
                RaiseStaticEvent(method, recipient, id, message, recipient);
            }
        }

        public void BroadcastModMessage(ushort id, byte[] message, bool reliable = true)
        {
            if (reliable)
            {
                var method = typeof(MyModAPIHelper.MyMultiplayer).GetMethod("ModMessageBroadcastReliable", BindingFlags.NonPublic | BindingFlags.Static);
                RaiseStaticEvent(method, id, message);
            }
            else
            {
                var method = typeof(MyModAPIHelper.MyMultiplayer).GetMethod("ModMessageBroadcastUneliable", BindingFlags.NonPublic | BindingFlags.Static);
                RaiseStaticEvent(method, id, message);
            }
        }

        public override List<ulong> GetConnectedPlayers( )
		{
			try
			{
                if(MyMultiplayer.Static == null)
                    return new List<ulong>();
                return (List<ulong>)typeof(MyDedicatedServerBase).GetField("m_members", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(MyMultiplayer.Static) ?? new List<ulong>();
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new List<ulong>( );
			}
		}
        
		//.5B9DDD8F4DF9A88D297B3B0B3B79FBAA
		public void ReplaceWorldJoin( )
		{
			try
			{
				var netManager = GetNetworkManager( );
				var controlHandlerField = BaseObject.GetEntityFieldValue( netManager, NetworkManagerControlHandlerField );
				Dictionary<int, object> controlHandlers = UtilityFunctions.ConvertDictionary<int>( controlHandlerField );
				MethodInfo removeMethod = controlHandlerField.GetType( ).GetMethod( "Remove" );
				removeMethod.Invoke( controlHandlerField, new object[ ] { 0 } );

				ThreadPool.QueueUserWorkItem( ( state ) =>
				{
					OnWorldRequestReplace( state );
				} );

				// Garbage is below as I tried to create a generic delegate and put it into the command handling dictionary.  It's not
				// as straightforward as it seems.  I can get the type, object, generic types, but I can't seem to create a delegate
				// that matches the what they have in game without it throwing an error

				/*
				var worldJoinField = controlHandlers[0];
				//=brsw3F6mys5AacUJHYIS14TVH2=

				FieldInfo worldJoinDelegateField = worldJoinField.GetType().GetField(NetworkingOnWorldRequestField);
				MulticastDelegate action = (MulticastDelegate)worldJoinDelegateField.GetValue(worldJoinField);
				Type worldJoinDelegateType = worldJoinDelegateField.FieldType;
				Type worldJoinDelegateGenericType = worldJoinDelegateType.GetGenericTypeDefinition();
				MethodInfo method = typeof(NetworkManager).GetMethod("WorldTest");
				method = method.MakeGenericMethod(worldJoinDelegateGenericType);
				Delegate.CreateDelegate(worldJoinDelegateType, method);
				*/
				//worldJoinDelegateField.SetValue(worldJoinField, Delegate.CreateDelegate(worldJoinDelegateType, typeof(NetworkManager).GetMethod("WorldTest")));
				//object newWorlDJoinDelegate = Activator.CreateInstance(worldJoinDelegateType, new object[] {

				//MulticastDelegate action = (MulticastDelegate)worldJoinDelegateField.GetValue(worldJoinField);
				//action.Method = typeof(NetworkManager).GetMethod("WorldTest", BindingFlags.Public);
				//m_onWorldRequest = action;
				//m_onWorldRequestType = worldJoinDelegateField.FieldType.GetGenericArguments()[0];
				//action.Method = 
				// this.RegisterControlMessage<MyControlWorldRequestMsg>(MyControlMessageEnum.WorldRequest, new ControlMessageHandler<MyControlWorldRequestMsg>(this.OnWorldRequest));

				/*
				Type controlMessageHandlerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, MyControlMessageHandlerClass);

				var worldJoinField = controlHandlers[0];
				FieldInfo worldJoinDelegateField = worldJoinField.GetType().GetField(NetworkingOnWorldRequestField);
				Type worldJoinDelegateType = worldJoinDelegateField.FieldType;
				Type worldJoinDelegateGenericType = worldJoinDelegateType.GetGenericTypeDefinition();

				var instance = Activator.CreateInstance(controlMessageHandlerType, new object[] { Delegate.CreateDelegate(typeof(object), typeof(NetworkManager).GetMethod("WorldTest")) as  worldJoinDelegateType });
				*/



				//object controlMessageHandler = 

			}
			catch ( Exception ex )
			{

			}
		}

		public void ReplaceWorldData( )
		{
			replaceData = true;
		}

		/// <summary>
		/// Ok so we're going to poll.  The dictionary that contains the world request event is just too hard to untangle, so I just removed it and am
		/// going to poll for new users.  This is painfully bad, but it works, so whatever.
		/// 
		/// The real way to do this is to store the old OnWorldRequest event, replace that event with my own.  I will look at it again another time.
		/// </summary>
		/// <param name="state"></param>
		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private void OnWorldRequestReplace( object state )
		{
			while (true)
			{
				try
				{
					DateTime start = DateTime.Now;
					List<ulong> connectedList = PlayerManager.Instance.ConnectedPlayers;
					for (int r = m_clearResponse.Count - 1; r >= 0; r--)
					{
						ulong player = m_clearResponse[r];

						if (!connectedList.Contains(player))
						{
							ApplicationLog.BaseLog.Info("Removing User - Clear Response");
							m_clearResponse.Remove(player);
							continue;
						}
						m_clearResponse.Remove(player);

						lock (m_inGame)
							m_inGame.Add(player);

						ThreadPool.QueueUserWorkItem((ns) =>
						{
							try
							{
								bool shouldSlowDown = false;
								if (m_slowDown.ContainsKey(player))
								{
									shouldSlowDown = (DateTime.Now - m_slowDown[player].Item1).TotalSeconds < 240;
								}

								ApplicationLog.BaseLog.Info("Sending world data.  Throttle: {0}", shouldSlowDown);
								SendWorldData(player);

								lock (m_slowDown)
								{
									if (!m_slowDown.ContainsKey(player))
										m_slowDown.Add(player, new Tuple<DateTime, int>(DateTime.Now, 1));
									else
									{
										int count = m_slowDown[player].Item2;
										m_slowDown[player] = new Tuple<DateTime, int>(DateTime.Now, count + 1);
									}
								}
							}
							catch
							{
								ApplicationLog.BaseLog.Error("Error sending world data to user.  User must retry");
							}
						});
					}

					foreach (ulong player in connectedList)
					{
						if (player.ToString().StartsWith("9009"))
							continue;

						if (!m_responded.Contains(player) && !m_clearResponse.Contains(player) && !m_inGame.Contains(player))
						{
							m_clearResponse.Add(player);
						}
					}

					lock (m_inGame)
					{
						for (int r = m_inGame.Count - 1; r >= 0; r--)
						{
							ulong player = m_inGame[r];

							if (!connectedList.Contains(player))
							{
								ApplicationLog.BaseLog.Info("Removing user - Ingame / Downloading");
								m_inGame.Remove(player);
								continue;
							}
						}
					}

					Thread.Sleep(200);
				}
				catch (Exception ex)
				{
					ApplicationLog.BaseLog.Error(string.Format("World Request Response Error: {0}", ex));
				}
			}
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private static void SendWorldData( ulong steamId )
		{
			try
			{
				MemoryStream ms = new MemoryStream( );
				if ( MyAPIGateway.Session != null )
				{
					DateTime start = DateTime.Now;
					ApplicationLog.BaseLog.Info( "...responding to user: {0}", steamId );
					SendPreamble( steamId, 1 );
					SendFlush( steamId );

					// Let's sleep for 5 seconds and let plugins know we're online -- let's not after all, causing sync issues
					//Thread.Sleep(5000);
					MyObjectBuilder_World myObjectBuilderWorld = null;
					lock ( m_inGame )
					{
						if ( !m_inGame.Contains( steamId ) )
						{
							ApplicationLog.BaseLog.Info( "Cancelled send to user: {0}", steamId );
							return;
						}
					}

					// This is probably safe to do outside of the game instance, but let's just make sure.
					SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
					{
						myObjectBuilderWorld = MyAPIGateway.Session.GetWorld( );
					} );

					if ( replaceData )
					{
						for ( int r = myObjectBuilderWorld.Sector.SectorObjects.Count - 1; r >= 0; r-- )
						{
							MyObjectBuilder_EntityBase entity = (MyObjectBuilder_EntityBase)myObjectBuilderWorld.Sector.SectorObjects[ r ];

							if ( !( entity is MyObjectBuilder_CubeGrid ) && !( entity is MyObjectBuilder_VoxelMap ) && !( entity is MyObjectBuilder_Character ) )
								continue;

							if ( ( entity is MyObjectBuilder_CubeGrid ) && ( (MyObjectBuilder_CubeGrid)entity ).DisplayName.Contains( "CommRelay" ) )
								continue;

							/*
							if (!(entity is MyObjectBuilder_CubeGrid))
								continue;

							if ((entity.PersistentFlags & MyPersistentEntityFlags2.InScene) == MyPersistentEntityFlags2.InScene)
								continue;
							*/

							myObjectBuilderWorld.Sector.SectorObjects.RemoveAt( r );
						}

						myObjectBuilderWorld.Sector.Encounters = null;

						myObjectBuilderWorld.VoxelMaps.Dictionary.Clear( );
						myObjectBuilderWorld.Checkpoint.Settings.ProceduralDensity = 0f;
						myObjectBuilderWorld.Checkpoint.Settings.ProceduralSeed = 0;

						// Check if this is OK?
						//myObjectBuilderWorld.Checkpoint.ConnectedPlayers.Dictionary.Clear();
						myObjectBuilderWorld.Checkpoint.DisconnectedPlayers.Dictionary.Clear( );

						long playerId = PlayerMap.Instance.GetFastPlayerIdFromSteamId( steamId );

						MyObjectBuilder_Toolbar blankToolbar = new MyObjectBuilder_Toolbar( );
						foreach ( KeyValuePair<MyObjectBuilder_Checkpoint.PlayerId, MyObjectBuilder_Player> p in myObjectBuilderWorld.Checkpoint.AllPlayersData.Dictionary )
						{
							if ( p.Value.EntityCameraData != null )
								p.Value.EntityCameraData.Clear( );

							if ( p.Value.CameraData != null )
								p.Value.CameraData.Dictionary.Clear( );

							if ( p.Key.ClientId == steamId )
							{
								continue;
							}

							p.Value.Toolbar = null;
							p.Value.CharacterCameraData = null;
						}

						for ( int r = myObjectBuilderWorld.Checkpoint.Gps.Dictionary.Count - 1; r >= 0; r-- )
						{
							KeyValuePair<long, MyObjectBuilder_Gps> p = myObjectBuilderWorld.Checkpoint.Gps.Dictionary.ElementAt( r );

							if ( p.Key == playerId )
								continue;

							myObjectBuilderWorld.Checkpoint.Gps.Dictionary.Remove( p.Key );
						}

						myObjectBuilderWorld.Checkpoint.ChatHistory.RemoveAll( x => x.IdentityId != playerId );

						long factionId = 0;
						if ( myObjectBuilderWorld.Checkpoint.Factions.Players.Dictionary.ContainsKey( playerId ) )
						{
							factionId = myObjectBuilderWorld.Checkpoint.Factions.Players.Dictionary[ playerId ];
							myObjectBuilderWorld.Checkpoint.FactionChatHistory.RemoveAll( x => x.FactionId1 != factionId && x.FactionId2 != factionId );
							myObjectBuilderWorld.Checkpoint.Factions.Requests.RemoveAll( x => x.FactionId != factionId );
						}
						else
						{
							myObjectBuilderWorld.Checkpoint.FactionChatHistory.Clear( );
							myObjectBuilderWorld.Checkpoint.Factions.Requests.Clear( );
						}

						foreach ( MyObjectBuilder_Faction faction in myObjectBuilderWorld.Checkpoint.Factions.Factions )
						{
							if ( faction.FactionId != factionId )
							{
								faction.PrivateInfo = "";
							}
						}
					}

                    // This will modify the world data to remove voxels and turn off procedural for the player.  The
                    // server controls procedural anyway, and doesn't require the user to know about it.  If we don't
                    // turn it off, the objects will still generate on the server.  Possible issue: turning on voxel
                    // management + procedural encounters may be an issue, as I don't think the server sends procedural
                    // encounters to the client.  This may be a gotcha issue when it comes to using this option
                    if (WorldVoxelModify)
                    {
                        for (int r = myObjectBuilderWorld.Sector.SectorObjects.Count - 1; r >= 0; r--)
                        {
                            MyObjectBuilder_EntityBase entity = (MyObjectBuilder_EntityBase)myObjectBuilderWorld.Sector.SectorObjects[r];

                            if (!(entity is MyObjectBuilder_VoxelMap))
                                continue;

                            myObjectBuilderWorld.Sector.SectorObjects.RemoveAt(r);
                        }

                        myObjectBuilderWorld.Sector.Encounters = null;

                        myObjectBuilderWorld.VoxelMaps.Dictionary.Clear();
                        myObjectBuilderWorld.Checkpoint.Settings.ProceduralDensity = 0f;
                        myObjectBuilderWorld.Checkpoint.Settings.ProceduralSeed = 0;
                    }

					MyObjectBuilder_Checkpoint checkpoint = myObjectBuilderWorld.Checkpoint;
					checkpoint.WorkshopId = null;
					checkpoint.CharacterToolbar = null;
					DateTime cs = DateTime.Now;
					MyObjectBuilderSerializer.SerializeXML( ms, myObjectBuilderWorld, MyObjectBuilderSerializer.XmlCompression.Gzip, null );

					/*
					MemoryStream vms = new MemoryStream();
					MyObjectBuilderSerializer.SerializeXML(vms, myObjectBuilderWorld, MyObjectBuilderSerializer.XmlCompression.Uncompressed, null);
					FileStream file = new FileStream("e:\\temp\\test.txt", FileMode.Create);
					vms.WriteTo(file);
					file.Close();
					*/

					ApplicationLog.BaseLog.Info( "...response construction took {0}ms (cp - {1}ms) size - {2} bytes", ( DateTime.Now - start ).TotalMilliseconds, ( DateTime.Now - cs ).TotalMilliseconds, ms.Length );
				}

				TransferWorld( ms, steamId );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex, "SendWorldData Error: {0}", ex );
			}
		}

		private static void TriggerWorldSendEvent( ulong steamId )
		{
			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
			newEvent.type = EntityEventManager.EntityEventType.OnPlayerWorldSent;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = steamId;
			newEvent.priority = 0;
			EntityEventManager.Instance.AddEvent( newEvent );
		}

		private static Type MyMultipartSenderType( )
		{
			Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( NetworkingNamespace, MyMultipartSenderClass );
			return type;
		}

		private static void TransferWorld( MemoryStream ms, ulong steamId )
		{
			try
			{
				// Just send it all to steam and let it handle it.  This can cause issues, so if it fails once, the next time a user connects, lets slow it down.
				DateTime start = DateTime.Now;
				byte[ ] array = ms.ToArray( );
				int size = 13000;
				int count = 0;
				int position = 0;

				lock ( m_slowDown )
				{
					if ( m_slowDown.ContainsKey( steamId ) )
					{
						if ( DateTime.Now - m_slowDown[ steamId ].Item1 > TimeSpan.FromMinutes( 4 ) )
						{
							//size = m_speeds[Math.Min(3, m_slowDown[steamId].Item2)];
							count = 10 * Math.Max( 0, ( m_slowDown[ steamId ].Item2 - 3 ) );
						}
						else
						{
							m_slowDown[ steamId ] = new Tuple<DateTime, int>( DateTime.Now, 0 );
						}
					}
				}

				var myMultipartSender = Activator.CreateInstance( MyMultipartSenderType( ), new object[ ] { array, (int)array.Length, steamId, 1, size } );
				while ( (bool)BaseObject.InvokeEntityMethod( myMultipartSender, MyMultipartSenderSendPart ) )
				{
					Thread.Sleep( 2 + count );

					position++;
					lock ( m_inGame )
					{
						if ( !m_inGame.Contains( steamId ) )
						{
							ApplicationLog.BaseLog.Info( "Interrupted send to user: {0} ({1} - {2})", steamId, size, count );
							break;
						}
					}
				}

				if ( m_inGame.Contains( steamId ) )
					TriggerWorldSendEvent( steamId );

				ApplicationLog.BaseLog.Info( "World Snapshot Send -> {0} ({2} - {3}): {1}ms", steamId, ( DateTime.Now - start ).TotalMilliseconds, size, count );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( string.Format( "TransferWorld Error: {0}", ex ) );
			}
		}

		private static Type MyMultipartMessageType( )
		{
			Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( NetworkingNamespace, MyMultipartMessageClass );
			return type;
		}

		private static void SendPreamble( ulong steamId, int num )
		{
			//.=cDj6WIzYFodmTa89pvnrADNanH=.SendPreemble
			BaseObject.InvokeStaticMethod( MyMultipartMessageType( ), MyMultipartMessagePreemble, new object[ ] { steamId, num } );
		}

		private static void SendFlush( ulong steamId )
		{
			var netManager = GetNetworkManager( );
			var mySyncLayer = BaseObject.GetEntityFieldValue( netManager, MySyncLayerField );
			var myTransportLayer = BaseObject.GetEntityFieldValue( mySyncLayer, MyTransportLayerField );
			BaseObject.InvokeEntityMethod( myTransportLayer, MyTransportLayerClearMethod, new object[ ] { steamId } );
		}

		#endregion
	}
}
