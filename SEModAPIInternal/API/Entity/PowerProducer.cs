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

		//public static string PowerProducerGetMaxPowerOutputMethod = "EE14CE9A4A4EC31170A289F0445C3EFB";
		//public static string PowerProducerGetCurrentOutputMethod = "D18B16A57B4F96C7B988C7AF8B9939D0";
		//public static string PowerProducerSetCurrentOutputMethod = "AFD1A9ED1DE4C6C93DF986D446802DEF";
		//public static string PowerProducerGetMaxPowerOutputMethod = "EE14CE9A4A4EC31170A289F0445C3EFB";
		//public static string PowerProducerGetCurrentOutputMethod = "D18B16A57B4F96C7B988C7AF8B9939D0";
		//public static string PowerProducerSetCurrentOutputMethod = "AFD1A9ED1DE4C6C93DF986D446802DEF";

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
					//float result = (float)BaseObject.InvokeEntityMethod( m_powerProducer, PowerProducerGetMaxPowerOutputMethod );
					float result = 0f;
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
					//float result = (float)BaseObject.InvokeEntityMethod( m_powerProducer, PowerProducerGetCurrentOutputMethod );
					float result = 0f;
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

				//result &= BaseObject.HasMethod( type1, PowerProducerGetMaxPowerOutputMethod );
				//result &= BaseObject.HasMethod( type1, PowerProducerGetCurrentOutputMethod );
				//result &= BaseObject.HasMethod( type1, PowerProducerSetCurrentOutputMethod );

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
			//BaseObject.InvokeEntityMethod( m_powerProducer, PowerProducerSetCurrentOutputMethod, new object[ ] { m_powerOutput } );
		}

		#endregion "Methods"
	}
}