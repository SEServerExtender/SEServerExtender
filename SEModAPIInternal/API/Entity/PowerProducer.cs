namespace SEModAPIInternal.API.Entity
{
	using System;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class PowerProducer
	{
		#region "Attributes"

		private PowerManager m_parent;
		private Object _powerProducer;

		public static string PowerProducerNamespace = "Sandbox.Game.GameSystems.Electricity";
		public static string PowerProducerInterface = "IMyPowerProducer";

				public static string PowerProducerGetMaxPowerOutputMethod = "get_MaxPowerOutput";
		public static string PowerProducerGetCurrentOutputMethod = "get_CurrentPowerOutput";
		public static string PowerProducerSetCurrentOutputMethod = "set_CurrentPowerOutput";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public PowerProducer( PowerManager parent, Object powerProducer )
		{
			m_parent = parent;
			_powerProducer = powerProducer;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public float MaxPowerOutput
		{
			get
			{
				try
				{
					return _powerProducer == null ? 0 : (float) BaseObject.InvokeEntityMethod( _powerProducer, PowerProducerGetMaxPowerOutputMethod );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
		}

		public float PowerOutput
		{
			get
			{
				try
				{
					return _powerProducer == null ? 0 : (float) BaseObject.InvokeEntityMethod( _powerProducer, PowerProducerGetCurrentOutputMethod );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
			set
			{
				Action<float> action = InternalUpdatePowerOutput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action, value );
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PowerProducerNamespace, PowerProducerInterface );
				if ( type1 == null )
					throw new TypeLoadException( "Could not find internal type for PowerProducer" );
				bool result = true;

				result &= BaseObject.HasMethod( type1, PowerProducerGetMaxPowerOutputMethod );
				result &= BaseObject.HasMethod( type1, PowerProducerGetCurrentOutputMethod );
				result &= BaseObject.HasMethod( type1, PowerProducerSetCurrentOutputMethod );

				return result;
			}
			catch ( TypeLoadException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		protected void InternalUpdatePowerOutput( float powerOutput )
		{
			BaseObject.InvokeEntityMethod( _powerProducer, PowerProducerSetCurrentOutputMethod, new object[ ] { powerOutput } );
		}

		#endregion "Methods"
	}
}