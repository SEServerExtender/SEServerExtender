namespace SEModAPI.API.TypeConverters
{
	using System.Collections;
	using System.ComponentModel;

	public class Vector3DTypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new Vector3DWrapper((double)propertyValues["X"], (double)propertyValues["Y"], (double)propertyValues["Z"]);
		}
	}
}