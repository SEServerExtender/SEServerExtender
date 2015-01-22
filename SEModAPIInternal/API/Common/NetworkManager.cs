using SteamSDK;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security;

using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Serializer;

using SEModAPIInternal.Support;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Utility;
using System.Linq.Expressions;

using VRageMath;

using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;

namespace SEModAPIInternal.API.Common
{
	//PacketIdEnum Attribute: C42525D7DE28CE4CFB44651F3D03A50D.4C6398741B0F8804D769E5A2E3999E1D

	public enum PacketIds
	{
		CharacterDamage = 5,							//..D666899DBA69CE94D9A0975FDAB80BEC
		VoxelSomething = 13,							//..8B85E2DD7354BDB570B8F448FF105601
		VoxelSomething2 = 36,							//..630C381F251A7EBADB91135C8E343D17
		EntityBase = 37,								//..4F06732E7F1BD73CCF9AA12C9675A0F6
		EntityBaseSerialized = 38,						//..9163D0037A92C9B6DBF801EF5D53998E
		GravitySettings = 666,							//..4E427F5D20ED55F40EFFE0F6D0E179D8
		FloatingObjectPositionOrientation = 1630,		//..D5E5BAF9064D0C9A26E2BB899ED3BED8
		InventoryTransferItem = 2467,					//..5D0E63127AEA2BE91B98D448983B0647
		InventoryUpdateItemAmount = 2468,				//..8305AA2AB275DF34165B55263A6A7AA5
		EnabledConveyorSystem = 2476,					//..696B1F840A189ED6F234D7875793AF6D
		GyroPower = 7586,								//..BB19174225804BB5035228F5477D82C9
		FloatingObjectAltPositionOrientation = 10150,	//..564E654F19DA5C21E7869B4744304993
		FloatingObjectContents = 10151,					//..0008E59AE36FA0F2E7ED91037507E4E8
		ChatMessage = 13872,							//C42525D7DE28CE4CFB44651F3D03A50D.12AEE9CB08C9FC64151B8A094D6BB668
		TerminalFunctionalBlockEnabled = 15268,			//..7F2B3C2BC4F8C6F50583C135CA112213
		TerminalFunctionalBlockName = 15269,			//..721B404F9CB193B34D5353A019A57DAB
	}

	public enum PacketRegistrationType
	{
		Static,
		Instance,
		Timespan,
	}

	public abstract class NetworkManager
	{
		#region "Attributes"

		protected static NetworkManager m_instance;
		protected static MethodInfo m_registerPacketHandlerMethod;
		protected static MethodInfo m_registerPacketHandlerMethod2;
		protected static MethodInfo m_registerPacketHandlerMethod3;

		protected static MulticastDelegate m_onWorldRequest;
		protected static Type m_onWorldRequestType;

		protected static int[] m_speeds = { 512, 256, 128, 128 };
		protected static bool replaceData = false;

		private static List<ulong> m_responded = new List<ulong>();
		private static List<ulong> m_clearResponse = new List<ulong>();
		private static List<ulong> m_inGame = new List<ulong>();
		private static Dictionary<ulong, Tuple<DateTime, int>> m_slowDown = new Dictionary<ulong, Tuple<DateTime, int>>();

		//This class is just a container for some basic steam game values as well as the actual network manager instance
		public static string NetworkManagerWrapperNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string NetworkManagerWrapperClass = "8920513CC2D9F0BEBCDC74DBD637049F";
		public static string NetworkManagerWrapperManagerInstanceField = "8E8199A1194065205F01051DC8B72DE7";
		public static string NetworkManagerControlType = "5B9DDD8F4DF9A88D297B3B0B3B79FBAA";

		//This is an abstract class that the actual network managers implement
		public static string NetworkManagerNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string NetworkManagerClass = "9CDBE03D49929CA686F49B66EE307DD7";
		public static string NetworkManagerSendStructMethod = "6D24456D3649B6393BA2AF59E656E4BF";
		public static string NetworkManagerRegisterChatReceiverMethod = "8A73057A206BFCA00EC372183441891A";
		public static string NetworkManagerInternalNetManagerField = "E863C8EAD57B154571B7A487C6A39AC6";
		public static string NetworkManagerControlHandlerField = "958DE615347A2316DDEF38E8149C34EC";

