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
	public class DoorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool m_state;

		public static string DoorNamespace = "Sandbox.Game.Entities";
		public static string DoorClass = "MyDoor";

		public static string DoorGetStateMethod = "get_Open";
		public static string DoorSetStateMethod = "set_Open";
		public static string DoorBroadcastStateMethod = "SetOpenRequest";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public DoorEntity( CubeGridEntity parent, MyObjectBuilder_Door definition )
			: base( parent, definition )
		{
			m_state = definition.State;
		}

		public DoorEntity( CubeGridEntity parent, MyObjectBuilder_Door definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_state = definition.State;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Door" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Door ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Door door = (MyObjectBuilder_Door)base.ObjectBuilder;

				return door;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Door" )]
		[ReadOnly( true )]
		public float Opening
		{
			get { return ObjectBuilder.Opening; }
		}

		[DataMember]
		[Category( "Door" )]
		public bool State
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.State;

				return GetDoorState( );
			}
			set
			{
				if ( State == value ) return;
				m_state = value;
				ObjectBuilder.State = value;
				Changed = true;

				if ( BackingObject != null )
				{
					MySandboxGame.Static.Invoke( InternalUpdateDoor );
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( DoorNamespace, DoorClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for DoorEntity" );
				result &= Reflection.HasMethod( type, DoorGetStateMethod );
				result &= Reflection.HasMethod( type, DoorSetStateMethod );
				result &= Reflection.HasMethod( type, DoorBroadcastStateMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#region "Internal"

		protected bool GetDoorState( )
		{
			try
			{
				bool result = (bool)InvokeEntityMethod( ActualObject, DoorGetStateMethod );
				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return m_state;
			}
		}

		protected void InternalUpdateDoor( )
		{
			try
			{
				InvokeEntityMethod( ActualObject, DoorSetStateMethod, new object[ ] { m_state } );
				InvokeEntityMethod( ActualObject, DoorBroadcastStateMethod, new object[ ] { m_state } );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Internal"

		#endregion "Methods"
	}
}