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
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPIInternal.Support;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.API.Common;
using System.Linq.Expressions;

using VRageMath;

using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;

namespace SEModAPIInternal.API.Server
{
	public class ServerNetworkManager : NetworkManager
	{
		#region "Attributes"
		new protected static ServerNetworkManager m_instance;

		private static MulticastDelegate m_onWorldRequest;
		private static Type m_onWorldRequestType;
		private static int[] m_speeds = { 512, 256, 128, 128 };
		private static bool replaceData = false;
		private static List<ulong> m_responded = new List<ulong>();
		private static List<ulong> m_clearResponse = new List<ulong>();
		private static List<ulong> m_inGame = new List<ulong>();
		private static Dictionary<ulong, Tuple<DateTime, int>> m_slowDown = new Dictionary<ulong, Tuple<DateTime, int>>();

		public static string ServerNetworkManagerNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string ServerNetworkManagerClass = "3B0B7A338600A7B9313DE1C3723DAD14";

		//public static string ServerNetworkManagerDisconnectPlayerMethod = "3EA4ED71531B0189F424CC7CD66E6524";
		//public static string ServerNetworkManagerSetPlayerBannedMethod = "5BD41EA814E02B4E4D0F12843FEB47FC";
		//public static string ServerNetworkManagerKickPlayerMethod = "4F49605BE0CBDF17EA3E46B5FCEFB1D7";
		public static string ServerNetworkManagerDisconnectPlayerMethod = "3EA4ED71531B0189F424CC7CD66E6524";
		public static string ServerNetworkManagerSetPlayerBannedMethod = "8BCE3804ABCEC7625C4D56B74B5FF98C";
		public static string ServerNetworkManagerKickPlayerMethod = "CCF347D895F54AB46484A67F94FF7AC2";         
        
        public static string ServerNetworkManagerConnectedPlayersField = "89E92B070228A8BC746EFB57A3F6D2E5";

		///////////// All these need testing /////////////
		public static string NetworkingNamespace = "36CC7CE820B9BBBE4B3FECFEEFE4AE86";
		public static string NetworkingOnWorldRequestField = "FAD031AB4FED05B9FE273ACD199496EE";

		public static string MyMultipartMessageClass = "7B6560DE2B6A29DE7F0157E9CDFFCC37";
		public static string MyMultipartMessagePreemble = "7AEDE70A5F16434A660FC187077FC86F";

		public static string MySyncLayerClass = "08FBF1782D25BEBDA2070CAF8CE47D72";
		public static string MySyncLayerField = "E863C8EAD57B154571B7A487C6A39AC6";

		public static string MyTransportLayerField = "6F79877D9F8B092082EAEF8828D69F98";
		public static string MyTransportLayerClearMethod = "DA0F40A1E0E2E5DD9B141562B91BDDDC";

		public static string MyMultipartSenderClass = "73C7CA87DB0535EFE711E10913D8ACFB";
		public static string MyMultipartSenderSendPart = "A822BAC1F661C682C78230403DDF5670";

		public static string MySyncLayerSendMessage = "358D29D15C14B49FEA47651E0DE22024";
		public static string MySyncLayerSendMessageToServer = "161C8D41497D2D26777646EE58FE2841";

		public static string MyControlMessageCallbackClass = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string MyControlMessageHandlerClass = "69BCF201AF4FAC4108B36AFA089FE230";

		///////////// All these need testing /////////////

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

		private static string PlayerCollectionClass = "E4C2964159826A46D102C2D7FDDC0733";

		private static string RespawnMsg = "0375BD13880985CF806BF38D80ABE4DB";
		private static string RespawnMsgJoinGame = "3091FD2824A71EAAD2D74C723BF1EE19";
		private static string RespawnMsgNewIdenity = "31F31D5F94108B5AF5DB5BFC01287B6A";
		private static string RespawnMsgMedicalRoom = "979224CC53178892C95097C126832539";
		private static string RespawnMsgRespawnShipId = "EA3E01C01EE14785DE672E6899318CA4";
		private static string RespawnMsgPlayerSerialId = "221D28591CBBCE26D7F0FC462FFB53E4";

