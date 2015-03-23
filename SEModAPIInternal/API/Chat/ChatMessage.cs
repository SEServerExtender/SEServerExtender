namespace SEModAPIInternal.API.Chat
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	public class ChatMessage
	{
		[DataMember]
		public DateTimeOffset Timestamp { get; set; }
		[DataMember]
		public string User { get; set; }
		[DataMember]
		public ulong UserId { get; set; }
		[DataMember]
		public string Message { get; set; }
	}
}
