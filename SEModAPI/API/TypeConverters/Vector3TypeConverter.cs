namespace SEModAPI.API.TypeConverters
{
	using System.ComponentModel;

	public class Vector3TypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3Wrapper((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"]);
		}
	}
}