		public static string InternalNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string InternalNetManagerClass = "08FBF1782D25BEBDA2070CAF8CE47D72";
		public static string InternalNetManagerPacketRegistryField = "6F79877D9F8B092082EAEF8828D69F98";
		public static string InternalNetManagerSendToAllExceptMethod = "5ED378823191AF1EBAAF484B160C4CBC";
		public static string InternalNetManagerSendToAllMethod = "88BEA4C178343A1B40A23DE1A2F8E0FF";

		public static string PacketRegistryNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string PacketRegistryClass = "4D0D6F8422AC35DCF2A403F1C4B70957";
		public static string PacketRegistryTypeIdMapField = "5C5BB4D88AA04A59AB078CB70049BAC8";

		//36CC7CE820B9BBBE4B3FECFEEFE4AE86.7B6560DE2B6A29DE7F0157E9CDFFCC37.7AEDE70A5F16434A660FC187077FC86F

		///
		public static string NetworkingNamespace = "36CC7CE820B9BBBE4B3FECFEEFE4AE86";
		public static string NetworkingOnWorldRequestField = "FAD031AB4FED05B9FE273ACD199496EE";

		public static string MyMultipartMessageClass = "7B6560DE2B6A29DE7F0157E9CDFFCC37";
		public static string MyMultipartMessagePreemble = "7AEDE70A5F16434A660FC187077FC86F";

		public static string MySyncLayerField = "E863C8EAD57B154571B7A487C6A39AC6";
		public static string MyTransportLayerField = "6F79877D9F8B092082EAEF8828D69F98";
		public static string MyTransportLayerClearMethod = "DA0F40A1E0E2E5DD9B141562B91BDDDC";

		public static string MyMultipartSenderClass = "73C7CA87DB0535EFE711E10913D8ACFB";
		public static string MyMultipartSenderSendPart = "A822BAC1F661C682C78230403DDF5670";

		public static string MySyncLayerSendMessage = "358D29D15C14B49FEA47651E0DE22024";

		/////////////////////////////////////////////////

		//1 Packet Type
		public static string GravityGeneratorNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string GravityGeneratorNetManagerClass = "74AB413CFFB499A7945B3E3B84DC56CB";

		//2 Packet Types
		public static string TerminalFunctionalBlocksNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string TerminalFunctionalBlocksNetManagerClass = "850F199A13F4F6D5ED23E89E7F8D99CD";

		//2+ Packet Types
		public static string InventoryNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string InventoryNetManagerClass = "98C1408628C42B9F7FDB1DE7B8FAE776";

		//1 Packet Type
		public static string ConveyorEnabledBlockNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string ConveyorEnabledBlockNetManagerClass = "C866709CB4D18071636E8389BEBA8508";

		//3 Packet Types
		public static string FloatingObjectNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string FloatingObjectNetManagerClass = "E97FDDC1EF9C912AA82D24410983D7E8";

		//2 Packet Types
		public static string VoxelMapNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string VoxelMapNetManagerClass = "EA51F988BB36804CAE6371053AD2602E";

		/////////////////////////////////////////////////////// 

		private static string MultiplayerNamespace = "5F381EA9388E0A32A8C817841E192BE8";

		private static string SendCloseClass = "48D79F8E3C8922F14D85F6D98237314C";
		private static string SendCloseClosedMsg = "4038C6AE0CB130E41455232470357263";
		private static string SendCloseClosedMsgEntityId = "3E16AD760B497CC0921CDE99D46348D9";

		private static string SendCreateClass = "8EFE49A46AB934472427B7D117FD3C64";
		private static string SendCreateRelativeCompressedMsg = "4DFD818DC1531F7E40ED1E5D94A2B650";
		private static string SendCreateCompressedMsg = "9163D0037A92C9B6DBF801EF5D53998E";

		private static string SendCreateRelativeCompressedMsgCreateMessage = "21859045930ACEE4A31D6391A0937D87";
		private static string SendCreateRelativeCompressedMsgBaseEntity = "EE1F27FD35F85E8CD38338A0D8AB4AC8";
		private static string SendCreateRelativeCompressedMsgRelativeVelocity = "A4DD67802385CCB3335B898BA717910B";

