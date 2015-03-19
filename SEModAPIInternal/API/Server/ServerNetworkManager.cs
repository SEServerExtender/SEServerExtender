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

		public static string ServerNetworkManagerNamespace = "";
		public static string ServerNetworkManagerClass = "=O1XbQ6WcY6sEbcduBcq5Wrj8ok=";

		//public static string ServerNetworkManagerDisconnectPlayerMethod = "MyDedicatedServer_ClientLeft";
		//public static string ServerNetworkManagerSetPlayerBannedMethod = "7746929EC77B732BB52EFC8FC757085D";
		//public static string ServerNetworkManagerKickPlayerMethod = "33F8624DFE8A8E3FA9B8CC9A2446CCB8";
		public static string ServerNetworkManagerDisconnectPlayerMethod = "MyDedicatedServer_ClientLeft";
		public static string ServerNetworkManagerSetPlayerBannedMethod = "BanClient";
		public static string ServerNetworkManagerKickPlayerMethod = "KickClient";         
        
        public static string ServerNetworkManagerConnectedPlayersField = "=hrd2r4yATC2NCJnhqOpJBW6EOU=";

		///////////// All these need testing /////////////
		public static string NetworkingNamespace = "";
		public static string NetworkingOnWorldRequestField = "=brsw3F6mys5AacUJHYIS14TVH2=";

		public static string MyMultipartMessageClass = "=cDj6WIzYFodmTa89pvnrADNanH=";
		public static string MyMultipartMessagePreemble = "SendPreemble";

		public static string MySyncLayerClass = "=KgBTSimjVMGbRzYnmrvyNW6go7=";
		public static string MySyncLayerField = "=CuPFQfpJpbb8r5FyAD4AIHGjwn=";

		public static string MyTransportLayerField = "=hh7T0u2R6UFfq7GWwBLvlXzDmY=";
		public static string MyTransportLayerClearMethod = "SendFlush";

		public static string MyMultipartSenderClass = "=mPqaTNwLBiwakf5CK3At5qdRPuP=";
		public static string MyMultipartSenderSendPart = "SendPart";

		public static string MySyncLayerSendMessage = "SendMessage";
		public static string MySyncLayerSendMessageToServer = "SendMessageToServer";

		public static string MyControlMessageCallbackClass = "";
		public static string MyControlMessageHandlerClass = "=ejTBGKAhbRZPjUJjNkuRRlSLGh=";

		///////////// All these need testing /////////////

		private static string MultiplayerNamespace = "";

		private static string SendCloseClass = "=uTZ4uCH8frEHtn0VfQJ0coHE2v=";
		private static string SendCloseClosedMsg = "=gWIXWndvNVwS3ts7qLawsCo4L6=";
		private static string SendCloseClosedMsgEntityId = "=oU1leYhaRCXt3fs7qWXtuRa3yR=";

		private static string SendCreateClass = "=r6VaZpriOkuuKqMuT2aPUQtWow=";
		private static string SendCreateRelativeCompressedMsg = "=Jb9TlhCazLAuTwbBe9RenF4v7Q=";
		private static string SendCreateCompressedMsg = "=P5EKdj3EJhVtTJV0b2GCB1xNHsc=";

		private static string SendCreateRelativeCompressedMsgCreateMessage = "=tQs567mzcxroGhjYr8lz1PqzoU=";
		private static string SendCreateRelativeCompressedMsgBaseEntity = "=xruOKzLEhNYhebgpKfA5KhhhXQ=";
		private static string SendCreateRelativeCompressedMsgRelativeVelocity = "=fhrdPJMxlPgyEkzKIN17aUZ71n=";

		private static string SendCreateCompressedMsgObjectBuilders = "=DH7Oztbz8DbiZ1r4yCr7zTe1nP=";
		private static string SendCreateCompressedMsgBuilderLengths = "=5pfmIWWAOZCsIysc7YMDSWY13g=";

		private static string PlayerCollectionClass = "=DRGhkayMZHgcBGswfi83RRIYcc=";

		private static string RespawnMsg = "=Eyig3zbAKk28MoFuUhwULbgO9h=";
		private static string RespawnMsgJoinGame = "=A029yp2A0WBnWMZpQSa7psz5f2=";
		private static string RespawnMsgNewIdenity = "=4Wq6erLoLZgEqlM1zSSWH2WWjO=";
		private static string RespawnMsgMedicalRoom = "=ShCxItwZ8brXdPEkz0JNzn1Fxp=";
		private static string RespawnMsgRespawnShipId = "=at6Sy5bwukZGZdQGJYlInzdVm8=";
		private static string RespawnMsgPlayerSerialId = "=YtTbep5CcJrw9BQnTJKPIk1pIFC=";

		private static string CharacterClass = "=2f1qCclG2slBN6Vw5QbX1zDIG6=";
		private static string AttachMsg = "=XEAo5lB7RgcI6LNeKCn5RQSxUd=";
		private static string AttachCharacterId = "=QoeTaBdFG5hLQTySy0Rra15gNK=";
		private static string AttachCockpitId = "=KTOQgaLVpUNcBVMewWW3FM4m3y=";

		private static string ControllableClass = "=mk2iB8SwAcxjNcwkIM1EjyAgDq=";
		private static string UseMsg = "=teayYzvyUnChFRguXuaAzby6dA=";
		private static string UseMsgEntityId = "=oU1leYhaRCXt3fs7qWXtuRa3yR=";
		private static string UseMsgUsedByEntityId = "=tYrDGqpzipKWJtDSv3yh7YoVyw=";
		private static string UseMsgUseAction = "=qQBgTP4aIaCfxhNjdeAaCPQv9fr=";
		private static string UseMsgActionResult = "=61UFXGPQBQkmqO1WnSNBjwgqVb=";

		private static string ModAPINamespace = "";
		private static string ModAPIHelperClass = "=llwrhjftxf0UHKTxHfJpMmeCEs=";
		private static string SendDataMessageClass = "=ccuaV71K6ms95rCYBXoWr8qjT2=";
		private static string SendReliableMsg = "=1FUeCV7aAYZCGFd3KUhH5A0mEe=";
		private static string SendReliableMsgId = "=uRgMOkIX0djjiF3VDo6Hr4CmJx=";
		private static string SendReliableMsgData = "=WNLspkvjzVBe8e418uPlh3EhJN=";

		private static string ControlChangedMsg = "=Py6oB722vMEMLvXlAoczDrpoOu=";
		private static string ControlChangedMsgEntityId = "=oU1leYhaRCXt3fs7qWXtuRa3yR=";
		private static string ControlChangedMsgSteamId = "=uj3LI43ZxE5HcfdPWFZSliFEA1=";
		private static string ControlChangedMsgSerialId = "=YtTbep5CcJrw9BQnTJKPIk1pIFC=";

		private static string SetPlayerDeadMsg = "=6pj8lSLelEguuqvNjgO6Hglqax=";
		private static string SetPlayerDeadMsgSteamId = "=uj3LI43ZxE5HcfdPWFZSliFEA1=";
		private static string SetPlayerDeadMsgSerialId = "=YtTbep5CcJrw9BQnTJKPIk1pIFC=";
		private static string SetPlayerDeadMsgIsDead = "=vJoyYM9pjeoG12SSUAOQjNFTHG=";

		private static string CharacterChangedMsg = "=GLzqhdD8SwqaIlodUM9kTayE7u=";
		private static string CharacterChangedMsgCharacterEntityId = "=QoeTaBdFG5hLQTySy0Rra15gNK=";
		private static string CharacterChangedMsgControlledEntityId = "=rZPDoWdhk5ehIChJ7JrirJmF7k=";
		private static string CharacterChangedMsgSteamId = "=uj3LI43ZxE5HcfdPWFZSliFEA1=";
		private static string CharacterChangedMsgClientId = "=YtTbep5CcJrw9BQnTJKPIk1pIFC=";

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

				Type type6 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, CharacterClass);
				if (type5 == null)
					throw new Exception("Could not find internal type for CharacterClass");

				Type attachMsgType = type6.GetNestedType(AttachMsg, BindingFlags.NonPublic | BindingFlags.Public);
				if (attachMsgType == null)
					throw new Exception("Could not find internal type for AttachMsg");

				result &= BaseObject.HasField(attachMsgType, AttachCharacterId);
				result &= BaseObject.HasField(attachMsgType, AttachCockpitId);

				Type controllableClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, ControllableClass);
				if (type5 == null)
					throw new Exception("Could not find internal type for ControllableClassType");

				Type useMsgType = controllableClassType.GetNestedType(UseMsg, BindingFlags.NonPublic | BindingFlags.Public);
				if (useMsgType == null)
					throw new Exception("Could not find internal type for UseMsg");

				result &= BaseObject.HasField(useMsgType, UseMsgEntityId);
				result &= BaseObject.HasField(useMsgType, UseMsgUsedByEntityId);
				result &= BaseObject.HasField(useMsgType, UseMsgUseAction);

				Type modAPIHelperClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ModAPINamespace, ModAPIHelperClass);
				if (modAPIHelperClassType == null)
					throw new Exception("Could not find internal type for ModAPIHelperClass");

				Type sendDataMessageClassType = modAPIHelperClassType.GetNestedType(SendDataMessageClass, BindingFlags.Public | BindingFlags.NonPublic);
				if (sendDataMessageClassType == null)
					throw new Exception("Could not find internal type for SendDataMessageClass");

				Type sendReliableMsgType = sendDataMessageClassType.GetNestedType(SendReliableMsg, BindingFlags.Public | BindingFlags.NonPublic);
				if (sendReliableMsgType == null)
					throw new Exception("Could not find internal type for SendReliableMsg");

				result &= BaseObject.HasField(sendReliableMsgType, SendReliableMsgId);
				result &= BaseObject.HasField(sendReliableMsgType, SendReliableMsgData);

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

		public static void SendControlMsgChangedSuccess(long entityId, ulong controlSteamid, ulong steamId)
		{
			Type playerCollectionClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, PlayerCollectionClass);
			Type controlChangedMsgType = playerCollectionClassType.GetNestedType(ControlChangedMsg, BindingFlags.Public | BindingFlags.NonPublic);

			FieldInfo controlChangedMsgEntityId = controlChangedMsgType.GetField(ControlChangedMsgEntityId);
			FieldInfo controlChangedMsgSteamId = controlChangedMsgType.GetField(ControlChangedMsgSteamId);
			FieldInfo controlChangedMsgSerialId = controlChangedMsgType.GetField(ControlChangedMsgSerialId);

			object controlChangedMsg = Activator.CreateInstance(controlChangedMsgType);

			controlChangedMsgEntityId.SetValue(controlChangedMsg, entityId);
			controlChangedMsgSteamId.SetValue(controlChangedMsg, controlSteamid);
			controlChangedMsgSerialId.SetValue(controlChangedMsg, 0);

			SendMessage(controlChangedMsg, steamId, controlChangedMsgType, 1);
		}

		public static void SendSetPlayerDead(ulong controlSteamId, bool isDead, ulong steamId)
		{
			Type playerCollectionClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, PlayerCollectionClass);
			Type setPlayerDeadMsgType = playerCollectionClassType.GetNestedType(SetPlayerDeadMsg, BindingFlags.Public | BindingFlags.NonPublic);

			FieldInfo setPlayerDeadMsgSteamId = setPlayerDeadMsgType.GetField(SetPlayerDeadMsgSteamId);
			FieldInfo setPlayerDeadMsgSerialId = setPlayerDeadMsgType.GetField(SetPlayerDeadMsgSerialId);
			FieldInfo setPlayerDeadMsgIsDead = setPlayerDeadMsgType.GetField(SetPlayerDeadMsgIsDead);

			object setPlayerDeadMsg = Activator.CreateInstance(setPlayerDeadMsgType);

			setPlayerDeadMsgSteamId.SetValue(setPlayerDeadMsg, controlSteamId);
			setPlayerDeadMsgSerialId.SetValue(setPlayerDeadMsg, 0);
			setPlayerDeadMsgIsDead.SetValue(setPlayerDeadMsg, (BoolBlit)isDead);

			SendMessage(setPlayerDeadMsg, steamId, setPlayerDeadMsgType, 2);
		}

		public static void SendCharacterChanged(long characterEntityId, long controlledEntityId, ulong controlSteamId, ulong steamId)
		{
			Type playerCollectionClassType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MultiplayerNamespace, PlayerCollectionClass);
			Type characterChangedMsgType = playerCollectionClassType.GetNestedType(CharacterChangedMsg, BindingFlags.Public | BindingFlags.NonPublic);

			FieldInfo characterChangedMsgCharacterEntityId = characterChangedMsgType.GetField(CharacterChangedMsgCharacterEntityId);
			FieldInfo characterChangedMsgControlledEntityId = characterChangedMsgType.GetField(CharacterChangedMsgControlledEntityId);
			FieldInfo characterChangedMsgSteamId = characterChangedMsgType.GetField(CharacterChangedMsgSteamId);
			FieldInfo characterChangedMsgClientId = characterChangedMsgType.GetField(CharacterChangedMsgClientId);

			object characterChangeMsg = Activator.CreateInstance(characterChangedMsgType);

			characterChangedMsgCharacterEntityId.SetValue(characterChangeMsg, characterEntityId);
			characterChangedMsgControlledEntityId.SetValue(characterChangeMsg, controlledEntityId);
			characterChangedMsgSteamId.SetValue(characterChangeMsg, controlSteamId);
			characterChangedMsgClientId.SetValue(characterChangeMsg, 0);

			SendMessage(characterChangeMsg, steamId, characterChangedMsgType, 1);
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

		//.5B9DDD8F4DF9A88D297B3B0B3B79FBAA
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
							
							if (!(entity is MyObjectBuilder_CubeGrid) && !(entity is MyObjectBuilder_VoxelMap) && !(entity is MyObjectBuilder_Character))
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
			//.=cDj6WIzYFodmTa89pvnrADNanH=.SendPreemble
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
