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

		//public static string PowerProducerGetMaxPowerOutputMethod = "9724CC520DB3D5DABA6DBB6B2EDC0D5A";
		//public static string PowerProducerGetCurrentOutputMethod = "81367E6B2991BC98D90F1862B8BDC1D7";
		//public static string PowerProducerSetCurrentOutputMethod = "6E5C384E82FF76BDAC76CE7A9A58333E";
		public static string PowerProducerGetMaxPowerOutputMethod = "E85C4222F3DA0EBD14B08CBFEEAD7E97";
		public static string PowerProducerGetCurrentOutputMethod = "B150B59E3540B701382DE785CC58C09C";
		public static string PowerProducerSetCurrentOutputMethod = "A295D69B03C5D384C6C3E1D9C1D2E805";

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