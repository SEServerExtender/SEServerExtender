using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract( Name = "LightEntityProxy" )]
	public class LightEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string LightNamespace = "";
		public static string LightClass = "=FS9XaTQBULhE857hF1lkq5eY89=";

		public static string LightUpdateColorMethod = "set_Color";
		public static string LightUpdateIntensityMethod = "set_Intensity";
		public static string LightUpdateFalloffMethod = "set_Falloff";
		public static string LightUpdateRadiusMethod = "set_Radius";

		public static string LightNetworkManagerField = "=gNAvpmtOwXYmmcG9ynRDGRitUw=";

		////////////////////////////////////////////////////////////////////////

		public static string LightNetworkManagerNamespace = "";
		public static string LightNetworkManagerClass = "=KQ3UhmTVDnlgjehKAcLT0Ldkte=";

		// Need to find this method update again, lights changed completely - I believe it changed to setting individual properties instead of all of them:
		// Color, Radius, Falloff, Intensity, BlinkInterval, BlinkLength, BlinkOffset - 
		//public static string LightNetworkManagerSendUpdateMethod = "582447224E2B03FA4EAB3D6C2DDD48D9";	//Color, Radius, Falloff, Intensity

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public LightEntity( CubeGridEntity parent, MyObjectBuilder_LightingBlock definition )
			: base( parent, definition )
		{
		}

		public LightEntity( CubeGridEntity parent, MyObjectBuilder_LightingBlock definition, Object backingObject )
			: base( parent, definition, backingObject )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		[IgnoreDataMember]
		[Category( "Light" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_LightingBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_LightingBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category( "Light" )]
		[Browsable( false )]
		public Color Color
		{
			get
			{
				MyObjectBuilder_LightingBlock baseEntity = ObjectBuilder;
				Color color = new Color( baseEntity.ColorRed, baseEntity.ColorGreen, baseEntity.ColorBlue, baseEntity.ColorAlpha );

				return color;
			}
			set
			{
				if ( Color == value ) return;
				MyObjectBuilder_LightingBlock baseEntity = ObjectBuilder;
				baseEntity.ColorAlpha = value.A;
				baseEntity.ColorRed = value.R;
				baseEntity.ColorGreen = value.G;
				baseEntity.ColorBlue = value.B;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Light" )]
		public float ColorAlpha
		{
			get { return ObjectBuilder.ColorAlpha; }
			set
			{
				if ( ObjectBuilder.ColorAlpha == value ) return;
				ObjectBuilder.ColorAlpha = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Light" )]
		public float ColorRed
		{
			get { return ObjectBuilder.ColorRed; }
			set
			{
				if ( ObjectBuilder.ColorRed == value ) return;
				ObjectBuilder.ColorRed = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Light" )]
		public float ColorGreen
		{
			get { return ObjectBuilder.ColorGreen; }
			set
			{
				if ( ObjectBuilder.ColorGreen == value ) return;
				ObjectBuilder.ColorGreen = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[IgnoreDataMember]
		[Category( "Light" )]
		public float ColorBlue
		{
			get { return ObjectBuilder.ColorBlue; }
			set
			{
				if ( ObjectBuilder.ColorBlue == value ) return;
				ObjectBuilder.ColorBlue = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Light" )]
		public float Intensity
		{
			get { return ObjectBuilder.Intensity; }
			set
			{
				if ( ObjectBuilder.Intensity == value ) return;
				ObjectBuilder.Intensity = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Light" )]
		public float Falloff
		{
			get { return ObjectBuilder.Falloff; }
			set
			{
				if ( ObjectBuilder.Falloff == value ) return;
				ObjectBuilder.Falloff = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Light" )]
		public float Radius
		{
			get { return ObjectBuilder.Radius; }
			set
			{
				if ( ObjectBuilder.Radius == value ) return;
				ObjectBuilder.Radius = value;
				Changed = true;

				if ( BackingObject != null )
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
				}
			}
		}

		[DataMember]
		[Category( "Light" )]
		[ReadOnly( true )]
		public float BlinkIntervalSeconds
		{
			get
			{
				return ObjectBuilder.BlinkIntervalSeconds;
			}
			private set
			{
			}
		}

		[DataMember]
		[Category( "Light" )]
		[ReadOnly( true )]
		public float BlinkLength
		{
			get
			{
				//Note: Keen's code has this misspelled. It is not a typo
				return ObjectBuilder.BlinkLenght;
			}
			private set
			{
			}
		}

		[DataMember]
		[Category( "Light" )]
		[ReadOnly( true )]
		public float BlinkOffset
		{
			get
			{
				return ObjectBuilder.BlinkOffset;
			}
			private set
			{
			}
		}

		#endregion "Properties"

		#region "Methods"

		new public static bool ReflectionUnitTest( )
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( LightNamespace, LightClass );
				if ( type == null )
					throw new Exception( "Could not find internal type for LightEntity" );
				result &= HasMethod( type, LightUpdateColorMethod );
				result &= HasMethod( type, LightUpdateIntensityMethod );
				result &= HasMethod( type, LightUpdateFalloffMethod );
				result &= HasMethod( type, LightUpdateRadiusMethod );
				result &= HasField( type, LightNetworkManagerField );

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( LightNetworkManagerNamespace, LightNetworkManagerClass );
				if ( type2 == null )
					throw new Exception( "Could not find network manager type for LightEntity" );
				//result &= HasMethod( type2, LightNetworkManagerSendUpdateMethod );

				return result;
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex );
				return false;
			}
		}

		protected Object GetLightNetworkManager( )
		{
			try
			{
				Object actualObject = GetActualObject( );

				FieldInfo field = GetEntityField( actualObject, LightNetworkManagerField );
				Object networkManager = field.GetValue( actualObject );
				return networkManager;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return null;
			}
		}

		protected void InternalUpdateLight( )
		{
			try
			{
				Object actualObject = GetActualObject( );

				Color color = new Color( ColorRed / 255.0f, ColorGreen / 255.0f, ColorBlue / 255.0f, ColorAlpha / 255.0f );
				InvokeEntityMethod( actualObject, LightUpdateColorMethod, new object[ ] { color } );
				InvokeEntityMethod( actualObject, LightUpdateIntensityMethod, new object[ ] { Intensity } );
				InvokeEntityMethod( actualObject, LightUpdateFalloffMethod, new object[ ] { Falloff } );
				InvokeEntityMethod( actualObject, LightUpdateRadiusMethod, new object[ ] { Radius } );

				Object netManager = GetLightNetworkManager( );
				//InvokeEntityMethod( netManager, LightNetworkManagerSendUpdateMethod, new object[ ] { color, Radius, Falloff, Intensity } );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}
}