using System;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class PowerProducer
	{
		#region "Attributes"

		private PowerManager m_parent;
		private Object m_powerProducer;

		protected float m_maxPowerOutput;
		protected float m_powerOutput;

		public static string PowerProducerNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerProducerClass = "7E69388ED0DB47818FB7AFF9F16C6EDA";

		//public static string PowerProducerGetMaxPowerOutputMethod = "81F664869DE51419C06DC2FC3D933666";
		//public static string PowerProducerGetCurrentOutputMethod = "235CDCDAB7EDE2C811A6FE4CDA064E97";
		//public static string PowerProducerSetCurrentOutputMethod = "5ACADCA968781CC19FE44053D361D2C2";
		public static string PowerProducerGetMaxPowerOutputMethod = "1D81A79E5ECEDAE00B52CA5534D810AD";
		public static string PowerProducerGetCurrentOutputMethod = "766589F5547B1FC04CA0061236F0DADF";
		public static string PowerProducerSetCurrentOutputMethod = "CAB2FAB3F32F120534BFCF2ADD9B2510";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public PowerProducer( PowerManager parent, Object powerProducer )
		{
			m_parent = parent;
			m_powerProducer = powerProducer;

			m_maxPowerOutput = 0;
			m_powerOutput = 0;

			m_maxPowerOutput = MaxPowerOutput;
			m_powerOutput = PowerOutput;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public float MaxPowerOutput
		{
			get
			{
				if ( m_powerProducer == null )
					return m_maxPowerOutput;

				try
				{
					float result = (float)BaseObject.InvokeEntityMethod( m_powerProducer, PowerProducerGetMaxPowerOutputMethod );
					return result;
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
					return m_maxPowerOutput;
				}
			}
		}

		public float PowerOutput
		{
			get
			{
				if ( m_powerProducer == null )
					return m_powerOutput;

				try
				{
					float result = (float)BaseObject.InvokeEntityMethod( m_powerProducer, PowerProducerGetCurrentOutputMethod );
					return result;
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
					return m_powerOutput;
				}
			}
			set
			{
				m_powerOutput = value;

				Action action = InternalUpdatePowerOutput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
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
					throw new Exception( "Could not find internal type for PowerProducer" );
				bool result = true;
				result &= BaseObject.HasMethod( type1, PowerProducerGetMaxPowerOutputMethod );
				result &= BaseObject.HasMethod( type1, PowerProducerGetCurrentOutputMethod );
				result &= BaseObject.HasMethod( type1, PowerProducerSetCurrentOutputMethod );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		protected void InternalUpdatePowerOutput( )
		{
			BaseObject.InvokeEntityMethod( m_powerProducer, PowerProducerSetCurrentOutputMethod, new object[ ] { m_powerOutput } );
		}

		#endregion "Methods"
	}
}