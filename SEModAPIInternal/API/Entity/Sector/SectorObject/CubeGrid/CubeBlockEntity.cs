using VRage.Game;
using VRage.Game.ModAPI;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Definitions;
	using Sandbox.ModAPI;
	using SEModAPI.API.TypeConverters;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;
	using VRage;
	using VRage.ObjectBuilders;
	using VRageMath;

	[DataContract( Name = "CubeBlockEntityProxy" )]
	[KnownType( "KnownTypes" )]
	public class CubeBlockEntity : BaseObject
	{
		#region "Attributes"

		private CubeGridEntity m_parent;
		private static Type m_internalType;
		private float m_buildPercent;
		private float m_integrityPercent;
		private long m_owner;
		private MyOwnershipShareModeEnum m_shareMode;

		public static string CubeBlockNamespace = "Sandbox.Game.Entities.Cube";
		public static string SlimBlockClass = "MySlimBlock";

		public static string CubeBlockGetObjectBuilderMethod = "GetObjectBuilder";
		public static string CubeBlockGetActualBlockMethod = "get_FatBlock";

		public static string CubeBlockDamageBlockMethod = "DoDamage";

		public static string CubeBlockGetBuildValueMethod = "get_BuildIntegrity";
		public static string CubeBlockGetBuildPercentMethod = "get_BuildLevelRatio";

		//public static string CubeBlockGetIntegrityValueMethod = "get_Integrity";
		public static string CubeBlockGetIntegrityValueMethod = "get_Integrity";

		public static string CubeBlockGetMaxIntegrityValueMethod = "get_MaxIntegrity";
		public static string CubeBlockUpdateWeldProgressMethod = "ApplyAccumulatedDamage";

		public static string CubeBlockGetBoneDamageMethod = "get_MaxDeformation";
		public static string CubeBlockFixBonesMethod = "FixBones";

		public static string CubeBlockParentCubeGridField = "m_cubeGrid";
		public static string CubeBlockColorMaskHSVField = "ColorMaskHSV";
		public static string CubeBlockConstructionManagerField = "m_componentStack";
		public static string CubeBlockCubeBlockDefinitionField = "BlockDefinition";

		/////////////////////////////////////////////////////

		public static string ActualCubeBlockNamespace = "";
		public static string ActualCubeBlockClass = "Sandbox.Game.Entities.MyCubeBlock";

		public static string ActualCubeBlockGetObjectBuilderMethod = "GetObjectBuilderCubeBlock";
		public static string ActualCubeBlockGetFactionsObjectMethod = "get_IDModule";
		public static string ActualCubeBlockSetFactionsDataMethod = "ChangeOwner";
		public static string ActualCubeBlockGetMatrixMethod = "get_WorldMatrix";

		public static string ActualCubeBlockGetOwnerMethod = "get_OwnerId";

		/////////////////////////////////////////////////////

		public static string FactionsDataNamespace = "Sandbox.Game.Entities";
		public static string FactionsDataClass = "MyIDModule";

		public static string FactionsDataOwnerField = "m_owner";
		public static string FactionsDataShareModeField = "ShareMode";

		/////////////////////////////////////////////////////

		public static string ConstructionManagerNamespace = "Sandbox.Game.Entities";
		public static string ConstructionManagerClass = "MyComponentStack";

		public static string ConstructionManagerSetIntegrityBuildValuesMethod = "SetIntegrity";
		public static string ConstructionManagerGetBuildValueMethod = "get_BuildIntegrity";
		public static string ConstructionManagerGetIntegrityValueMethod = "get_Integrity";
		public static string ConstructionManagerGetMaxIntegrityMethod = "get_MaxIntegrity";
		public static string ConstructionManagerGetBuildPercentMethod = "get_BuildRatio";
		public static string ConstructionManagerGetIntegrityPercentMethod = "get_IntegrityRatio";

		public static string ConstructionManagerIntegrityValueField = "m_integrity";
		public static string ConstructionManagerBuildValueField = "m_buildIntegrity";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CubeBlockEntity( CubeGridEntity parent, MyObjectBuilder_CubeBlock definition )
			: base( definition )
		{
			m_parent = parent;

			m_buildPercent = definition.BuildPercent;
			m_integrityPercent = definition.IntegrityPercent;
			m_owner = definition.Owner;
			m_shareMode = definition.ShareMode;
		}

		public CubeBlockEntity( CubeGridEntity parent, MyObjectBuilder_CubeBlock definition, Object backingObject )
			: base( definition, backingObject )
		{
			m_parent = parent;

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
			newEvent.type = EntityEventManager.EntityEventType.OnCubeBlockCreated;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			if ( m_parent.IsLoading )
			{
				newEvent.priority = 10;
			}
			else if ( EntityId != 0 )
			{
				newEvent.priority = 1;
			}
			else
			{
				newEvent.priority = 2;
			}
			EntityEventManager.Instance.AddEvent( newEvent );

			if ( EntityId != 0 )
			{
				GameEntityManager.AddEntity( EntityId, this );
			}

			m_buildPercent = definition.BuildPercent;
			m_integrityPercent = definition.IntegrityPercent;
			m_owner = definition.Owner;
			m_shareMode = definition.ShareMode;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal static Type InternalType
		{
			get
			{
				if ( m_internalType == null )
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CubeBlockNamespace, SlimBlockClass );
				return m_internalType;
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_CubeBlock ObjectBuilder
		{
			get
			{
				MyObjectBuilder_CubeBlock objectBuilder = (MyObjectBuilder_CubeBlock)base.ObjectBuilder;
				if ( objectBuilder == null )
				{
					objectBuilder = new MyObjectBuilder_CubeBlock( );
					ObjectBuilder = objectBuilder;
				}

				return objectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember( Order = 1 )]
		[Category( "Cube Block" )]
		[ReadOnly( true )]
		public override string Name
		{
			get
			{
				String name = Id.SubtypeName;
				if ( name == null || name == "" )
					name = Id.TypeId.ToString( );
				if ( name == null || name == "" )
					name = EntityId.ToString( );
				if ( name == null || name == "" )
					name = "Cube Block";
				return name;
			}
		}

		[DataMember( Order = 1 )]
		[Category( "Cube Block" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The unique entity ID representing a functional entity in-game" )]
		public long EntityId
		{
			get { return ObjectBuilder.EntityId; }
			set
			{
				if ( ObjectBuilder.EntityId == value ) return;
				ObjectBuilder.EntityId = value;

				Changed = true;
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[TypeConverter( typeof( Vector3ITypeConverter ) )]
		[Obsolete]
		public SerializableVector3I Min
		{
			get { return ObjectBuilder.Min; }
			set
			{
				if ( ObjectBuilder.Min.Equals( value ) ) return;
				ObjectBuilder.Min = value;
				Changed = true;
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		[ReadOnly( true )]
		[TypeConverter( typeof( Vector3ITypeConverter ) )]
		public Vector3I Position
		{
			get { return ObjectBuilder.Min; }
			set
			{
				if ( value.Equals( (Vector3I)ObjectBuilder.Min ) ) return;
				ObjectBuilder.Min = value;
				Changed = true;
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		[ReadOnly( true )]
		[TypeConverter( typeof( Vector3ITypeConverter ) )]
		public Vector3I Size
		{
			get
			{
				MyCubeBlockDefinition def = MyDefinitionManager.Static.GetCubeBlockDefinition( ObjectBuilder );
				return def.Size;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public MyBlockOrientation BlockOrientation
		{
			get { return ObjectBuilder.BlockOrientation; }
			set
			{
				if ( ObjectBuilder.BlockOrientation.Equals( value ) ) return;
				ObjectBuilder.BlockOrientation = value;
				Changed = true;
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper Up
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return Vector3.Zero;

				return GetBlockEntityMatrix( ).Up;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper Forward
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return Vector3.Zero;

				return GetBlockEntityMatrix( ).Forward;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		[TypeConverter( typeof( Vector3TypeConverter ) )]
		public Vector3Wrapper ColorMaskHSV
		{
			get { return ObjectBuilder.ColorMaskHSV; }
			set
			{
				if ( ObjectBuilder.ColorMaskHSV.Equals( value ) ) return;
				ObjectBuilder.ColorMaskHSV = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateColorMaskHSV );
				}
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		public float BuildPercent
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.BuildPercent;

				return InternalGetBuildPercent( );
			}
			set
			{
				if ( BuildPercent == value ) return;
				ObjectBuilder.BuildPercent = value;
				m_buildPercent = value;
				Changed = true;

				ObjectBuilder.IntegrityPercent = value;
				m_integrityPercent = value;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateConstructionManager );
				}
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		public float IntegrityPercent
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.IntegrityPercent;

				return InternalGetIntegrityPercent( );
			}
			set
			{
				if ( IntegrityPercent == value ) return;
				ObjectBuilder.IntegrityPercent = value;
				m_integrityPercent = value;
				Changed = true;

				ObjectBuilder.BuildPercent = value;
				m_buildPercent = value;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateConstructionManager );
				}
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		public long Owner
		{
			get
			{
				if ( BackingObject == null )
					return ObjectBuilder.Owner;

				return GetBlockOwner( );
			}
			set
			{
				if ( Owner == value ) return;
				ObjectBuilder.Owner = value;
				m_owner = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalSetOwnerShareMode );
				}
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		public MyOwnershipShareModeEnum ShareMode
		{
			get
			{
				if ( BackingObject == null )
					return ObjectBuilder.ShareMode;

				return GetBlockShareMode( );
			}
			set
			{
				if ( ShareMode == value ) return;
				ObjectBuilder.ShareMode = value;
				m_shareMode = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalSetOwnerShareMode );
				}
			}
		}

		[DataMember( Order = 2 )]
		[Category( "Cube Block" )]
		public float BoneDamage
		{
			get
			{
				if ( BackingObject == null )
					return 0f;

				return GetBoneDamage( );
			}
		}

		[IgnoreDataMember]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		public CubeGridEntity Parent
		{
			get { return m_parent; }
		}

		[IgnoreDataMember]
		[Category( "Cube Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal Object ActualObject
		{
			get { return GetActualObject( ); }
		}

		#endregion "Properties"

		#region "Methods"

		public static void SetBuildPercent( IMySlimBlock block, float percent )
		{
			try
			{
				// Set locally
				object constructionManager = GetEntityFieldValue( block, CubeBlockConstructionManagerField );
				float maxIntegrity = (float)InvokeEntityMethod( constructionManager, ConstructionManagerGetMaxIntegrityMethod );
				float integrity = percent * maxIntegrity;
				float build = percent * maxIntegrity;
				InvokeEntityMethod( constructionManager, ConstructionManagerSetIntegrityBuildValuesMethod, new object[ ] { build, integrity } );

				// Broadcast to players
				Type someEnum = CubeGridEntity.InternalType.GetNestedType( CubeGridNetworkManager.CubeGridIntegrityChangeEnumClass );
				Array someEnumValues = someEnum.GetEnumValues( );
				object enumValue = someEnumValues.GetValue( 0 );
				object netManager = InvokeEntityMethod( block.CubeGrid, CubeGridNetworkManager.CubeGridGetNetManagerMethod );
				InvokeEntityMethod( netManager, CubeGridNetworkManager.CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod, new object[ ] { block, enumValue, 0L } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( string.Format( "SetBuildPercent(): {0}", ex.ToString( ) ) );
			}

		}

		/*
		public void BroadcastCubeBlockBuildIntegrityValues(CubeBlockEntity cubeBlock)
		{
			try
			{
				Type someEnum = CubeGridEntity.InternalType.GetNestedType(CubeGridIntegrityChangeEnumClass);
				Array someEnumValues = someEnum.GetEnumValues();
				Object enumValue = someEnumValues.GetValue(0);
				BaseObject.InvokeEntityMethod(m_netManager, CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod, new object[] { cubeBlock.BackingObject, enumValue, 0L });
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
			}
		}
		*/
		public void FixBones( float a, float b )
		{
			try
			{
				SandboxGameAssemblyWrapper.Instance.GameAction( ( ) =>
				{
					InvokeEntityMethod( BackingObject, CubeBlockFixBonesMethod, new object[ ] { a, b } );
				} );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		public static List<Type> KnownTypes( )
		{
			return UtilityFunctions.GetCubeBlockTypes( );
		}

		public override void Dispose( )
		{
			m_isDisposed = true;

			Parent.DeleteCubeBlock( this );

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
			newEvent.type = EntityEventManager.EntityEventType.OnCubeBlockDeleted;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = (ushort)( ( EntityId != 0 ) ? 1 : 2 );
			EntityEventManager.Instance.AddEvent( newEvent );

			if ( EntityId != 0 )
			{
				GameEntityManager.RemoveEntity( EntityId );
			}

			base.Dispose( );
		}

		public override void Export( FileInfo fileInfo )
		{
			MyObjectBuilderSerializer.SerializeXML( fileInfo.FullName, false, ObjectBuilder );
		}

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for CubeBlockEntity" );
				bool result = true;

				result &= Reflection.HasMethod( type, CubeBlockGetObjectBuilderMethod );
				result &= Reflection.HasMethod( type, CubeBlockGetActualBlockMethod );
				result &= Reflection.HasMethod( type, CubeBlockDamageBlockMethod );
				result &= Reflection.HasMethod( type, CubeBlockGetBuildValueMethod );
				result &= Reflection.HasMethod( type, CubeBlockGetBuildPercentMethod );
				result &= Reflection.HasMethod( type, CubeBlockGetIntegrityValueMethod );
				result &= Reflection.HasMethod( type, CubeBlockGetMaxIntegrityValueMethod );
				result &= Reflection.HasMethod( type, CubeBlockUpdateWeldProgressMethod );

				result &= Reflection.HasField( type, CubeBlockParentCubeGridField );
				result &= Reflection.HasField( type, CubeBlockColorMaskHSVField );
				result &= Reflection.HasField( type, CubeBlockConstructionManagerField );
				result &= Reflection.HasField( type, CubeBlockCubeBlockDefinitionField );

				type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ActualCubeBlockNamespace, ActualCubeBlockClass );
				if ( type == null )
					throw new Exception( "Could not find actual type for CubeBlockEntity" );
				result &= Reflection.HasMethod( type, ActualCubeBlockGetObjectBuilderMethod );
				result &= Reflection.HasMethod( type, ActualCubeBlockGetFactionsObjectMethod );
				result &= Reflection.HasMethod( type, ActualCubeBlockSetFactionsDataMethod );
				result &= Reflection.HasMethod( type, ActualCubeBlockGetMatrixMethod );
				result &= Reflection.HasMethod( type, ActualCubeBlockGetOwnerMethod );

				type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( FactionsDataNamespace, FactionsDataClass );
				if ( type == null )
					throw new Exception( "Could not find factions data type for CubeBlockEntity" );
				result &= Reflection.HasField( type, FactionsDataOwnerField );
				result &= Reflection.HasField( type, FactionsDataShareModeField );

				type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ConstructionManagerNamespace, ConstructionManagerClass );
				if ( type == null )
					throw new Exception( "Could not find construction manager type for CubeBlockEntity" );
				result &= Reflection.HasMethod( type, ConstructionManagerSetIntegrityBuildValuesMethod );
				result &= Reflection.HasMethod( type, ConstructionManagerGetBuildValueMethod );
				result &= Reflection.HasMethod( type, ConstructionManagerGetIntegrityValueMethod );
				result &= Reflection.HasMethod( type, ConstructionManagerGetMaxIntegrityMethod );
				result &= Reflection.HasMethod( type, ConstructionManagerGetBuildPercentMethod );
				result &= Reflection.HasMethod( type, ConstructionManagerGetIntegrityPercentMethod );
				result &= Reflection.HasField( type, ConstructionManagerIntegrityValueField );
				result &= Reflection.HasField( type, ConstructionManagerBuildValueField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#region "Internal"

		internal static Object GetInternalParentCubeGrid( Object backingActualBlock )
		{
			if ( backingActualBlock == null )
				return null;

			return GetEntityFieldValue( backingActualBlock, CubeBlockParentCubeGridField );
		}

		internal Matrix GetBlockEntityMatrix( )
		{
			try
			{
				Matrix result = (Matrix)InvokeEntityMethod( ActualObject, ActualCubeBlockGetMatrixMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return new Matrix( );
			}
		}

		internal MyCubeBlockDefinition GetBlockDefinition( )
		{
			if ( BackingObject == null )
				return null;

			try
			{
				return (MyCubeBlockDefinition)GetEntityFieldValue( BackingObject, CubeBlockCubeBlockDefinitionField );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected Object GetActualObject( )
		{
			try
			{
				Object actualCubeObject = InvokeEntityMethod( BackingObject, CubeBlockGetActualBlockMethod );

				return actualCubeObject;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected Object GetFactionData( )
		{
			try
			{
				if ( ActualObject == null )
					return null;

				Object factionData = InvokeEntityMethod( ActualObject, ActualCubeBlockGetFactionsObjectMethod );
				return factionData;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return null;
			}
		}

		protected Object GetConstructionManager( )
		{
			return GetEntityFieldValue( BackingObject, CubeBlockConstructionManagerField );
		}

		protected long GetBlockOwner( )
		{
			try
			{
				if ( GetFactionData( ) == null )
					return 0;
			}
			catch { }

			Object rawResult = InvokeEntityMethod( ActualObject, ActualCubeBlockGetOwnerMethod );
			if ( rawResult == null )
				return 0;
			long result = (long)rawResult;
			return result;
		}

		protected MyOwnershipShareModeEnum GetBlockShareMode( )
		{
			Object factionData = null;
			try
			{
				factionData = GetFactionData( );
			}
			catch { }

			if ( factionData == null )
				return MyOwnershipShareModeEnum.None;

			Object rawResult = GetEntityFieldValue( factionData, FactionsDataShareModeField );
			if ( rawResult == null )
				return MyOwnershipShareModeEnum.None;
			MyOwnershipShareModeEnum result = (MyOwnershipShareModeEnum)rawResult;
			return result;
		}

		protected void InternalSetOwnerShareMode( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, ActualCubeBlockSetFactionsDataMethod, new object[ ] { m_owner, m_shareMode } );
				m_parent.NetworkManager.BroadcastCubeBlockFactionData( this );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected void InternalUpdateConstructionManager( )
		{
			try
			{
				//Update construction manager details
				Object constructionManager = GetConstructionManager( );
				float maxIntegrity = (float)InvokeEntityMethod( constructionManager, ConstructionManagerGetMaxIntegrityMethod );
				float integrity = m_integrityPercent * maxIntegrity;
				float build = m_buildPercent * maxIntegrity;

				InvokeEntityMethod( constructionManager, ConstructionManagerSetIntegrityBuildValuesMethod, new object[ ] { build, integrity } );

				Parent.NetworkManager.BroadcastCubeBlockBuildIntegrityValues( this );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		protected float InternalGetBuildPercent( )
		{
			try
			{
				float result = (float)InvokeEntityMethod( BackingObject, CubeBlockGetBuildPercentMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return 1;
			}
		}

		protected float InternalGetIntegrityPercent( )
		{
			try
			{
				float result = (float)InvokeEntityMethod( GetConstructionManager( ), ConstructionManagerGetIntegrityPercentMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return 1;
			}
		}

		protected void InternalUpdateColorMaskHSV( )
		{
			SetEntityFieldValue( BackingObject, CubeBlockColorMaskHSVField, (Vector3)ColorMaskHSV );
		}

		protected float GetBoneDamage( )
		{
			try
			{
				float result = (float)InvokeEntityMethod( BackingObject, CubeBlockGetBoneDamageMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return 0f;
			}
		}

		#endregion "Internal"

		/// <summary>
		/// Repairs this <see cref="CubeBlockEntity"/> (sets <see cref="BuildPercent"/> and <see cref="IntegrityPercent"/> to 1)
		/// </summary>
		public void Repair( )
		{
			if ( IntegrityPercent < 1f )
				IntegrityPercent = 1;
			if ( BuildPercent < 1f )
				BuildPercent = 1;
		}
		#endregion "Methods"
	}

	public class CubeBlockManager : BaseObjectManager
	{
		#region "Attributes"

		private CubeGridEntity m_parent;
		private bool m_isLoading;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CubeBlockManager( CubeGridEntity parent )
		{
			m_isLoading = true;
			m_parent = parent;
		}

		public CubeBlockManager( CubeGridEntity parent, Object backingSource, string backingSourceMethodName )
			: base( backingSource, backingSourceMethodName, InternalBackingType.Hashset )
		{
			m_isLoading = true;
			m_parent = parent;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public bool IsLoading
		{
			get
			{
				//Request a refresh if we are still loading
				if ( m_isLoading )
				{
					Refresh( );
				}

				return m_isLoading;
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion "Properties"

		#region "Methods"

		protected override bool IsValidEntity( Object entity )
		{
			try
			{
				if ( entity == null )
					return false;

				return true;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected override void LoadDynamic( )
		{
			try
			{
				HashSet<Object> rawEntities = GetBackingDataHashSet( );
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>( GetInternalData( ) );

				//Update the main data mapping
				foreach ( Object entity in rawEntities )
				{
					try
					{
						if ( !IsValidEntity( entity ) )
							continue;

						MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)CubeBlockEntity.InvokeEntityMethod( entity, CubeBlockEntity.CubeBlockGetObjectBuilderMethod );
						if ( baseEntity == null )
							continue;

						Vector3I cubePosition = baseEntity.Min;
						long packedBlockCoordinates = (long)cubePosition.X + (long)cubePosition.Y * 10000 + (long)cubePosition.Z * 100000000;

						//If the original data already contains an entry for this, skip creation
						if ( internalDataCopy.ContainsKey( packedBlockCoordinates ) )
						{
							CubeBlockEntity matchingCubeBlock = (CubeBlockEntity)GetEntry( packedBlockCoordinates );
							if ( matchingCubeBlock.IsDisposed )
								continue;

							matchingCubeBlock.BackingObject = entity;
							matchingCubeBlock.ObjectBuilder = baseEntity;
						}
						else
						{
							CubeBlockEntity newCubeBlock = null;

							if ( BlockRegistry.Instance.ContainsGameType( baseEntity.TypeId ) )
							{
								//Get the matching API type from the registry
								Type apiType = BlockRegistry.Instance.GetAPIType( baseEntity.TypeId );

								//Create a new API cube block
								newCubeBlock = (CubeBlockEntity)Activator.CreateInstance( apiType, new object[ ] { m_parent, baseEntity, entity } );
							}

							if ( newCubeBlock == null )
								newCubeBlock = new CubeBlockEntity( m_parent, baseEntity, entity );

							AddEntry( packedBlockCoordinates, newCubeBlock );
						}
					}
					catch ( Exception ex )
					{
						//ApplicationLog.BaseLog.Error( ex );
					}
				}

				//Cleanup old entities
				foreach ( KeyValuePair<long, BaseObject> entry in internalDataCopy )
				{
					try
					{
						if ( !rawEntities.Contains( entry.Value.BackingObject ) )
							DeleteEntry( entry.Value );
					}
					catch ( Exception ex )
					{
						//ApplicationLog.BaseLog.Error( ex );
					}
				}

				if ( GetInternalData( ).Count > 0 && m_isLoading )
				{
					//Trigger an event now that this cube grid has finished loading
					EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent( );
					newEvent.type = EntityEventManager.EntityEventType.OnCubeGridLoaded;
					newEvent.timestamp = DateTime.Now;
					newEvent.entity = this.m_parent;
					newEvent.priority = 1;
					EntityEventManager.Instance.AddEvent( newEvent );

					m_isLoading = false;
				}
			}
			catch ( Exception ex )
			{
				//ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}