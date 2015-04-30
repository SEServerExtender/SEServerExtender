using System;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.Support;

	public class AirVentEntity : TerminalBlockEntity
	{
		public static string AirVentNamespace = "Sandbox.Game.Entities.Blocks";
		public static string AirVentClass = "MyAirVent";

		public static string AirVentGetOxygenLevelMethod = "GetOxygenLevel";
		public static string AirVentIsPressurizedMethod = "IsPressurized";


		public AirVentEntity( CubeGridEntity parent, MyObjectBuilder_AirVent definition )
			: base( parent, definition )
		{
			
		}

		public AirVentEntity( CubeGridEntity parent, MyObjectBuilder_AirVent definition, object backingObject )
			: base( parent, definition, backingObject )
		{
			
		}

		[IgnoreDataMember]
		[Category( "Air Vent" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_AirVent ObjectBuilder
		{
			get
			{
				MyObjectBuilder_AirVent objectBuilder = (MyObjectBuilder_AirVent)base.ObjectBuilder;
				if ( objectBuilder == null )
				{
					objectBuilder = new MyObjectBuilder_AirVent( );
					ObjectBuilder = objectBuilder;
				}

				return objectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		/// <exception cref="System.Exception">Any exception is thrown when trying to invoke the GetOxygenLevel method.</exception>
		public float GetOxygenLevel( )
		{
			try
			{
				object airVent = GetActualObject( );
				return (float)InvokeEntityMethod( airVent, AirVentGetOxygenLevelMethod );
			}
			catch ( Exception e )
			{
				ApplicationLog.BaseLog.Error( e );
				throw;
			}
		}

		/// <exception cref="System.Exception">Any exception is thrown when trying to invoke the IsPressurized method.</exception>
		public bool IsPressurized( )
		{
			try
			{
				object airVent = GetActualObject( );
				return (bool) InvokeEntityMethod( airVent, AirVentIsPressurizedMethod );
			}
			catch ( Exception e )
			{
				ApplicationLog.BaseLog.Error( e );
				throw;
			}
		}
	}
}
