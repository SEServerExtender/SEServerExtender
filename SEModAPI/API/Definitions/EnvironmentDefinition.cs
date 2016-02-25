using VRage.Game;

namespace SEModAPI.API.Definitions
{
	using global::Sandbox.Common.ObjectBuilders.Definitions;
	using VRage;

	public class EnvironmentDefinition
	{
		#region "Attributes"

		MyObjectBuilder_EnvironmentDefinition _baseDefinition;

		#endregion

		#region "Constructors and Initializers"

		#endregion

		#region "Properties"

		public bool Changed
		{
			get;
			private set;
		}

		public SerializableVector3 SunDirection
		{
			get { return _baseDefinition.SunDirection; }
			set
			{
				if (_baseDefinition.SunDirection == value)
					return;
				_baseDefinition.SunDirection = value;
				Changed = true;
			}
		}

		public string EnvironmentTexture
		{
			get { return _baseDefinition.EnvironmentTexture; }
			set
			{
				if (_baseDefinition.EnvironmentTexture == value)
					return;
				_baseDefinition.EnvironmentTexture = value;
				Changed = true;
			}
		}

		public MyOrientation EnvironmentOrientation
		{
			get { return _baseDefinition.EnvironmentOrientation; }
			set
			{
				if (_baseDefinition.EnvironmentOrientation.Pitch == value.Pitch &&
					_baseDefinition.EnvironmentOrientation.Roll == value.Roll &&
					_baseDefinition.EnvironmentOrientation.Yaw == value.Yaw) 
					return;
				_baseDefinition.EnvironmentOrientation = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

        #endregion
    }
}
