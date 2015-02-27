namespace SEComm
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	public class StartServerResponse
	{
		[DataMember]
		public int StatusCode { get; set; }

		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public Version ExtenderVersion { get; set; }

		[DataMember]
		public Version ProtocolVersion { get; set; }
	}
}