namespace SEComm.Plugins
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	public class PluginInfo
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public Version Version { get; set; }
		[DataMember]
		public Guid Id { get; set; }
	}
}
