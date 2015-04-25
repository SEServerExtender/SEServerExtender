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

		public static string PowerProducerNamespace = "";
		public static string PowerProducerClass = "=H36sAJ3q2dwiHOAJoDFIiSAhzB=";

		

		//public static string PowerProducerGetMaxPowerOutputMethod = "2F4566F5785A938FCDE93C36C71066C8";		//1.73.09  float 2F4566F5785A938FCDE93C36C71066C8();  Second function of this type
		//public static string PowerProducerGetCurrentOutputMethod = "0DE7BC75E34014AA5A2C722A6C6B048E";		//1.73.09  float 0DE7BC75E34014AA5A2C722A6C6B048E();  First function of this type
		//public static string PowerProducerSetCurrentOutputMethod = "8A64E0D6467536E75388E502F1CCB920";		//1.73.09  void 8A64E0D6467536E75388E502F1CCB920(float 094E467FF825343C63A4502A66537B82);
		public static string PowerProducerGetMaxPowerOutputMethod = "get_MaxPowerOutput";						//1.74.10
		public static string PowerProducerGetCurrentOutputMethod = "get_CurrentPowerOutput";					//1.74.10
		public static string PowerProducerSetCurrentOutputMethod = "set_CurrentPowerOutput";					//1.74.10

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
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( PowerProducerNamespace, PowerProducerClass );
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