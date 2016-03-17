using VRage.Game;
using VRage.Game.ModAPI;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	using System;
	using System.Collections.Generic;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Definitions;
	using Sandbox.ModAPI;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.Support;
	using VRage.Library.Utils;
	using VRageMath;

	public class CubeGridNetworkManager
	{
		//28 Packets
		public enum CubeGridPacketIds
		{
			CubeBlockHashSet = 14,				//..AAC558DB3CA968D0D3B965EA00DF05D4
			Packet1_2 = 15,
			Packet1_3 = 16,
			CubeBlockPositionList = 17,			//..5A55EA00576BB526436F3708D1F55455
			CubeBlockRemoveLists = 18,			//..94E4EFFF7257EEC85C3D8FA0F1EC9E69
			AllPowerStatus = 19,				//..782C8DC19A883BCB6A43C3006F456A2F

			//Construction/Item packets
			CubeBlockBuildIntegrityValues = 25,	//..EF2D90F50F1E378F0495FFB906D1C6C6

			CubeBlockItemList = 26,				//..3FD479635EACD6C3047ACB77CBAB645D
			Packet2_4 = 27,
			Packet2_5 = 28,
			Packet2_6 = 29,

			Packet3_1 = 4711,
			NewCubeBlock = 4712,				//..64F0E2C1B88DAB5903379AB2206F9A43
			Packet3_3 = 4713,
			Packet3_4 = 4714,

			ThrusterOverrideVector = 11212,		//..08CDB5B2B7DD39CF2E3D29D787045D83

			ThrusterGyroForceVectors = 15262,	//..632113536EC30663C6FF30251EFE637A
			Packet5_2 = 15263,
			Packet5_3 = 15264,
			CubeBlockOrientationIsh = 15265,	//..69FB43596400BF997D806DF041F2B54D
			CubeBlockFactionData = 15266,		//..090EFC311778552F418C0835D1248D60
			CubeBlockOwnershipMode = 15267,		//..F62F6360C3B7B7D32C525D5987F70A68

			AllPowerStatus2 = 15271,			//..903CC5CD740D130E90DB6CBF79F80F4F

			HandbrakeStatus = 15275,			//..4DCFFCEE8D5BA392C7A57ACD6470D7CD
			Packet7_1 = 15276,

			Packet8_1 = 15278,
			Packet8_2 = 15279,
			Packet8_3 = 15280,
		}

		#region "Attributes"

		private CubeGridEntity m_cubeGrid;
		private Object m_netManager;

		private static bool m_isRegistered;

		public static string CubeGridGetNetManagerMethod = "get_SyncObject";

		//Definition
		public static string CubeGridNetManagerNamespace = "Sandbox.Game.Multiplayer";

		public static string CubeGridNetManagerClass = "MySyncGrid";

		//Methods
		public static string CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod = "SendIntegrityChanged";

		public static string CubeGridNetManagerBroadcastCubeBlockFactionDataMethod = "ChangeOwnerRequest";
		public static string CubeGridNetManagerBroadcastCubeBlockRemoveListsMethod = "SendRemovedBlocks";
		public static string CubeGridNetManagerBroadcastAddCubeBlockMethod = "BuildBlocksSuccess";

		//Fields
		public static string CubeGridNetManagerCubeBlocksToDestroyField = "m_destroyBlockQueue";

		//////////////////////////////////////////////////////////////////

		public static string CubeGridIntegrityChangeEnumNamespace = string.Format( "{0}.{1}", CubeGridEntity.CubeGridNamespace, CubeGridEntity.CubeGridClass );
		public static string CubeGridIntegrityChangeEnumClass = "MyIntegrityChangeEnum";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CubeGridNetworkManager( CubeGridEntity cubeGrid )
		{
			m_cubeGrid = cubeGrid;
			object entity = m_cubeGrid.BackingObject;
			m_netManager = BaseObject.InvokeEntityMethod( entity, CubeGridGetNetManagerMethod );

			MySandboxGame.Static.Invoke( RegisterPacketHandlers );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CubeGridNetManagerNamespace, CubeGridNetManagerClass );
				return type;
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for CubeGridNetworkManager" );
				bool result = true;
				result &= Reflection.HasMethod( type, CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod );
				result &= Reflection.HasMethod( type, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod );
				result &= Reflection.HasMethod( type, CubeGridNetManagerBroadcastCubeBlockRemoveListsMethod );
				result &= Reflection.HasMethod( type, CubeGridNetManagerBroadcastAddCubeBlockMethod );
				result &= Reflection.HasField( type, CubeGridNetManagerCubeBlocksToDestroyField );

				Type type2 = CubeGridEntity.InternalType.GetNestedType( CubeGridIntegrityChangeEnumClass );
				if ( type2 == null )
					throw new Exception( "Could not find type for CubeGridNetworkManager-CubeGridIntegrityChangeEnum" );

				return result;
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void BroadcastCubeBlockFactionData( CubeBlockEntity cubeBlock )
		{
			try
			{
				BaseObject.InvokeEntityMethod( m_netManager, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod, new[ ] { m_cubeGrid.BackingObject, cubeBlock.ActualObject, cubeBlock.Owner, cubeBlock.ShareMode } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public static void BroadcastCubeBlockFactionData(IMyCubeBlock block, long owner, MyOwnershipShareModeEnum shareMode)
		{
			try
			{
				IMyCubeGrid parent = (IMyCubeGrid)block.Parent;
				object netManager = BaseObject.InvokeEntityMethod(parent, CubeGridGetNetManagerMethod);

				SandboxGameAssemblyWrapper.Instance.GameAction(() =>
				{
					BaseObject.InvokeEntityMethod(netManager, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod, new object[] { (object)parent, (object)block, owner, shareMode });
				});
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
			}
		}

		public static void BroadcastCubeBlockFactionData(List<IMyCubeBlock> blocks, long owner, MyOwnershipShareModeEnum shareMode)
		{
			try
			{
				SandboxGameAssemblyWrapper.Instance.GameAction(() =>
				{
					foreach (IMyCubeBlock block in blocks)
					{
						IMyCubeGrid parent = (IMyCubeGrid)block.Parent;
						object netManager = BaseObject.InvokeEntityMethod(parent, CubeGridGetNetManagerMethod);
						BaseObject.InvokeEntityMethod(netManager, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod, new object[] { (object)parent, (object)block, owner, shareMode });						
					}
				});
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
			}
		}

		public void BroadcastCubeBlockBuildIntegrityValues( CubeBlockEntity cubeBlock )
		{
			try
			{
				Type someEnum = CubeGridEntity.InternalType.GetNestedType( CubeGridIntegrityChangeEnumClass );
				Array someEnumValues = someEnum.GetEnumValues( );
				Object enumValue = someEnumValues.GetValue( 0 );
				BaseObject.InvokeEntityMethod( m_netManager, CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod, new[ ] { cubeBlock.BackingObject, enumValue, 0L } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public void BroadcastCubeBlockRemoveLists( )
		{
			BaseObject.InvokeEntityMethod( m_netManager, CubeGridNetManagerBroadcastCubeBlockRemoveListsMethod );
		}

		public void BroadcastAddCubeBlock( CubeBlockEntity cubeBlock )
		{
			try
			{
				Type packedStructType = CubeGridEntity.InternalType.GetNestedType( CubeGridEntity.CubeGridPackedCubeBlockClass );
				Object packedStruct = Activator.CreateInstance( packedStructType );
				MyCubeBlockDefinition def = MyDefinitionManager.Static.GetCubeBlockDefinition( cubeBlock.ObjectBuilder );

				//Set def id
				BaseObject.SetEntityFieldValue( packedStruct, "35E024D9E3B721592FB9B6FC1A1E239A", (DefinitionIdBlit)def.Id );

				//Set position
				BaseObject.SetEntityFieldValue( packedStruct, "5C3938C9B8CED1D0057CCF12F04329AB", cubeBlock.Position );

				//Set block size
				BaseObject.SetEntityFieldValue( packedStruct, "0DDB53EB9299ECC9826DF9A47E5E4F38", new Vector3UByte( def.Size ) );

				//Set block margins
				BaseObject.SetEntityFieldValue( packedStruct, "4045ED59A8C93DE0B41218EF2E947E55", new Vector3B( 0, 0, 0 ) );
				BaseObject.SetEntityFieldValue( packedStruct, "096897446D5BD5243D3D6E5C53CE1772", new Vector3B( 0, 0, 0 ) );

				//Set block margin scale
				BaseObject.SetEntityFieldValue( packedStruct, "E28B9725868E18B339D1E0594EF14444", new Vector3B( 0, 0, 0 ) );

				//Set orientation
				Quaternion rot;
				cubeBlock.BlockOrientation.GetQuaternion( out rot );
				BaseObject.SetEntityFieldValue( packedStruct, "F1AAFF5C8F200592F313BC7E02140A38", Base6Directions.GetForward( rot ) );
				BaseObject.SetEntityFieldValue( packedStruct, "E80AA7B84131E39F9F88209A109EED59", Base6Directions.GetUp( rot ) );

				//Set color
				BaseObject.SetEntityFieldValue( packedStruct, "556976F2528411FF5F95FC75DC13FEED", ColorExtensions.PackHSVToUint( cubeBlock.ColorMaskHSV ) );

				object[ ] parameters = {
					                       packedStruct,
					                       new HashSet<Vector3UByte>(),
					                       cubeBlock.EntityId,
					                       MyRandom.Instance.CreateRandomSeed()
				                       };
				BaseObject.InvokeEntityMethod( m_netManager, CubeGridNetManagerBroadcastAddCubeBlockMethod, parameters );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected static void RegisterPacketHandlers( )
		{
			try
			{
				if ( m_isRegistered )
					return;

				bool result = true;

				//Skip the overrides for now until we figure out more about client controlled position packets
				/*
				Type packetType = InternalType.GetNestedType("08CDB5B2B7DD39CF2E3D29D787045D83", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(CubeGridNetworkManager).GetMethod("ReceiveThrusterManagerVectorPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);
				Type packetType2 = InternalType.GetNestedType("632113536EC30663C6FF30251EFE637A", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method2 = typeof(CubeGridNetworkManager).GetMethod("ReceiveThrusterGyroForceVectorPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType2, method2, InternalType);
				*/

				if ( !result )
					return;

				m_isRegistered = true;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected static void ReceiveThrusterManagerVectorPacket<T>( Object instanceNetManager, ref T packet, Object masterNetManager ) where T : struct
		{
			try
			{
				//For now we ignore any inbound packets that set the positionorientation
				//This prevents the clients from having any control over the actual ship position
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected static void ReceiveThrusterGyroForceVectorPacket<T>( Object instanceNetManager, ref T packet, Object masterNetManager ) where T : struct
		{
			try
			{
				//For now we ignore any inbound packets that set the positionorientation
				//This prevents the clients from having any control over the actual ship position
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}