namespace SEModAPIInternal.API.Entity
{
	using System;
	using Sandbox.ModAPI.Ingame;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class PowerProducer : IMyPowerProducer
	{
		#region "Attributes"

		private PowerManager m_parent;
		private Object _powerProducer;

		public static string PowerProducerNamespace = "Sandbox.Game.GameSystems.Electricity";
		public static string PowerProducerInterface = "IMyPowerProducer";

		public static string PowerProducerGetMaxPowerOutputMethod = "get_MaxPowerOutput";
		public static string PowerProducerGetDefinedPowerOutputMethod = "get_DefinedPowerOutput";
		public static string PowerProducerCurrentOutputProperty = "CurrentPowerOutput";
		public const string PowerProducerRemainingCapacityProperty = "RemainingCapacity";
		public const string PowerProducerHasCapacityRemainingProperty = "HasCapacityRemaining";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public PowerProducer( PowerManager parent, Object powerProducer )
		{
			m_parent = parent;
			_powerProducer = powerProducer;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public float CurrentPowerOutput { get { return PowerOutput; } }

		public float MaxPowerOutput
		{
			get
			{
				try
				{
					return _powerProducer == null ? 0 : (float)BaseObject.InvokeEntityMethod( _powerProducer, PowerProducerGetMaxPowerOutputMethod );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
		}

		public float DefinedPowerOutput
		{
			get
			{
				try
				{
					return _powerProducer == null ? 0 : (float)BaseObject.InvokeEntityMethod( _powerProducer, PowerProducerGetDefinedPowerOutputMethod );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
			private set { }
		}

		public bool ProductionEnabled
		{
			get { return (bool) BaseObject.InvokeEntityMethod( _powerProducer, "get_ProductionEnabled" ); }
		}

		public float PowerOutput
		{
			get
			{
				try
				{
					return _powerProducer == null ? 0 : (float)BaseObject.GetEntityPropertyValue( _powerProducer, PowerProducerCurrentOutputProperty );
				}
				catch ( Exception ex )
				{
					ApplicationLog.BaseLog.Error( ex );
					return 0;
				}
			}
			set { BaseObject.SetEntityPropertyValue( _powerProducer, PowerProducerCurrentOutputProperty, value ); }
		}

		public bool HasCapacityRemaining { get { return _powerProducer != null && (bool)BaseObject.GetEntityPropertyValue( _powerProducer, PowerProducerHasCapacityRemainingProperty ); } }
		public float RemainingCapacity { get { return _powerProducer == null ? 0 : (float)BaseObject.GetEntityPropertyValue( _powerProducer, PowerProducerRemainingCapacityProperty ); } }

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

				if ( !BaseObject.HasProperty( type1, PowerProducerCurrentOutputProperty ) )
				{
					result = false;
					ApplicationLog.BaseLog.Warn( "PowerProducer does not have property {0}", PowerProducerCurrentOutputProperty );
				}
				if ( !BaseObject.HasProperty( type1, PowerProducerHasCapacityRemainingProperty ) )
				{
					result = false;
					ApplicationLog.BaseLog.Warn( "PowerProducer does not have property {0}", PowerProducerHasCapacityRemainingProperty );
				}
				if ( !BaseObject.HasProperty( type1, PowerProducerRemainingCapacityProperty ) )
				{
					result = false;
					ApplicationLog.BaseLog.Warn( "PowerProducer does not have property {0}", PowerProducerRemainingCapacityProperty );
				}
				return result;
			}
			catch ( TypeLoadException ex )
			{
				ApplicationLog.BaseLog.Error( ex );
				return false;
			}
		}

		#endregion "Methods"
	}
}