		private static string SendCreateCompressedMsgObjectBuilders = "75490843CC702E3F6857E0CF65C5E908";
		private static string SendCreateCompressedMsgBuilderLengths = "DADA09BE16684760302EB5A06A68A7C4";


		#endregion

		#region "Constructors and Initializers"

		static NetworkManager()
		{
			PreparePacketRegistrationMethod();
		}

		protected NetworkManager()
		{
			
			m_instance = this;
			Console.WriteLine("Finished loading NetworkManager");
		}

		#endregion

		#region "Properties"

		public static NetworkManager Instance
		{
			get { return m_instance; }
		}

		public static Type NetworkManagerType
		{
			get
			{
				Type netManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkManagerNamespace, NetworkManagerClass);
				return netManagerType;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = NetworkManagerType;
				if (type == null)
					throw new Exception("Could not find internal type for NetworkManager");

				Type type2 = MyMultipartMessageType();
				if (type2 == null)
					throw new Exception("Could not find interal type for MyMultipartMessage");

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public static Object GetNetworkManager()
		{
			try
			{
				Type networkManagerWrapper = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkManagerWrapperNamespace, NetworkManagerWrapperClass);
				Object networkManager = BaseObject.GetStaticFieldValue(networkManagerWrapper, NetworkManagerWrapperManagerInstanceField);

				return networkManager;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public void SendStruct(ulong remoteUserId, Object data, Type structType)
		{
			try
			{
				MethodInfo sendStructMethod = NetworkManagerType.GetMethod(NetworkManagerSendStructMethod, BindingFlags.NonPublic | BindingFlags.Instance);

				sendStructMethod = sendStructMethod.MakeGenericMethod(structType);

				var netManager = GetNetworkManager();
				sendStructMethod.Invoke(netManager, new object[] { remoteUserId, data });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void RegisterChatReceiver(Action<ulong, string, ChatEntryTypeEnum> action)
		{
			try
			{
				var netManager = GetNetworkManager();
				BaseObject.InvokeEntityMethod(netManager, NetworkManagerRegisterChatReceiverMethod, new object[] { action });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		private Type GetControlType()
		{
			Type controlType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkManagerWrapperNamespace, NetworkManagerControlType);
			return controlType;
		}

		private static void SendMessage(object msg, ulong userId, Type msgType, int flag)
		{
			try
			{
				var netManager = GetNetworkManager();
				var mySyncLayer = BaseObject.GetEntityFieldValue(netManager, MySyncLayerField);
				MethodInfo[] methods = mySyncLayer.GetType().GetMethods();
				MethodInfo sendMessageMethod = methods.FirstOrDefault(x => x.Name == MySyncLayerSendMessage);
				sendMessageMethod = sendMessageMethod.MakeGenericMethod(msgType);
				sendMessageMethod.Invoke(mySyncLayer, new object[] { msg, userId, flag });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public static void SendCloseEntity(ulong userId, long entityId)
		{
			int pos = 0;
			try
			{
				Type sendCloseClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, SendCloseClass);
				Type sendCloseType = sendCloseClassType.GetNestedType(SendCloseClosedMsg, BindingFlags.NonPublic);
				FieldInfo sendCloseEntityIdField = sendCloseType.GetField(SendCloseClosedMsgEntityId);
				Object sendCloseStruct = Activator.CreateInstance(sendCloseType);
				sendCloseEntityIdField.SetValue(sendCloseStruct, entityId);

				NetworkManager.SendMessage(sendCloseStruct, userId, sendCloseType, 2);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole(string.Format("SendCloseEntity({1}): {0}", ex.ToString(), pos));
			}
		}

		public static void SendEntityCreated(MyObjectBuilder_EntityBase entity, ulong userId)
		{
			Type sendCreateClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, SendCreateClass);
			Type sendCreateCompressedMsgType = sendCreateClassType.GetNestedType(SendCreateCompressedMsg, BindingFlags.NonPublic);

			FieldInfo createObjectBuilders = sendCreateCompressedMsgType.GetField(SendCreateCompressedMsgObjectBuilders);
			FieldInfo createBuilderLengths = sendCreateCompressedMsgType.GetField(SendCreateCompressedMsgBuilderLengths);

			MemoryStream memoryStream = new MemoryStream();
			MyObjectBuilderSerializer.SerializeXML(memoryStream, entity, MyObjectBuilderSerializer.XmlCompression.Gzip, typeof(MyObjectBuilder_EntityBase));
			if (memoryStream.Length > (long)2147483647)
			{
				return;
			}

			object createMessage = Activator.CreateInstance(sendCreateCompressedMsgType);

			createObjectBuilders.SetValue(createMessage, memoryStream.ToArray());
			createBuilderLengths.SetValue(createMessage, new int[] { (int)memoryStream.Length });

			SendMessage(createMessage, userId, sendCreateCompressedMsgType, 1);
		}

		public static void SendEntityCreatedRelative(MyObjectBuilder_EntityBase entity, IMyCubeGrid grid, Vector3 relativeVelocity, ulong userId)
		{
			Type sendCreateClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, SendCreateClass);
			Type sendCreateRelativeCompressedMsgType = sendCreateClassType.GetNestedType(SendCreateRelativeCompressedMsg, BindingFlags.NonPublic);
			Type sendCreateCompressedMsgType = sendCreateClassType.GetNestedType(SendCreateCompressedMsg, BindingFlags.NonPublic);

			FieldInfo createMessageField = sendCreateRelativeCompressedMsgType.GetField(SendCreateRelativeCompressedMsgCreateMessage);
			FieldInfo createBaseEntity = sendCreateRelativeCompressedMsgType.GetField(SendCreateRelativeCompressedMsgBaseEntity);
			FieldInfo createRelativeVelocity = sendCreateRelativeCompressedMsgType.GetField(SendCreateRelativeCompressedMsgRelativeVelocity);

			FieldInfo createObjectBuilders = sendCreateCompressedMsgType.GetField(SendCreateCompressedMsgObjectBuilders);
			FieldInfo createBuilderLengths = sendCreateCompressedMsgType.GetField(SendCreateCompressedMsgBuilderLengths);

			MemoryStream memoryStream = new MemoryStream();
			MyPositionAndOrientation value = entity.PositionAndOrientation.Value;
			Matrix matrix = value.GetMatrix() * grid.PositionComp.WorldMatrixNormalizedInv;
			entity.PositionAndOrientation = new MyPositionAndOrientation?(new MyPositionAndOrientation(matrix));
			MyObjectBuilderSerializer.SerializeXML(memoryStream, entity, MyObjectBuilderSerializer.XmlCompression.Gzip, typeof(MyObjectBuilder_EntityBase));
			if (memoryStream.Length > (long)2147483647)
			{
				return;
			}

			// SetValues
			object relativeMessage = Activator.CreateInstance(sendCreateRelativeCompressedMsgType);
			object createMessage = createMessageField.GetValue(relativeMessage);

			createObjectBuilders.SetValue(createMessage, memoryStream.ToArray());
			createBuilderLengths.SetValue(createMessage, new int[] { (int)memoryStream.Length });

			createBaseEntity.SetValue(relativeMessage, entity.EntityId);
			createRelativeVelocity.SetValue(relativeMessage, relativeVelocity);

			SendMessage(relativeMessage, userId, sendCreateCompressedMsgType, 1);
		}

		//C42525D7DE28CE4CFB44651F3D03A50D.5B9DDD8F4DF9A88D297B3B0B3B79FBAA
		public void ReplaceWorldJoin()
		{
			try
			{
				var netManager = GetNetworkManager();
				var controlHandlerField = BaseObject.GetEntityFieldValue(netManager, NetworkManagerControlHandlerField);
				Dictionary<int, object> controlHandlers = UtilityFunctions.ConvertDictionary<int>(controlHandlerField);
				var worldJoinField = controlHandlers[0];
				//FAD031AB4FED05B9FE273ACD199496EE
				FieldInfo worldJoinDelegateField = worldJoinField.GetType().GetField(NetworkingOnWorldRequestField);
				MulticastDelegate action = (MulticastDelegate)worldJoinDelegateField.GetValue(worldJoinField);
				m_onWorldRequest = action;
				m_onWorldRequestType = worldJoinDelegateField.FieldType.GetGenericArguments()[0];
				MethodInfo removeMethod = controlHandlerField.GetType().GetMethod("Remove");
				removeMethod.Invoke(controlHandlerField, new object[] { 0 });
				
				ThreadPool.QueueUserWorkItem((state) =>
				{
					OnWorldRequestReplace(state);
				});
			}
			catch (Exception ex)
			{

			}
		}

		public void ReplaceWorldData()
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
		protected void OnWorldRequestReplace(object state)
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
							LogManager.APILog.WriteLineAndConsole("Removing User - Clear Response");
							m_clearResponse.Remove(player);
							continue;
						}
						m_clearResponse.Remove(player);

						lock(m_inGame)
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

								LogManager.APILog.WriteLineAndConsole(string.Format("Sending world data.  Throttle: {0}", shouldSlowDown));
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
								LogManager.APILog.WriteLineAndConsole("Error sending world data to user.  User must retry");
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
								LogManager.APILog.WriteLineAndConsole("Removing user - Ingame / Downloading");
								m_inGame.Remove(player);
								continue;
							}
						}
					}

