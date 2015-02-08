namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	using System;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.Support;

	public class FloatingObjectManager
	{
		#region "Attributes"

		private static FloatingObjectManager m_instance;
		private static Type m_internalType;

		private FloatingObject m_floatingObjectToChange;

		public static string FloatingObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectManagerClass = "66E5A072764E86AD0AC8B63304F0DC31";

		public static string FloatingObjectManagerRemoveFloatingObjectMethod = "CDD52493D4DD9E7D7BDB9AFC5512A9E1";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected FloatingObjectManager( )
		{
			m_instance = this;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		internal static Type InternalType
		{
			get { return m_internalType ?? ( m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( FloatingObjectManagerNamespace, FloatingObjectManagerClass ) ); }
		}

		public static FloatingObjectManager Instance
		{
			get { return m_instance ?? ( m_instance = new FloatingObjectManager( ) ); }
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for FloatingObjectManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type, FloatingObjectManagerRemoveFloatingObjectMethod );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		public void RemoveFloatingObject( FloatingObject floatingObject )
		{
			m_floatingObjectToChange = floatingObject;

			Action action = InternalRemoveFloatingObject;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
		}

		protected void InternalRemoveFloatingObject( )
		{
			if ( m_floatingObjectToChange == null )
				return;

			Object backingObject = m_floatingObjectToChange.BackingObject;
			BaseObject.InvokeStaticMethod( InternalType, FloatingObjectManagerRemoveFloatingObjectMethod, new[ ] { backingObject } );

			m_floatingObjectToChange = null;
		}

		#endregion "Methods"
	}
}