namespace SEModAPIInternal.API.Common
{
	using System;
	using System.Runtime.Serialization;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;

	[DataContract]
	public class PowerManager
	{
		#region "Attributes"

		protected Object m_powerManager;

		public static string PowerManagerNamespace = "Sandbox.Game.GameSystems.Electricity";
		public static string PowerManagerClass = "MyPowerDistributor";

		public static string PowerManagerRegisterPowerReceiverMethod = "AddConsumer";
		public static string PowerManagerUnregisterPowerReceiverMethod = "RemoveConsumer";
		public static string PowerManagerRegisterPowerProducerMethod = "AddProducer";
		public static string PowerManagerUnregisterPowerProducerMethod = "RemoveProducer";
		public static string PowerManagerGetAvailablePowerMethod = "get_TotalRequiredInput";
		public static string PowerManagerGetUsedPowerPercentMethod = "get_RemainingFuelTime";

		public static string PowerManagerPowerReceiverHashSetField = "m_consumersByPriority";
		public static string PowerManagerPowerProducerHashSetField = "m_producersByPriority";
		public static string PowerManagerMaxAvailablePowerField = "m_maxAvailablePower";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public PowerManager( Object powerManager )
		{
			m_powerManager = powerManager;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public float TotalPower
		{
			get
			{
				try
				{
					return m_powerManager != null ? (float) BaseObject.GetEntityFieldValue( m_powerManager, PowerManagerMaxAvailablePowerField ) : 0;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
		}

		public float AvailablePower
		{
			get
			{
				try
				{
					return m_powerManager != null ? TotalPower - (float) BaseObject.InvokeEntityMethod( m_powerManager, PowerManagerGetAvailablePowerMethod ) : 0;
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PowerManagerNamespace, PowerManagerClass );
				if ( type1 == null )
					throw new Exception( "Could not find internal type for PowerManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, PowerManagerRegisterPowerReceiverMethod );
				result &= BaseObject.HasMethod( type1, PowerManagerUnregisterPowerReceiverMethod );
				result &= BaseObject.HasMethod( type1, PowerManagerRegisterPowerProducerMethod );
				result &= BaseObject.HasMethod( type1, PowerManagerUnregisterPowerProducerMethod );
				result &= BaseObject.HasMethod( type1, PowerManagerGetAvailablePowerMethod );
				result &= BaseObject.HasMethod( type1, PowerManagerGetUsedPowerPercentMethod );
				result &= BaseObject.HasField( type1, PowerManagerPowerReceiverHashSetField );
				result &= BaseObject.HasField( type1, PowerManagerPowerProducerHashSetField );
				result &= BaseObject.HasField( type1, PowerManagerMaxAvailablePowerField );

				return result;
			}
			catch ( Exception ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		public void RegisterPowerReceiver( Object receiver )
		{
			BaseObject.InvokeEntityMethod( m_powerManager, PowerManagerRegisterPowerReceiverMethod, new[ ] { receiver } );
		}

		public void UnregisterPowerReceiver( Object receiver )
		{
			BaseObject.InvokeEntityMethod( m_powerManager, PowerManagerUnregisterPowerReceiverMethod, new[ ] { receiver } );
		}

		public void RegisterPowerProducer( Object producer )
		{
			BaseObject.InvokeEntityMethod( m_powerManager, PowerManagerRegisterPowerProducerMethod, new[ ] { producer } );
		}

		public void UnregisterPowerProducer( Object producer )
		{
			BaseObject.InvokeEntityMethod( m_powerManager, PowerManagerUnregisterPowerProducerMethod, new[ ] { producer } );
		}

		#endregion "Methods"
	}
}