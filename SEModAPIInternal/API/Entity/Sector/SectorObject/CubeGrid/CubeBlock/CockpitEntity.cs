namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class CockpitEntity : ShipControllerEntity
	{
		#region "Attributes"

		private bool _weaponStatus;
		private CharacterEntity _pilot;

		public static string CockpitEntityNamespace = "";
		public static string CockpitEntityClass = "=a46pBNaH1dvgjWHyn2Az7HnXm0=";

		//public static string CockpitGetPilotEntityMethod = "C9A8295457C46F4DF5FC4DDBC7276287";
		//public static string CockpitGetPilotEntityMethod = "B0B4C9DD7231024CD14A50DB178582C8";
		public static string CockpitSetPilotEntityMethod = "AttachPilot";

		public static string CockpitGetPilotEntityField = "=0olbllUszKBbT2Web0qxywjatAD=";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public CockpitEntity( CubeGridEntity parent, MyObjectBuilder_Cockpit definition )
			: base( parent, definition )
		{
		}

		public CockpitEntity( CubeGridEntity parent, MyObjectBuilder_Cockpit definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[DataMember]
		[Category( "Cockpit" )]
		[ReadOnly( true )]
		public bool IsPassengerSeat
		{
			get
			{
				if ( ObjectBuilder.SubtypeName == "PassengerSeatLarge" )
					return true;

				if ( ObjectBuilder.SubtypeName == "PassengerSeatSmall" )
					return true;

				return false;
			}
		}

		[IgnoreDataMember]
		[Category( "Cockpit" )]
		[Browsable( false )]
		public CharacterEntity PilotEntity
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return null;

				Object backingPilot = GetPilotEntity( );
				if ( backingPilot == null )
					return null;

				if ( _pilot == null )
				{
					try
					{
						MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder( backingPilot );
						_pilot = new CharacterEntity( objectBuilder, backingPilot );
					}
					catch ( InvalidCastException ex )
					{
						ApplicationLog.BaseLog.Error( ex );
					}
				}

				if ( _pilot != null )
				{
					try
					{
						if ( _pilot.BackingObject != backingPilot )
						{
							MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder( backingPilot );
							_pilot.BackingObject = backingPilot;
							_pilot.ObjectBuilder = objectBuilder;
						}
					}
					catch ( InvalidCastException ex )
					{
						ApplicationLog.BaseLog.Error( ex );
					}
				}

				return _pilot;
			}
			set
			{
				_pilot = value;
				Changed = true;

				if ( BackingObject != null && ActualObject != null )
				{
					Action action = InternalUpdatePilotEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
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
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( CockpitEntityNamespace, CockpitEntityClass );
				if ( type == null )
					throw new TypeLoadException( "Could not find type for CockpitEntity" );

				//result &= BaseEntity.HasMethod(type, CockpitGetPilotEntityMethod);

				result &= HasMethod( type, CockpitSetPilotEntityMethod );
				result &= HasField( type, CockpitGetPilotEntityField );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected Object GetPilotEntity( )
		{
			//Object result = InvokeEntityMethod(ActualObject, CockpitGetPilotEntityMethod);
			Object result = GetEntityPropertyValue( ActualObject, CockpitGetPilotEntityField );
			return result;
		}

		protected void InternalUpdatePilotEntity( )
		{
			if ( _pilot == null || _pilot.BackingObject == null )
				return;

			InvokeEntityMethod( ActualObject, CockpitSetPilotEntityMethod, new [ ] { _pilot.BackingObject, Type.Missing, Type.Missing } );
		}

		public void FireWeapons( )
		{
			if ( _weaponStatus )
				return;

			_weaponStatus = true;

			Action action = InternalFireWeapons;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		public void StopWeapons( )
		{
			if ( !_weaponStatus )
				return;

			_weaponStatus = false;

			Action action = InternalStopWeapons;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		protected void InternalFireWeapons( )
		{
			//TODO - Patch 1.046 broke all of this. Find another method to call
		}

		protected void InternalStopWeapons( )
		{
			//TODO - Patch 1.046 broke all of this. Find another method to call
		}

		#endregion "Methods"
	}
}