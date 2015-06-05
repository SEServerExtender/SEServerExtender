namespace SEModAPI.API.Definitions.CubeBlocks
{
	using System.ComponentModel;
	using global::Sandbox.Common.ObjectBuilders.Definitions;

	public class GravityGeneratorDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public GravityGeneratorDefinition(MyObjectBuilder_GravityGeneratorDefinition definition)
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
		new public MyObjectBuilder_GravityGeneratorDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_GravityGeneratorDefinition)BaseDefinition;
		}

		#endregion
	}
}
