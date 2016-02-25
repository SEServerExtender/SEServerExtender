using VRage.Game;

namespace SEModAPI.API.Definitions
{
	using System.ComponentModel;
	using global::Sandbox.Common.ObjectBuilders.Definitions;
	using VRage.ObjectBuilders;

	/// <summary>
	/// This class is only intended for easy data access and management. It is a wrapper around all MyObjectBuilder_DefinitionsBase children sub type
	/// </summary>
	public abstract class DefinitionOverLayer
	{
		#region "Attributes"

		/// <summary>
		/// Gets the Internal data of the object
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Internal data of the object")]
		protected MyObjectBuilder_DefinitionBase BaseDefinition;

		#endregion

		#region "Constructors and Initializers"

		protected DefinitionOverLayer(MyObjectBuilder_DefinitionBase baseDefinition)
		{
			BaseDefinition = baseDefinition;
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Gets the changed status of the object
		/// </summary>
		[Browsable(true)]
		[Description("Represent the state of changes in the object")]
		public virtual bool Changed { get; protected set; }

		/// <summary>
		/// Obtain the API formated name of the object
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The formatted name of the object")]
		public abstract string Name { get; }


		/// <summary>
		/// Gets the internal data of the object
		/// </summary>
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("The value ID representing the type of the object")]
		public MyObjectBuilderType TypeId
		{
			get { return BaseDefinition.TypeId; }
		}

		/// <summary>
		/// Gets the internal Id of the instance
		/// </summary>
		[Browsable(true)]
		[Description("The Id as SerializableDefinitionId that represent the Object representation")]
		public SerializableDefinitionId Id
		{
			get { return BaseDefinition.Id; }
			set
			{
				if (BaseDefinition.Id.ToString() == value.ToString())
					return;
				BaseDefinition.Id = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Get the description of the object
		/// </summary>
		[Browsable(true)]
		[Description("Represents the description of the object")]
		public string Description
		{
			get { return BaseDefinition.Description; }
			set
			{
				if (BaseDefinition.Description == value) return;
				BaseDefinition.Description = value;
				Changed = true;
			}
		}

		#endregion
	}
}