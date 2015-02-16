namespace SEModAPI.API.TypeConverters
{
	using System.ComponentModel;
	using VRageMath;

	public class Vector3ITypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3I((int)propertyValues["X"], (int)propertyValues["Y"], (int)propertyValues["Z"]);
		}
	}
}