		private static string CharacterClass = "FA70B722FFD1F55F5A5019DA2499E60B";
		private static string AttachMsg = "40523128EAA280C3B469E3C07BC9AA59";
		private static string AttachCharacterId = "42A1FAB6988564AD174FAFE32427AF1F";
		private static string AttachCockpitId = "92AC7F78274FB9C79B48F68D03166DC5";

		private static string ControllableClass = "13C872C66F0BD2DC78D3D80ECAF6DD0E";
		private static string UseRequest = "580BEBFBA2193A07A867968A00F933D1";
		private static string UseMsg = "B9061E64FCAE2676D7C8BB0CBEB2B558";
		private static string UseMsgEntityId = "B8FB60B21AA9E31FBE1BE03977FBB5C4";
		private static string UseMsgUsedByEntityId = "1EDCD7F5F272CEC95910B9BD327F8159";
		private static string UseMsgUseAction = "D1AB76CECD107930E4CED6045B5EE206";
		private static string UseMsgActionResult = "184D262E162762B50FBAF1A443AE3F48";

		private static string ModAPINamespace = "91D02AC963BE35D1F9C1B9FBCFE1722D";
		private static string ModAPIHelperClass = "4C1ED56341F07A7D73298D03926F04DE";
		private static string SendDataMessageClass = "CC6EB6677E764BA0BB8C9E3F219B7FB6";
		private static string SendReliableMsg = "94BA33CF24FDB04C5858133B9CA10B65";
		private static string SendReliableMsgId = "A1065D4F4F78592D380E3EBA7517D263";
		private static string SendReliableMsgData = "EFAAFF2EF963935E2CE19D55C6C98DD1";
			
		#endregion

		#region "Properties"

		new public static ServerNetworkManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ServerNetworkManager();

				return m_instance;
			}
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ServerNetworkManagerNamespace, ServerNetworkManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for ServerNetworkManager");
				bool result = true;
				result &= BaseObject.HasMethod(type1, ServerNetworkManagerSetPlayerBannedMethod);
				result &= BaseObject.HasMethod(type1, ServerNetworkManagerDisconnectPlayerMethod);
				result &= BaseObject.HasMethod(type1, ServerNetworkManagerKickPlayerMethod);
				result &= BaseObject.HasField(type1, ServerNetworkManagerConnectedPlayersField);

				Type type2 = NetworkManagerType;
				if (type2 == null)
					throw new Exception("Could not find internal type for NetworkManager");
				result &= BaseObject.HasField(type2, MySyncLayerField);

