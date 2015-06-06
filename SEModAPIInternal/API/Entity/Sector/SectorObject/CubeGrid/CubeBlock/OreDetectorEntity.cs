namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class OreDetectorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool m_broadcastUsingAntennas;
		private float m_detectionRadius;

		public static string OreDetectorNamespace = "Sandbox.Game.Entities.Cube";
		public static string OreDetectorClass = "MyOreDetector";

		public static string OreDetectorGetUsingAntennasMethod = "get_BroadcastUsingAntennas";
		public static string OreDetectorSetUsingAntennasMethod = "set_BroadcastUsingAntennas";
		public static string OreDetectorGetDetectionRadiusMethod = "get_Range";
		public static string OreDetectorSetDetectionRadiusMethod = "set_Range";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public OreDetectorEntity( CubeGridEntity parent, MyObjectBuilder_OreDetector definition )
			: base( parent, definition )
		{
			m_broadcastUsingAntennas = definition.BroadcastUsingAntennas;
			m_detectionRadius = definition.DetectionRadius;
		}

		public OreDetectorEntity( CubeGridEntity parent, MyObjectBuilder_OreDetector definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_broadcastUsingAntennas = definition.BroadcastUsingAntennas;
			m_detectionRadius = definition.DetectionRadius;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Ore Detector" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_OreDetector ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_OreDetector)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Ore Detector" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( OreDetectorNamespace, OreDetectorClass );
				return type;
			}
		}

		[DataMember]
		[Category( "Ore Detector" )]
		public bool BroadcastUsingAntennas
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.BroadcastUsingAntennas;

				return GetUsingAntennas( );
			}
			set
			{
				if ( BroadcastUsingAntennas == value ) return;
				ObjectBuilder.BroadcastUsingAntennas = value;
				m_broadcastUsingAntennas = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateUsingAntennas );
				}
			}
		}

		[DataMember]
		[Category( "Ore Detector" )]
		public float DetectionRadius
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.DetectionRadius;

				return GetDetectionRadius( );
			}
			set
			{
				if ( DetectionRadius == value ) return;
				ObjectBuilder.DetectionRadius = value;
				m_detectionRadius = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateDetectionRadius );
				}
			}
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for OreDetectorEntity" );

				result &= Reflection.HasMethod( type, OreDetectorGetUsingAntennasMethod );
				result &= Reflection.HasMethod( type, OreDetectorSetUsingAntennasMethod );
				result &= Reflection.HasMethod( type, OreDetectorGetDetectionRadiusMethod );
				result &= Reflection.HasMethod( type, OreDetectorSetDetectionRadiusMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected bool GetUsingAntennas( )
		{
			Object rawResult = InvokeEntityMethod( ActualObject, OreDetectorGetUsingAntennasMethod );
			if ( rawResult == null )
				return false;
			bool result = (bool)rawResult;
			return result;
		}

		protected void InternalUpdateUsingAntennas( )
		{
			InvokeEntityMethod( ActualObject, OreDetectorSetUsingAntennasMethod, new object[ ] { m_broadcastUsingAntennas } );
		}

		protected float GetDetectionRadius( )
		{
			Object rawResult = InvokeEntityMethod( ActualObject, OreDetectorGetDetectionRadiusMethod );
			if ( rawResult == null )
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void InternalUpdateDetectionRadius( )
		{
			InvokeEntityMethod( ActualObject, OreDetectorSetDetectionRadiusMethod, new object[ ] { m_detectionRadius } );
		}

		#endregion "Methods"
	}
}