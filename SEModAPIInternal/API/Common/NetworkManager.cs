using SEModAPIInternal.API.Server;

namespace SEModAPIInternal.API.Common
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Sandbox.Engine.Multiplayer;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;
	using SteamSDK;

	//PacketIdEnum Attribute: .4C6398741B0F8804D769E5A2E3999E1D

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
		ChatMessage = 13872,							//.12AEE9CB08C9FC64151B8A094D6BB668
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

		//This class is just a container for some basic steam game values as well as the actual network manager instance
		public static string NetworkManagerWrapperNamespace = "Sandbox.Engine.Multiplayer";
		public static string NetworkManagerWrapperClass = "MyMultiplayer";
		public static string NetworkManagerWrapperManagerInstanceField = "Static";
		public static string NetworkManagerControlType = "ITransportCallback";

		//This is an abstract class that the actual network managers implement
		public static string NetworkManagerNamespace = "Sandbox.Engine.Multiplayer";
		public static string NetworkManagerClass = "MyMultiplayerBase";
		public static string NetworkManagerSendStructMethod = "SendControlMessage";
		public static string NetworkManagerRegisterChatReceiverMethod = "add_ChatMessageReceived";
		public static string NetworkManagerInternalNetManagerField = "SyncLayer";
		public static string NetworkManagerControlHandlerField = "m_controlMessageHandlers";

		public static string InternalNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string InternalNetManagerClass = "MySyncLayer";
		public static string InternalNetManagerPacketRegistryField = "TransportLayer";
		public static string InternalNetManagerSendToAllExceptMethod = "SendMessageToAllButOne";
		public static string InternalNetManagerSendToAllMethod = "SendMessageToAll";

		public static string PacketRegistryNamespace = "Sandbox.Game.Multiplayer";
		public static string PacketRegistryClass = "MyTransportLayer";
		public static string PacketRegistryTypeIdMapField = "TypeMap";

		//36CC7CE820B9BBBE4B3FECFEEFE4AE86.7B6560DE2B6A29DE7F0157E9CDFFCC37.7AEDE70A5F16434A660FC187077FC86F

		/////////////////////////////////////////////////

		//1 Packet Type
		public static string GravityGeneratorNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string GravityGeneratorNetManagerClass = "MySyncGravityGenerator";

		//2 Packet Types
		public static string TerminalFunctionalBlocksNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string TerminalFunctionalBlocksNetManagerClass = "MySyncBlockHelpers";

		//2+ Packet Types
		public static string InventoryNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string InventoryNetManagerClass = "MySyncInventory";

		//1 Packet Type
		public static string ConveyorEnabledBlockNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string ConveyorEnabledBlockNetManagerClass = "MySyncConveyors";

		//3 Packet Types
		public static string FloatingObjectNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string FloatingObjectNetManagerClass = "MySyncFloatingObjects";

		//2 Packet Types
		public static string VoxelMapNetManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string VoxelMapNetManagerClass = "MySyncVoxel";


		#endregion

		#region "Constructors and Initializers"

		static NetworkManager( )
		{
			PreparePacketRegistrationMethod( );
		}

		protected NetworkManager( )
		{

			m_instance = this;
			ApplicationLog.BaseLog.Info(  "Finished loading NetworkManager" );
		}

		#endregion

		#region "Properties"

		public static NetworkManager Instance
		{
			get { return m_instance; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type networkManagerWrapper = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( NetworkManagerWrapperNamespace, NetworkManagerWrapperClass );
				networkManagerWrapper.GetField( NetworkManagerWrapperManagerInstanceField, BindingFlags.Static );
				BaseObject.GetStaticFieldValue( networkManagerWrapper, NetworkManagerWrapperManagerInstanceField );			

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error(  ex );
				return false;
			}
		}
		public static MyMultiplayerBase GetNetworkManager( )
		{
			try
			{
				MyMultiplayerBase networkManager = MyMultiplayer.Static;

				return networkManager;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		public void SendStruct( ulong remoteUserId, Object data, Type structType )
		{
			try
			{
				Type netManagerType = typeof ( MyMultiplayerBase );
				MethodInfo sendStructMethod = netManagerType.GetMethod( NetworkManagerSendStructMethod, BindingFlags.NonPublic | BindingFlags.Instance );
				sendStructMethod = sendStructMethod.MakeGenericMethod( structType );

				var netManager = GetNetworkManager( );
				sendStructMethod.Invoke( netManager, new object[ ] { remoteUserId, data } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public void RegisterChatReceiver( Action<ulong, string, ChatEntryTypeEnum> action )
		{
			try
			{
				var netManager = GetNetworkManager( );
				BaseObject.InvokeEntityMethod( netManager, NetworkManagerRegisterChatReceiverMethod, new object[ ] { action } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		private Type GetControlType( )
		{
			Type controlType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( NetworkManagerWrapperNamespace, NetworkManagerControlType );
			return controlType;
		}

		public abstract List<ulong> GetConnectedPlayers( );

		protected Object GetInternalNetManager( )
		{
			try
			{
				Object internalNetManager = BaseObject.GetEntityFieldValue( GetNetworkManager( ), NetworkManagerInternalNetManagerField );
				return internalNetManager;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected Object GetPacketRegistry( )
		{
			try
			{
				Object internalNetManager = GetInternalNetManager( );
				Object packetRegistry = BaseObject.GetEntityFieldValue( internalNetManager, InternalNetManagerPacketRegistryField );

				return packetRegistry;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		public Dictionary<Type, ushort> GetRegisteredPacketTypes( )
		{
			try
			{
				Object packetRegistry = GetPacketRegistry( );
				Dictionary<Type, ushort> packetTypeIdMap = (Dictionary<Type, ushort>)BaseObject.GetEntityFieldValue( packetRegistry, PacketRegistryTypeIdMapField );

				return packetTypeIdMap;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new Dictionary<Type, ushort>( );
			}
		}

		protected static Delegate CreatePacketHandlerDelegate( PacketRegistrationType registrationType, Type packetType, MethodInfo handler )
		{
			try
			{
				Type delegateType = null;
				switch ( registrationType )
				{
					case PacketRegistrationType.Static:
						delegateType = typeof( ReceivePacketStatic<> );
						break;
					case PacketRegistrationType.Instance:
						delegateType = typeof( ReceivePacketInstance<> );
						break;
					case PacketRegistrationType.Timespan:
						delegateType = typeof( ReceivePacketTimespan<> );
						break;
					default:
						return null;
				}
				delegateType = delegateType.MakeGenericType( packetType );
				handler = handler.MakeGenericMethod( packetType );
				Delegate action = Delegate.CreateDelegate( delegateType, handler );
				return action;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected static void PreparePacketRegistrationMethod( )
		{
			try
			{
				Type masterNetManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( InternalNetManagerNamespace, InternalNetManagerClass );

				MethodInfo[ ] methods = masterNetManagerType.GetMethods( BindingFlags.Public | BindingFlags.Static );
				foreach ( MethodInfo method in methods )
				{
					if ( method.Name == "80EECA92933FCDB28F13F8A8A479BFBD" )	//Static
					{
						ParameterInfo[ ] parameters = method.GetParameters( );
						if ( parameters[ 0 ].ParameterType.Name == "BA9ED4CEE897B521F3D57A4EE3B3B8FC" )
						{
							m_registerPacketHandlerMethod = method;
						}
					}
					if ( method.Name == "43F78311C85C936231EC67195D5D2B73" )	//Instance
					{
						ParameterInfo[ ] parameters = method.GetParameters( );
						if ( parameters[ 0 ].ParameterType.Name == "22B36CE5809693930272FEC3EE3B9BA8" )
						{
							m_registerPacketHandlerMethod2 = method;
						}
					}
					if ( method.Name == "80EECA92933FCDB28F13F8A8A479BFBD" )	//Timespan
					{
						ParameterInfo[ ] parameters = method.GetParameters( );
						if ( parameters[ 0 ].ParameterType.Name == "54FCB5BECF9CB9F5346B87773CF643A6" )
						{
							m_registerPacketHandlerMethod3 = method;
						}
					}
				}
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		internal static bool RegisterCustomPacketHandler( PacketRegistrationType registrationType, Type packetType, MethodInfo handler, Type baseNetManagerType )
		{
			try
			{
				if ( m_registerPacketHandlerMethod == null )
					return false;
				if ( m_registerPacketHandlerMethod2 == null )
					return false;
				if ( m_registerPacketHandlerMethod3 == null )
					return false;
				if ( packetType == null )
					return false;
				if ( handler == null )
					return false;

				//Find the old packet handler
				Type masterNetManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( InternalNetManagerNamespace, InternalNetManagerClass );
				FieldInfo packetRegisteryHashSetField = masterNetManagerType.GetField( "9858E5CD512FFA5633683B9551FA4C30", BindingFlags.NonPublic | BindingFlags.Static );
				Object packetRegisteryHashSetRaw = packetRegisteryHashSetField.GetValue( null );
				HashSet<Object> packetRegisteryHashSet = UtilityFunctions.ConvertHashSet( packetRegisteryHashSetRaw );
				if ( packetRegisteryHashSet.Count == 0 )
					return false;
				Object matchedHandler = null;
				List<Object> matchedHandlerList = new List<object>( );
				List<Type> messageTypes = new List<Type>( );
				foreach ( var entry in packetRegisteryHashSet )
				{
					FieldInfo delegateField = entry.GetType( ).GetField( "C2AEC105AF9AB1EF82105555583139FC" );
					Type fieldType = delegateField.FieldType;
					Type[ ] genericArgs = fieldType.GetGenericArguments( );
					Type[ ] messageTypeArgs = genericArgs[ 1 ].GetGenericArguments( );
					Type messageType = messageTypeArgs[ 0 ];
					if ( messageType == packetType )
					{
						matchedHandler = entry;
						matchedHandlerList.Add( entry );
					}

					messageTypes.Add( messageType );
				}

				if ( matchedHandlerList.Count > 1 )
				{
					ApplicationLog.BaseLog.Warn( "Found more than 1 packet handler match for type '" + packetType.Name + "'" );
					return false;
				}

				if ( matchedHandler == null )
					return false;

				FieldInfo field = matchedHandler.GetType( ).GetField( "C2AEC105AF9AB1EF82105555583139FC", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				Object value = field.GetValue( matchedHandler );
				FieldInfo secondaryFlagsField = matchedHandler.GetType( ).GetField( "655022D3B2BE47EBCBA675CCE8B784AD", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				Object secondaryFlags = secondaryFlagsField.GetValue( matchedHandler );
				MulticastDelegate action = (MulticastDelegate)value;
				object target = action.Target;
				FieldInfo field2 = target.GetType( ).GetField( "F774FA5087F549F79181BD64E7B7FF30", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				Object value2 = field2.GetValue( target );
				MulticastDelegate action2 = (MulticastDelegate)value2;
				object target2 = action2.Target;

				string field3Name = "";
				string flagsFieldName = "";
				string serializerFieldName = "";
				switch ( registrationType )
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
				FieldInfo field3 = target2.GetType( ).GetField( field3Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				Object value3 = field3.GetValue( target2 );
				MulticastDelegate action3 = (MulticastDelegate)value3;

				FieldInfo flagsField = target2.GetType( ).GetField( flagsFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				Object flagsValue = flagsField.GetValue( target2 );
				FieldInfo serializerField = target2.GetType( ).GetField( serializerFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				Object serializerValue = serializerField.GetValue( target2 );

				FieldInfo methodBaseField = action3.GetType( ).GetField( "_methodBase", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				FieldInfo methodPtrField = action3.GetType( ).GetField( "_methodPtr", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
				FieldInfo methodPtrAuxField = action3.GetType( ).GetField( "_methodPtrAux", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );

				Delegate handlerAction = CreatePacketHandlerDelegate( registrationType, packetType, handler );

				//Remove the old handler from the registry
				MethodInfo removeMethod = packetRegisteryHashSetRaw.GetType( ).GetMethod( "Remove" );
				removeMethod.Invoke( packetRegisteryHashSetRaw, new object[ ] { matchedHandler } );

				//Update the handler delegate with our new method info
				methodBaseField.SetValue( action3, handlerAction.Method );
				methodPtrField.SetValue( action3, methodPtrField.GetValue( handlerAction ) );
				methodPtrAuxField.SetValue( action3, methodPtrAuxField.GetValue( handlerAction ) );

				if ( baseNetManagerType == null )
					baseNetManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( "", "48D79F8E3C8922F14D85F6D98237314C" );

				//Register the new packet handler
				MethodInfo registerMethod = null;
				switch ( registrationType )
				{
					case PacketRegistrationType.Static:
						registerMethod = m_registerPacketHandlerMethod.MakeGenericMethod( packetType );
						registerMethod.Invoke( null, new object[ ] { action3, flagsValue, secondaryFlags, serializerValue } );
						break;
					case PacketRegistrationType.Instance:
						registerMethod = m_registerPacketHandlerMethod2.MakeGenericMethod( baseNetManagerType, packetType );
						registerMethod.Invoke( null, new object[ ] { action3, flagsValue, secondaryFlags, serializerValue } );
						break;
					case PacketRegistrationType.Timespan:
						registerMethod = m_registerPacketHandlerMethod3.MakeGenericMethod( packetType );
						registerMethod.Invoke( null, new object[ ] { action3, flagsValue, secondaryFlags, serializerValue } );
						break;
					default:
						return false;
				}
				return true;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		internal delegate void ReceivePacketStatic<T>( ref T packet, Object netManager ) where T : struct;
		internal delegate void ReceivePacketInstance<T>( Object instanceNetManager, ref T packet, Object masterNetManager ) where T : struct;
		internal delegate void ReceivePacketTimespan<T>( ref T packet, Object netManager, TimeSpan time ) where T : struct;

		#endregion
	}
}