					Thread.Sleep(200);
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLineAndConsole(string.Format("World Request Response Error: {0}", ex.ToString()));
				}
			}
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		public static void SendWorldData(ulong steamId)
		{
			try
			{
				MemoryStream ms = new MemoryStream();
				if (MyAPIGateway.Session != null)
				{
					DateTime start = DateTime.Now;
					LogManager.APILog.WriteLineAndConsole(string.Format("...responding to user: {0}", steamId));
					SendPreemble(steamId, 1);
					SendFlush(steamId);

					// Let's sleep for 5 seconds and let plugins know we're online
					//Thread.Sleep(5000);
					MyObjectBuilder_World myObjectBuilderWorld = null;
					lock(m_inGame)
					{
						if (!m_inGame.Contains(steamId))
						{
							LogManager.APILog.WriteLineAndConsole(string.Format("Cancelled send to user: {0}", steamId));
							return;
						}
					}

					// This is probably safe to do outside of the game instance, but let's just make sure.
					SandboxGameAssemblyWrapper.Instance.GameAction(() =>
					{
						myObjectBuilderWorld = MyAPIGateway.Session.GetWorld();
					});

					if (replaceData)
					{
						for (int r = myObjectBuilderWorld.Sector.SectorObjects.Count - 1; r >= 0; r--)
						{
							MyObjectBuilder_EntityBase entity = (MyObjectBuilder_EntityBase)myObjectBuilderWorld.Sector.SectorObjects[r];
							if (!(entity is MyObjectBuilder_CubeGrid))
								continue;

							if ((entity.PersistentFlags & MyPersistentEntityFlags2.InScene) == MyPersistentEntityFlags2.InScene)
								continue;

							myObjectBuilderWorld.Sector.SectorObjects.RemoveAt(r);
						}
					}

					MyObjectBuilder_Checkpoint checkpoint = myObjectBuilderWorld.Checkpoint;
					checkpoint.WorkshopId = null;
					checkpoint.CharacterToolbar = null;
					DateTime cs = DateTime.Now;
					MyObjectBuilderSerializer.SerializeXML(ms, myObjectBuilderWorld, MyObjectBuilderSerializer.XmlCompression.Gzip, null);
					LogManager.APILog.WriteLineAndConsole(string.Format("...response construction took {0}ms (cp - {1}ms)", (DateTime.Now - start).TotalMilliseconds, (DateTime.Now - cs).TotalMilliseconds));

				}

				TransferWorld(ms, steamId);
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole(string.Format("SendWorldData Error: {0}", ex.ToString()));
			}
		}

		private static void TriggerWorldSendEvent(ulong steamId)
		{
			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnPlayerWorldSent;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = steamId;
			newEvent.priority = 0;
			EntityEventManager.Instance.AddEvent(newEvent);	
		}

		private static Type MyMultipartSenderType()
		{
			Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkingNamespace, MyMultipartSenderClass);
			return type;
		}

		private static void TransferWorld(MemoryStream ms, ulong steamId)
		{
			try
			{
				// Just send it all to steam and let it handle it.  This can cause issues, so if it fails once, the next time a user connects, lets slow it down.
				DateTime start = DateTime.Now;
				byte[] array = ms.ToArray();
				int size = 13000;
				int count = 0;
				int position = 0;

				lock (m_slowDown)
				{
					if (m_slowDown.ContainsKey(steamId))
					{
						if (DateTime.Now - m_slowDown[steamId].Item1 > TimeSpan.FromMinutes(4))
						{
							//size = m_speeds[Math.Min(3, m_slowDown[steamId].Item2)];
							count = 10 * Math.Max(0, (m_slowDown[steamId].Item2 - 3));
						}
						else
						{
							m_slowDown[steamId] = new Tuple<DateTime, int>(DateTime.Now, 0);
						}
					}
				}

				var myMultipartSender = Activator.CreateInstance(MyMultipartSenderType(), new object[] { array, (int)array.Length, steamId, 1, size });
				while ((bool)BaseObject.InvokeEntityMethod(myMultipartSender, MyMultipartSenderSendPart))
				{
					Thread.Sleep(2 + count);

					position++;
					lock (m_inGame)
					{
						if (!m_inGame.Contains(steamId))
						{
							LogManager.APILog.WriteLineAndConsole(string.Format("Interrupted send to user: {0} ({1} - {2})", steamId, size, count));
							break;
						}
					}
				}

				if (m_inGame.Contains(steamId))
					TriggerWorldSendEvent(steamId);

				LogManager.APILog.WriteLineAndConsole(string.Format("World Snapshot Send -> {0} ({2} - {3}): {1}ms", steamId, (DateTime.Now - start).TotalMilliseconds, size, count));
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole(string.Format("TransferWorld Error: {0}", ex.ToString()));
			}
		}

		private static Type MyMultipartMessageType()
		{
			Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkingNamespace, MyMultipartMessageClass);
			return type;
		}

		private static void SendPreemble(ulong steamId, int num)
		{
			//36CC7CE820B9BBBE4B3FECFEEFE4AE86.7B6560DE2B6A29DE7F0157E9CDFFCC37.7AEDE70A5F16434A660FC187077FC86F
			BaseObject.InvokeStaticMethod(MyMultipartMessageType(), MyMultipartMessagePreemble, new object[] { steamId, num });
		}		

		private static void SendFlush(ulong steamId)
		{
			var netManager = GetNetworkManager();
			var mySyncLayer = BaseObject.GetEntityFieldValue(netManager, MySyncLayerField);
			var myTransportLayer = BaseObject.GetEntityFieldValue(mySyncLayer, MyTransportLayerField);
			BaseObject.InvokeEntityMethod(myTransportLayer, MyTransportLayerClearMethod, new object[] { steamId });
		}

		public abstract List<ulong> GetConnectedPlayers();

		protected Object GetInternalNetManager()
		{
			try
			{
				Object internalNetManager = BaseObject.GetEntityFieldValue(GetNetworkManager(), NetworkManagerInternalNetManagerField);
				return internalNetManager;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected Object GetPacketRegistry()
		{
			try
			{
				Object internalNetManager = GetInternalNetManager();
				Object packetRegistry = BaseObject.GetEntityFieldValue(internalNetManager, InternalNetManagerPacketRegistryField);

				return packetRegistry;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public Dictionary<Type, ushort> GetRegisteredPacketTypes()
		{
			try
			{
				Object packetRegistry = GetPacketRegistry();
				Dictionary<Type, ushort> packetTypeIdMap = (Dictionary<Type, ushort>)BaseObject.GetEntityFieldValue(packetRegistry, PacketRegistryTypeIdMapField);

				return packetTypeIdMap;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new Dictionary<Type, ushort>();
			}
		}

		protected static Delegate CreatePacketHandlerDelegate(PacketRegistrationType registrationType, Type packetType, MethodInfo handler)
		{
			try
			{
				Type delegateType = null;
				switch (registrationType)
				{
					case PacketRegistrationType.Static:
						delegateType = typeof(NetworkManager.ReceivePacketStatic<>);
						break;
					case PacketRegistrationType.Instance:
						delegateType = typeof(NetworkManager.ReceivePacketInstance<>);
						break;
					case PacketRegistrationType.Timespan:
						delegateType = typeof(NetworkManager.ReceivePacketTimespan<>);
						break;
					default:
						return null;
				}
				delegateType = delegateType.MakeGenericType(packetType);
				handler = handler.MakeGenericMethod(packetType);
				Delegate action = Delegate.CreateDelegate(delegateType, handler);
				return action;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected static void PreparePacketRegistrationMethod()
		{
			try
			{
				Type masterNetManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkManager.InternalNetManagerNamespace, NetworkManager.InternalNetManagerClass);

				MethodInfo[] methods = masterNetManagerType.GetMethods(BindingFlags.Public | BindingFlags.Static);
				foreach (MethodInfo method in methods)
				{
					if (method.Name == "80EECA92933FCDB28F13F8A8A479BFBD")	//Static
					{
						ParameterInfo[] parameters = method.GetParameters();
						if (parameters[0].ParameterType.Name == "BA9ED4CEE897B521F3D57A4EE3B3B8FC")
						{
							m_registerPacketHandlerMethod = method;
						}
					}
					if (method.Name == "43F78311C85C936231EC67195D5D2B73")	//Instance
					{
						ParameterInfo[] parameters = method.GetParameters();
						if (parameters[0].ParameterType.Name == "22B36CE5809693930272FEC3EE3B9BA8")
						{
							m_registerPacketHandlerMethod2 = method;
						}
					}
					if (method.Name == "80EECA92933FCDB28F13F8A8A479BFBD")	//Timespan
					{
						ParameterInfo[] parameters = method.GetParameters();
						if (parameters[0].ParameterType.Name == "54FCB5BECF9CB9F5346B87773CF643A6")
						{
							m_registerPacketHandlerMethod3 = method;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		internal static bool RegisterCustomPacketHandler(PacketRegistrationType registrationType, Type packetType, MethodInfo handler, Type baseNetManagerType)
		{
			try
			{
				if (m_registerPacketHandlerMethod == null)
					return false;
				if (m_registerPacketHandlerMethod2 == null)
					return false;
				if (m_registerPacketHandlerMethod3 == null)
					return false;
				if (packetType == null)
					return false;
				if (handler == null)
					return false;

				//Find the old packet handler
				Type masterNetManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(NetworkManager.InternalNetManagerNamespace, NetworkManager.InternalNetManagerClass);
				FieldInfo packetRegisteryHashSetField = masterNetManagerType.GetField("9858E5CD512FFA5633683B9551FA4C30", BindingFlags.NonPublic | BindingFlags.Static);
				Object packetRegisteryHashSetRaw = packetRegisteryHashSetField.GetValue(null);
				HashSet<Object> packetRegisteryHashSet = UtilityFunctions.ConvertHashSet(packetRegisteryHashSetRaw);
				if (packetRegisteryHashSet.Count == 0)
					return false;
				Object matchedHandler = null;
				List<Object> matchedHandlerList = new List<object>();
				List<Type> messageTypes = new List<Type>();
				foreach (var entry in packetRegisteryHashSet)
				{
					FieldInfo delegateField = entry.GetType().GetField("C2AEC105AF9AB1EF82105555583139FC");
					Type fieldType = delegateField.FieldType;
					Type[] genericArgs = fieldType.GetGenericArguments();
					Type[] messageTypeArgs = genericArgs[1].GetGenericArguments();
					Type messageType = messageTypeArgs[0];
					if (messageType == packetType)
					{
						matchedHandler = entry;
						matchedHandlerList.Add(entry);
					}

					messageTypes.Add(messageType);
				}

				if (matchedHandlerList.Count > 1)
				{
					LogManager.APILog.WriteLine("Found more than 1 packet handler match for type '" + packetType.Name + "'");
					return false;
				}

				if (matchedHandler == null)
					return false;

				FieldInfo field = matchedHandler.GetType().GetField("C2AEC105AF9AB1EF82105555583139FC", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				Object value = field.GetValue(matchedHandler);
				FieldInfo secondaryFlagsField = matchedHandler.GetType().GetField("655022D3B2BE47EBCBA675CCE8B784AD", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				Object secondaryFlags = secondaryFlagsField.GetValue(matchedHandler);
				MulticastDelegate action = (MulticastDelegate)value;
				object target = action.Target;
				FieldInfo field2 = target.GetType().GetField("F774FA5087F549F79181BD64E7B7FF30", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				Object value2 = field2.GetValue(target);
				MulticastDelegate action2 = (MulticastDelegate)value2;
				object target2 = action2.Target;

				string field3Name = "";
				string flagsFieldName = "";
				string serializerFieldName = "";
				switch (registrationType)
				{
					case PacketRegistrationType.Static:
						field3Name = "2919BD18904683E267BFC1B48709D971";
						flagsFieldName = "0723D9EDBE6B0BE979D08F70F06DA741";
						serializerFieldName = "9F70C9F89F36D5FC6C1EB816AFF491A9";
						break;
					case PacketRegistrationType.Instance:
						field3Name = "F6EE81B03BFA4E50FF9E5E08DA897D98";
						flagsFieldName = "A766151383CE73157E57B8ACCB430B04";
						serializerFieldName = "501AE44AC35E909FEB7EAEE4264D3398";
						break;
					case PacketRegistrationType.Timespan:
						field3Name = "B76A60C8C0680C4AD569366502B8CF47";
						flagsFieldName = "90FCC62CB555FB7216BD38667979B221";
						serializerFieldName = "DB0C5A72269DAB99179543BE08EEADAB";
						break;
					default:
						return false;
				}
				FieldInfo field3 = target2.GetType().GetField(field3Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				Object value3 = field3.GetValue(target2);
				MulticastDelegate action3 = (MulticastDelegate)value3;

				FieldInfo flagsField = target2.GetType().GetField(flagsFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				Object flagsValue = flagsField.GetValue(target2);
				FieldInfo serializerField = target2.GetType().GetField(serializerFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				Object serializerValue = serializerField.GetValue(target2);

				FieldInfo methodBaseField = action3.GetType().GetField("_methodBase", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				FieldInfo methodPtrField = action3.GetType().GetField("_methodPtr", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				FieldInfo methodPtrAuxField = action3.GetType().GetField("_methodPtrAux", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

				Delegate handlerAction = CreatePacketHandlerDelegate(registrationType, packetType, handler);

				//Remove the old handler from the registry
				MethodInfo removeMethod = packetRegisteryHashSetRaw.GetType().GetMethod("Remove");
				removeMethod.Invoke(packetRegisteryHashSetRaw, new object[] { matchedHandler });

				//Update the handler delegate with our new method info
				methodBaseField.SetValue(action3, handlerAction.Method);
				methodPtrField.SetValue(action3, methodPtrField.GetValue(handlerAction));
				methodPtrAuxField.SetValue(action3, methodPtrAuxField.GetValue(handlerAction));

				if(baseNetManagerType == null)
					baseNetManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType("5F381EA9388E0A32A8C817841E192BE8", "48D79F8E3C8922F14D85F6D98237314C");

				//Register the new packet handler
				MethodInfo registerMethod = null;
				switch (registrationType)
				{
					case PacketRegistrationType.Static:
						registerMethod = m_registerPacketHandlerMethod.MakeGenericMethod(packetType);
						registerMethod.Invoke(null, new object[] { action3, flagsValue, secondaryFlags, serializerValue });
						break;
					case PacketRegistrationType.Instance:
						registerMethod = m_registerPacketHandlerMethod2.MakeGenericMethod(baseNetManagerType, packetType);
						registerMethod.Invoke(null, new object[] { action3, flagsValue, secondaryFlags, serializerValue });
						break;
					case PacketRegistrationType.Timespan:
						registerMethod = m_registerPacketHandlerMethod3.MakeGenericMethod(packetType);
						registerMethod.Invoke(null, new object[] { action3, flagsValue, secondaryFlags, serializerValue });
						break;
					default:
						return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		internal delegate void ReceivePacketStatic<T>(ref T packet, Object netManager) where T : struct;
		internal delegate void ReceivePacketInstance<T>(Object instanceNetManager, ref T packet, Object masterNetManager) where T : struct;
		internal delegate void ReceivePacketTimespan<T>(ref T packet, Object netManager, TimeSpan time) where T : struct;

		#endregion
	}
}
