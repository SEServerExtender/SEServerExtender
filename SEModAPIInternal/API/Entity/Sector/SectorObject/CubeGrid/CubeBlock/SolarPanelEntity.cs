namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.ModAPI.Ingame;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	[DataContract]
	public class SolarPanelEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private float m_maxPowerOutput;

		public static string SolarPanelNamespace = "Sandbox.Game.Entities.Blocks";
		public static string SolarPanelClass = "MySolarPanel";

		public static string SolarPanelSetMaxOutputMethod = "set_MaxPowerOutput";

		#endregion "Attributes"

		#region "Constructors and Intializers"

		public SolarPanelEntity( CubeGridEntity parent, MyObjectBuilder_SolarPanel definition )
			: base( parent, definition )
		{
		}

		public SolarPanelEntity( CubeGridEntity parent, MyObjectBuilder_SolarPanel definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Intializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Solar Panel" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_SolarPanel ObjectBuilder
		{
			get { return (MyObjectBuilder_SolarPanel)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( SolarPanelNamespace, SolarPanelClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for SolarPanelEntity" );
				result &= Reflection.HasMethod( type, SolarPanelSetMaxOutputMethod );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected void InternalUpdateMaxPowerOutput( )
		{
			InvokeEntityMethod( ActualObject, SolarPanelSetMaxOutputMethod, new object[ ] { m_maxPowerOutput } );
		}

		#endregion "Methods"
	}
}