				Type syncLayer = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, MySyncLayerClass);
				if (syncLayer == null)
					throw new Exception("Could not find internal type for SyncLayer");
				result &= BaseObject.HasMethod(syncLayer, MySyncLayerSendMessage);

				Type type3 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, SendCloseClass);
				if (type3 == null)
					throw new Exception("Could not find internal type for SendCloseClass");

				result &= BaseObject.HasNestedType(type3, SendCloseClosedMsg);

				Type nestedClosedMsgType = type3.GetNestedType(SendCloseClosedMsg, BindingFlags.NonPublic | BindingFlags.Public);
				if(nestedClosedMsgType == null)
					throw new Exception("Could not find internal NestedClosedMsgType");

				result &= BaseObject.HasField(nestedClosedMsgType, SendCloseClosedMsgEntityId);

				Type type4 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, SendCreateClass);
				if (type4 == null)
					throw new Exception("Could not find internal type for SendCreateClass");

				result &= BaseObject.HasNestedType(type4, SendCreateCompressedMsg);

				Type nestedCreateType = type4.GetNestedType(SendCreateCompressedMsg, BindingFlags.Public | BindingFlags.NonPublic);
				if(nestedCreateType == null)
					throw new Exception("Could not find internal type for SendCreateCompressedMsg");

				result &= BaseObject.HasField(nestedCreateType, SendCreateCompressedMsgObjectBuilders);
				result &= BaseObject.HasField(nestedCreateType, SendCreateCompressedMsgBuilderLengths);

				Type type5 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, PlayerCollectionClass);
				if (type5 == null)
					throw new Exception("Could not find internal type for PlayerCollectionClass");

				Type respawnMsgType = type5.GetNestedType(RespawnMsg, BindingFlags.NonPublic | BindingFlags.Public);
				if (respawnMsgType == null)
					throw new Exception("Could not find internal type for RespawnMsg");


				result &= BaseObject.HasField(respawnMsgType, RespawnMsgJoinGame);
				result &= BaseObject.HasField(respawnMsgType, RespawnMsgNewIdenity);
				result &= BaseObject.HasField(respawnMsgType, RespawnMsgMedicalRoom);
				result &= BaseObject.HasField(respawnMsgType, RespawnMsgRespawnShipId);
				result &= BaseObject.HasField(respawnMsgType, RespawnMsgPlayerSerialId);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		public override List<ulong> GetConnectedPlayers()
		{
			try
			{
				Object steamServerManager = GetNetworkManager();
				if (steamServerManager == null)
					return new List<ulong>();

				FieldInfo connectedPlayersField = steamServerManager.GetType().GetField(ServerNetworkManagerConnectedPlayersField, BindingFlags.NonPublic | BindingFlags.Instance);
				List<ulong> connectedPlayers = (List<ulong>)connectedPlayersField.GetValue(steamServerManager);

				return connectedPlayers;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new List<ulong>();
			}
		}

		public void SetPlayerBan(ulong remoteUserId, bool isBanned)
		{
			try
			{
				Object steamServerManager = GetNetworkManager();
				BaseObject.InvokeEntityMethod(steamServerManager, ServerNetworkManagerSetPlayerBannedMethod, new object[] { remoteUserId, isBanned });

				KickPlayer(remoteUserId);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void KickPlayer(ulong remoteUserId)
		{
			try
			{
				Object steamServerManager = GetNetworkManager();
				BaseObject.InvokeEntityMethod(steamServerManager, ServerNetworkManagerKickPlayerMethod, new object[] { remoteUserId });
				BaseObject.InvokeEntityMethod(steamServerManager, ServerNetworkManagerDisconnectPlayerMethod, new object[] { remoteUserId, ChatMemberStateChangeEnum.Kicked });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
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

		private static void SendMessageToServer(object msg, Type msgType, int flag)
		{
			try
			{
				var netManager = GetNetworkManager();
				var mySyncLayer = BaseObject.GetEntityFieldValue(netManager, MySyncLayerField);
				MethodInfo sendMessageMethod = mySyncLayer.GetType().GetMethod(MySyncLayerSendMessageToServer);
				sendMessageMethod = sendMessageMethod.MakeGenericMethod(msgType);
				sendMessageMethod.Invoke(mySyncLayer, new object[] { msg, flag });
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

				SendMessage(sendCloseStruct, userId, sendCloseType, 2);
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
			// Extract Type and Field Info
			Type sendCreateClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, SendCreateClass);
			Type sendCreateRelativeCompressedMsgType = sendCreateClassType.GetNestedType(SendCreateRelativeCompressedMsg, BindingFlags.NonPublic);
			Type sendCreateCompressedMsgType = sendCreateClassType.GetNestedType(SendCreateCompressedMsg, BindingFlags.NonPublic);

			FieldInfo createMessageField = sendCreateRelativeCompressedMsgType.GetField(SendCreateRelativeCompressedMsgCreateMessage);
			FieldInfo createBaseEntity = sendCreateRelativeCompressedMsgType.GetField(SendCreateRelativeCompressedMsgBaseEntity);
			FieldInfo createRelativeVelocity = sendCreateRelativeCompressedMsgType.GetField(SendCreateRelativeCompressedMsgRelativeVelocity);

			FieldInfo createObjectBuilders = sendCreateCompressedMsgType.GetField(SendCreateCompressedMsgObjectBuilders);
			FieldInfo createBuilderLengths = sendCreateCompressedMsgType.GetField(SendCreateCompressedMsgBuilderLengths);

			// Run logic
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

		public static void ShowRespawnMenu(ulong userId)
		{
			Type playerCollectionType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, PlayerCollectionClass);
			Type respawnMsgType = playerCollectionType.GetNestedType(RespawnMsg, BindingFlags.NonPublic | BindingFlags.Public);

			FieldInfo respawnMsgJoinGame = respawnMsgType.GetField(RespawnMsgJoinGame);
			FieldInfo respawnMsgNewIdentity = respawnMsgType.GetField(RespawnMsgNewIdenity);
			FieldInfo respawnMsgMedicalRoom = respawnMsgType.GetField(RespawnMsgMedicalRoom);
			FieldInfo respawnMsgRespawnShipId = respawnMsgType.GetField(RespawnMsgRespawnShipId);
			FieldInfo respawnMsgPlayerSerialId = respawnMsgType.GetField(RespawnMsgPlayerSerialId);

			object respawnMsg = Activator.CreateInstance(respawnMsgType);

			respawnMsgJoinGame.SetValue(respawnMsg, true);
			respawnMsgNewIdentity.SetValue(respawnMsg, true);
			respawnMsgMedicalRoom.SetValue(respawnMsg, 0);
			respawnMsgRespawnShipId.SetValue(respawnMsg, "");
			respawnMsgPlayerSerialId.SetValue(respawnMsg, 0);

			SendMessage(respawnMsg, userId, respawnMsgType, 3);
		}

		public static void AttachToCockpit(long characterId, long cockpitId, ulong steamId)
		{
			Type syncCharacterClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, CharacterClass);
			Type attachMsgType = syncCharacterClassType.GetNestedType(AttachMsg, BindingFlags.NonPublic | BindingFlags.Public);

			FieldInfo attachCharacterId = attachMsgType.GetField(AttachCharacterId);
			FieldInfo attachCockpitId = attachMsgType.GetField(AttachCockpitId);

			object attachMsg = Activator.CreateInstance(attachMsgType);

			attachCharacterId.SetValue(attachMsg, characterId);
			attachCockpitId.SetValue(attachMsg, cockpitId);

			SendMessage(attachMsg, steamId, attachMsgType, 1);
		}

		public static void UseCockpit(long characterId, long cockpitId)
		{
			Type syncControllableClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, ControllableClass);
			Type useMsgType = syncControllableClassType.GetNestedType(UseMsg, BindingFlags.Public | BindingFlags.NonPublic);

			FieldInfo useMsgEntityId = useMsgType.GetField(UseMsgEntityId);
			FieldInfo useMsgUsedByEntityId = useMsgType.GetField(UseMsgUsedByEntityId);
			FieldInfo useMsgUseAction = useMsgType.GetField(UseMsgUseAction);

			object useMsg = Activator.CreateInstance(useMsgType);

			useMsgEntityId.SetValue(useMsg, cockpitId);
			useMsgUsedByEntityId.SetValue(useMsg, characterId);
			useMsgUseAction.SetValue(useMsg, 1);

			SendMessageToServer(useMsg, useMsgType, 1);
		}

		public static void SendUseCockpitSuccess(long characterId, long cockpitId, ulong steamId)
		{
			Type syncControllableClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, ControllableClass);
			Type useMsgType = syncControllableClassType.GetNestedType(UseMsg, BindingFlags.Public | BindingFlags.NonPublic);

			FieldInfo useMsgEntityId = useMsgType.GetField(UseMsgEntityId);
			FieldInfo useMsgUsedByEntityId = useMsgType.GetField(UseMsgUsedByEntityId);
			FieldInfo useMsgUseAction = useMsgType.GetField(UseMsgUseAction);
			FieldInfo useMsgUseActionResult = useMsgType.GetField(UseMsgActionResult);

			object useMsg = Activator.CreateInstance(useMsgType);

			useMsgEntityId.SetValue(useMsg, cockpitId);
			useMsgUsedByEntityId.SetValue(useMsg, characterId);
			useMsgUseAction.SetValue(useMsg, 1);
			useMsgUseActionResult.SetValue(useMsg, 0);

			SendMessage(useMsg, steamId, useMsgType, 2);
		}

		public static void SendDataMessage(ushort dataId, byte[] data, ulong steamId)
		{
			Type modAPIHelperClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ModAPINamespace, ModAPIHelperClass);
			Type sendDataMessageClassType = modAPIHelperClassType.GetNestedType(SendDataMessageClass, BindingFlags.Public | BindingFlags.NonPublic);
			Type sendReliableMsgType = sendDataMessageClassType.GetNestedType(SendReliableMsg, BindingFlags.Public | BindingFlags.NonPublic);

			FieldInfo sendReliableMsgId = sendReliableMsgType.GetField(SendReliableMsgId);
			FieldInfo sendReliableMsgData = sendReliableMsgType.GetField(SendReliableMsgData);

			object sendReliableMsg = Activator.CreateInstance(sendReliableMsgType);

			sendReliableMsgId.SetValue(sendReliableMsg, dataId);
			sendReliableMsgData.SetValue(sendReliableMsg, data);

			SendMessage(sendReliableMsg, steamId, sendReliableMsgType, 2);
		}

		//C42525D7DE28CE4CFB44651F3D03A50D.5B9DDD8F4DF9A88D297B3B0B3B79FBAA
		public void ReplaceWorldJoin()
		{
			try
			{
				var netManager = GetNetworkManager();
				var controlHandlerField = BaseObject.GetEntityFieldValue(netManager, NetworkManagerControlHandlerField);
				Dictionary<int, object> controlHandlers = UtilityFunctions.ConvertDictionary<int>(controlHandlerField);
				MethodInfo removeMethod = controlHandlerField.GetType().GetMethod("Remove");
				removeMethod.Invoke(controlHandlerField, new object[] { 0 });

				ThreadPool.QueueUserWorkItem((state) =>
				{
					OnWorldRequestReplace(state);
				});

				// Garbage is below as I tried to create a generic delegate and put it into the command handling dictionary.  It's not
				// as straightforward as it seems.  I can get the type, object, generic types, but I can't seem to create a delegate
				// that matches the what they have in game without it throwing an error

				/*
				var worldJoinField = controlHandlers[0];
				//FAD031AB4FED05B9FE273ACD199496EE

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
		private void OnWorldRequestReplace(object state)
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
		private static void SendWorldData(ulong steamId)
		{
			try
			{
				MemoryStream ms = new MemoryStream();
				if (MyAPIGateway.Session != null)
				{
					DateTime start = DateTime.Now;
					LogManager.APILog.WriteLineAndConsole(string.Format("...responding to user: {0}", steamId));
					SendPreamble(steamId, 1);
					SendFlush(steamId);

					// Let's sleep for 5 seconds and let plugins know we're online -- let's not after all, causing sync issues
					//Thread.Sleep(5000);
					MyObjectBuilder_World myObjectBuilderWorld = null;
					lock (m_inGame)
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
							
							if (!(entity is MyObjectBuilder_CubeGrid) && !(entity is MyObjectBuilder_VoxelMap))
								continue;

							if ((entity is MyObjectBuilder_CubeGrid) && ((MyObjectBuilder_CubeGrid)entity).DisplayName.Contains("CommRelay"))
								continue;							

							/*
							if (!(entity is MyObjectBuilder_CubeGrid))
								continue;

							if ((entity.PersistentFlags & MyPersistentEntityFlags2.InScene) == MyPersistentEntityFlags2.InScene)
								continue;
							*/

							myObjectBuilderWorld.Sector.SectorObjects.RemoveAt(r);
						}

						myObjectBuilderWorld.Sector.Encounters = null;

						myObjectBuilderWorld.VoxelMaps.Dictionary.Clear();
						myObjectBuilderWorld.Checkpoint.Settings.ProceduralDensity = 0f;
						myObjectBuilderWorld.Checkpoint.Settings.ProceduralSeed = 0;

						// Check if this is OK?
						//myObjectBuilderWorld.Checkpoint.ConnectedPlayers.Dictionary.Clear();
						myObjectBuilderWorld.Checkpoint.DisconnectedPlayers.Dictionary.Clear();

						long playerId = PlayerMap.Instance.GetFastPlayerIdFromSteamId(steamId);
				
						MyObjectBuilder_Toolbar blankToolbar = new MyObjectBuilder_Toolbar();
						foreach(KeyValuePair<MyObjectBuilder_Checkpoint.PlayerId, MyObjectBuilder_Player> p in myObjectBuilderWorld.Checkpoint.AllPlayersData.Dictionary)
						{
							if (p.Value.EntityCameraData != null)
								p.Value.EntityCameraData.Clear();

							if (p.Value.CameraData != null)
								p.Value.CameraData.Dictionary.Clear();

							if (p.Key.ClientId == steamId)
							{
								continue;
							}

							p.Value.Toolbar = null;
							p.Value.CharacterCameraData = null;
						}

						for (int r = myObjectBuilderWorld.Checkpoint.Gps.Dictionary.Count - 1; r >= 0; r--)
						{
							KeyValuePair<long, MyObjectBuilder_Gps> p = myObjectBuilderWorld.Checkpoint.Gps.Dictionary.ElementAt(r);

							if (p.Key == playerId)
								continue;

							myObjectBuilderWorld.Checkpoint.Gps.Dictionary.Remove(p.Key);
						}

						myObjectBuilderWorld.Checkpoint.ChatHistory.RemoveAll(x => x.IdentityId != playerId);

						long factionId = 0;
						if (myObjectBuilderWorld.Checkpoint.Factions.Players.Dictionary.ContainsKey(playerId))
						{
							factionId = myObjectBuilderWorld.Checkpoint.Factions.Players.Dictionary[playerId];
							myObjectBuilderWorld.Checkpoint.FactionChatHistory.RemoveAll(x => x.FactionId1 != factionId && x.FactionId2 != factionId);
							myObjectBuilderWorld.Checkpoint.Factions.Requests.RemoveAll(x => x.FactionId != factionId);
						}
						else
						{
							myObjectBuilderWorld.Checkpoint.FactionChatHistory.Clear();
							myObjectBuilderWorld.Checkpoint.Factions.Requests.Clear();						
						}

						foreach (MyObjectBuilder_Faction faction in myObjectBuilderWorld.Checkpoint.Factions.Factions)
						{
							if (faction.FactionId != factionId)
							{
								faction.PrivateInfo = "";
							}
						}
					}

					MyObjectBuilder_Checkpoint checkpoint = myObjectBuilderWorld.Checkpoint;
					checkpoint.WorkshopId = null;
					checkpoint.CharacterToolbar = null;
					DateTime cs = DateTime.Now;
					MyObjectBuilderSerializer.SerializeXML(ms, myObjectBuilderWorld, MyObjectBuilderSerializer.XmlCompression.Gzip, null);

					/*
					MemoryStream vms = new MemoryStream();
					MyObjectBuilderSerializer.SerializeXML(vms, myObjectBuilderWorld, MyObjectBuilderSerializer.XmlCompression.Uncompressed, null);
					FileStream file = new FileStream("e:\\temp\\test.txt", FileMode.Create);
					vms.WriteTo(file);
					file.Close();
					*/

					LogManager.APILog.WriteLineAndConsole(string.Format("...response construction took {0}ms (cp - {1}ms) size - {2} bytes", (DateTime.Now - start).TotalMilliseconds, (DateTime.Now - cs).TotalMilliseconds, ms.Length));
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

		private static void SendPreamble(ulong steamId, int num)
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

		#endregion
	}

	public enum MyObjectSeedType
	{
		Asteroid,
		AsteroidCluster,
		EncounterAlone,
		EncounterSingle,
		EncounterMulti
	}
}
