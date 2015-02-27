namespace SEComm
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	public class StartServerRequest
	{
		[DataMember]
		public Version ProtocolVersion { get; set; }

		[DataMember]
		public string ConfigurationName { get; set; }
	}
}