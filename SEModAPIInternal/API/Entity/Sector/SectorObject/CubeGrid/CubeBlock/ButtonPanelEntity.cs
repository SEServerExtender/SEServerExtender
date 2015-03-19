using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.API.Common;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "ButtonPanelEntityProxy" )]
	public class ButtonPanelEntity : ShipControllerEntity
	{
		#region "Attributes"

		private long m_currentPlayerId;

		public static string ButtonPanelNamespace = "";
		public static string ButtonPanelClass = "=51h1yWMOSorv3CAc61MO8nFJJ0=";

		//public static string ButtonPanelGetCurrentPlayerIdMethod = "";
		//public static string ButtonPanelSetCurrentPlayerIdMethod = "";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public ButtonPanelEntity( CubeGridEntity parent, MyObjectBuilder_RemoteControl definition )
			: base( parent, definition )
		{
			//			m_currentPlayerId = definition.CurrentPlayerId.GetValueOrDefault(0);
		}

		public ButtonPanelEntity( CubeGridEntity parent, MyObjectBuilder_RemoteControl definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
			//			m_currentPlayerId = definition.CurrentPlayerId.GetValueOrDefault(0);
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Button Panel" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_RemoteControl ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_RemoteControl)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category( "Button Panel" )]
		[Browsable( false )]
		[ReadOnly( true )]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ButtonPanelNamespace, ButtonPanelClass );
				return type;
			}
		}

		/*
		[DataMember]
		[Category("Button Panel")]
		public long CurrentPlayerId
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.CurrentPlayerId.GetValueOrDefault(0);

				return GetCurrentPlayerId();
			}
			set
			{
				if (CurrentPlayerId == value) return;
				ObjectBuilder.CurrentPlayerId = value;
				m_currentPlayerId = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateCurrentPlayerId;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}
		*/

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for ButtonPanelEntity" );

				//result &= HasMethod(type, ButtonPanelGetCurrentPlayerIdMethod);
				//result &= HasMethod(type, ButtonPanelSetCurrentPlayerIdMethod);

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		/*
		protected long GetCurrentPlayerId()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, ButtonPanelGetCurrentPlayerIdMethod);
			if (rawResult == null)
				return 0;
			long result = (long)rawResult;
			return result;
		}

		protected void InternalUpdateCurrentPlayerId()
		{
			InvokeEntityMethod(ActualObject, ButtonPanelSetCurrentPlayerIdMethod, new object[] { m_currentPlayerId });
		}
		*/

		#endregion "Methods"
	}
}