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
	public class CameraBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool m_isActive;

		public static string CameraBlockNamespace = "Sandbox.Game.Entities";
		public static string CameraBlockClass = "MyCameraBlock";

		public static string CameraBlockGetIsActiveMethod = "get_IsActive";
		public static string CameraBlockSetIsActiveMethod = "set_IsActive";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CameraBlockEntity( CubeGridEntity parent, MyObjectBuilder_CameraBlock definition )
			: base( parent, definition )
		{
			m_isActive = definition.IsActive;
		}

		public CameraBlockEntity( CubeGridEntity parent, MyObjectBuilder_CameraBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_isActive = definition.IsActive;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Camera" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_CameraBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_CameraBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Camera" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CameraBlockNamespace, CameraBlockClass );
				return type;
			}
		}

		[DataMember]
		[Category( "Camera" )]
		public bool IsActive
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.IsActive;

				return GetIsCameraActive( );
			}
			set
			{
				if ( IsActive == value ) return;
				ObjectBuilder.IsActive = value;
				m_isActive = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateCameraIsActive );
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
					throw new Exception( "Could not find internal type for CameraBlockEntity" );

				result &= Reflection.HasMethod( type, CameraBlockGetIsActiveMethod );
				result &= Reflection.HasMethod( type, CameraBlockSetIsActiveMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected bool GetIsCameraActive( )
		{
			Object rawResult = InvokeEntityMethod( ActualObject, CameraBlockGetIsActiveMethod );
			if ( rawResult == null )
				return false;
			bool result = (bool)rawResult;
			return result;
		}

		protected void InternalUpdateCameraIsActive( )
		{
			InvokeEntityMethod( ActualObject, CameraBlockSetIsActiveMethod, new object[ ] { m_isActive } );
		}

		#endregion "Methods"
	}
}