using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using Sandbox.ModAPI;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "TerminalBlockEntityProxy" )]
	[KnownType( typeof( InventoryEntity ) )]
	public class TerminalBlockEntity : CubeBlockEntity
	{
		#region "Attributes"

		private string m_customName;

		public static string TerminalBlockNamespace = "";
		public static string TerminalBlockClass = "Sandbox.Game.Entities.Cube.MyTerminalBlock";

		public static string TerminalBlockGetCustomNameMethod = "get_CustomName";
		public static string TerminalBlockSetCustomNameMethod = "set_CustomName";

		//public static string TerminalBlockBroadcastCustomNameMethod = "FEBDC0DCFB7DA9823C08CE7FC0927638";
		public static string TerminalBlockBroadcastCustomNameMethod = "<.cctor>b__7";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public TerminalBlockEntity( CubeGridEntity parent, MyObjectBuilder_TerminalBlock definition )
			: base( parent, definition )
		{
			m_customName = definition.CustomName;
		}

		public TerminalBlockEntity( CubeGridEntity parent, MyObjectBuilder_TerminalBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			m_customName = definition.CustomName;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[DataMember]
		[Category( "Terminal Block" )]
		public override string Name
		{
			get
			{
				String name = CustomName;
				if ( name == null || name == "" )
					name = base.Name;
				return name;
			}
		}

		[IgnoreDataMember]
		[Category( "Terminal Block" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_TerminalBlock ObjectBuilder
		{
			get
			{
				try
				{
					if ( m_objectBuilder == null )
						m_objectBuilder = new MyObjectBuilder_TerminalBlock( );

					return (MyObjectBuilder_TerminalBlock)base.ObjectBuilder;
				}
				catch ( Exception )
				{
					return new MyObjectBuilder_TerminalBlock( );
				}
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Terminal Block" )]
		public string CustomName
		{
			get
			{
				if ( BackingObject == null || ActualObject == null )
					return ObjectBuilder.CustomName;

				return GetCustomName( );
			}
			set
			{
				if ( CustomName == value ) return;
				ObjectBuilder.CustomName = value;
				m_customName = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalSetCustomName;
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
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( TerminalBlockNamespace, TerminalBlockClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for TerminalBlockEntity" );
				bool result = true;
				result &= HasMethod( type, TerminalBlockGetCustomNameMethod );
				result &= HasMethod( type, TerminalBlockSetCustomNameMethod );
				result &= HasMethod( type, TerminalBlockBroadcastCustomNameMethod );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		public static void SetCustomName(Sandbox.ModAPI.Ingame.IMyTerminalBlock terminalBlock, string text)
		{
			try
			{
				StringBuilder newCustomName = new StringBuilder(text);
				InvokeStaticMethod(terminalBlock.GetType(), TerminalBlockBroadcastCustomNameMethod, new object[] { terminalBlock, newCustomName });
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole(string.Format("SetCustomName(): {0}", ex.ToString()));				
			}
		}

		protected string GetCustomName( )
		{
			Object rawObject = InvokeEntityMethod( ActualObject, TerminalBlockGetCustomNameMethod );
			if ( rawObject == null )
				return "";
			StringBuilder result = (StringBuilder)rawObject;
			return result.ToString( );
		}

		protected void InternalSetCustomName( )
		{
			try
			{
				StringBuilder newCustomName = new StringBuilder( m_customName );

				//InvokeEntityMethod(ActualObject, TerminalBlockSetCustomNameMethod, new object[] { newCustomName });
				InvokeStaticMethod( ActualObject.GetType( ), TerminalBlockBroadcastCustomNameMethod, new object[ ] { ActualObject, newCustomName } );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}