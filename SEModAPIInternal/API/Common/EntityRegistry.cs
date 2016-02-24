using VRage.Game;

namespace SEModAPIInternal.API.Common
{
	using Sandbox.Common.ObjectBuilders;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;

	public class EntityRegistry : GameObjectRegistry
	{
		#region "Attributes"

		private static EntityRegistry m_instance;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		protected EntityRegistry( )
		{
			Register( typeof( MyObjectBuilder_Character ), typeof( CharacterEntity ) );
			Register( typeof( MyObjectBuilder_CubeGrid ), typeof( CubeGridEntity ) );
			Register( typeof( MyObjectBuilder_FloatingObject ), typeof( FloatingObject ) );
			Register( typeof( MyObjectBuilder_Meteor ), typeof( Meteor ) );
			//Register( typeof( MyObjectBuilder_VoxelMap ), typeof( VoxelMap ) );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		new public static EntityRegistry Instance
		{
			get { return m_instance ?? ( m_instance = new EntityRegistry( ) ); }
		}

		#endregion "Properties"
	}
}