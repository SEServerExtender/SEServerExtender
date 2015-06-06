namespace SEModAPIInternal.API.Entity
{
	using System;
	using System.ComponentModel;
	using Sandbox;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class PowerReceiver
	{
		#region "Attributes"

		private Object m_parent;
		private PowerManager m_powerManager;
		private Object m_powerReceiver;
		private float m_maxRequiredInput;
		private Func<float> m_powerRateCallback;

		//Power Receiver Type
		public static string PowerReceiverNamespace = "Sandbox.Game.GameSystems.Electricity";

		public static string PowerReceiverClass = "MyPowerReceiver";

		//Power Receiver Methods
		public static string PowerReceiverRunPowerRateCallbackMethod = "Update";

		public static string PowerReceiverGetCurrentInputMethod = "get_RequiredInput";
		public static string PowerReceiverGetCurrentRateMethod = "get_CurrentInput";
		public static string PowerReceiverSetMaxRequiredInputMethod = "set_RequiredInput";

		//Power Receiver Fields
		public static string PowerReceiverMaxRequiredInputField = "MaxRequiredInput";

		public static string PowerReceiverPowerRatioField = "<SuppliedRatio>k__BackingField";
		public static string PowerReceiverInputRateCallbackField = "m_requiredInputFunc";

		////////////////////////////////////////////////////////////////////

		//3 - Door, 4 - Gravity Generator, 5 - Battery, 8 - BatteryBlock
		public static string PowerReceiverTypeEnumNamespace = "Sandbox.Game.GameSystems.Electricity";

		public static string PowerReceiverTypeEnumClass = "MyConsumerGroupEnum";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public PowerReceiver( Object parent, PowerManager powerManager, Object powerReceiver )
		{
			m_parent = parent;
			m_powerManager = powerManager;
			m_powerReceiver = powerReceiver;
			m_powerRateCallback = null;

			m_maxRequiredInput = 0;
		}

		public PowerReceiver( Object parent, PowerManager powerManager, Object powerReceiver, Func<float> powerRateCallback )
		{
			m_parent = parent;
			m_powerManager = powerManager;
			m_powerReceiver = powerReceiver;
			m_powerRateCallback = powerRateCallback;

			m_maxRequiredInput = 0;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[Browsable( false )]
		[ReadOnly( true )]
		public Object BackingObject
		{
			get { return m_powerReceiver; }
		}

		public float MaxRequiredInput
		{
			get { return m_maxRequiredInput; }
			set
			{
				if ( BackingObject == null || m_powerRateCallback == null )
					return;
				m_maxRequiredInput = value;

				MySandboxGame.Static.Invoke( InternalUpdateMaxRequiredInput );
			}
		}

		public float CurrentInput
		{
			get
			{
				if ( BackingObject == null )
					return 0;

				float currentInput = (float)BaseObject.InvokeEntityMethod( BackingObject, PowerReceiverGetCurrentInputMethod );

				return currentInput;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PowerReceiverNamespace, PowerReceiverClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for PowerReceiver" );
				bool result = true;
				result &= Reflection.HasMethod( type1, PowerReceiverRunPowerRateCallbackMethod );
				result &= Reflection.HasMethod( type1, PowerReceiverGetCurrentInputMethod );
				result &= Reflection.HasMethod( type1, PowerReceiverGetCurrentRateMethod );
				result &= Reflection.HasMethod( type1, PowerReceiverSetMaxRequiredInputMethod );
				result &= Reflection.HasField( type1, PowerReceiverMaxRequiredInputField );
				result &= Reflection.HasField( type1, PowerReceiverPowerRatioField );
				result &= Reflection.HasField( type1, PowerReceiverInputRateCallbackField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected void InternalUpdateMaxRequiredInput( )
		{
			try
			{
				if ( m_powerRateCallback == null )
					return;

				BaseObject.SetEntityFieldValue( BackingObject, PowerReceiverInputRateCallbackField, m_powerRateCallback );

				BaseObject.SetEntityFieldValue( BackingObject, PowerReceiverMaxRequiredInputField, m_maxRequiredInput );

				BaseObject.InvokeEntityMethod( BackingObject, PowerReceiverSetMaxRequiredInputMethod, new object[ ] { m_maxRequiredInput } );

				m_powerManager.UnregisterPowerReceiver( m_parent );
				m_powerManager.RegisterPowerReceiver( m_parent );
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
			}
		}

		#endregion "Methods"
	}
}