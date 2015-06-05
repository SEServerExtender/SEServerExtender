namespace SEModAPI.API.Definitions.CubeBlocks
{
	using System.ComponentModel;
	using global::Sandbox.Common.ObjectBuilders.Definitions;

	public class GyroscopeDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public GyroscopeDefinition(MyObjectBuilder_GyroDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Gravity generator required power input
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Gravity generator required power input.")]
		public float RequiredPowerInput
		{
			get { return GetSubTypeDefinition().RequiredPowerInput; }
			set
			{
				if (GetSubTypeDefinition().RequiredPowerInput.Equals(value)) return;
				GetSubTypeDefinition().RequiredPowerInput = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_GyroDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_GyroDefinition)BaseDefinition;
		}

		#endregion
	}
}