namespace SEModAPIInternal
{
	using System.Runtime.Serialization;

	[DataContract]
	public class ChatUserItem
	{
		[DataMember]
		public ulong SteamId;
		[DataMember]
		public string Username;
		public override string ToString()
		{
			return string.Format("{0} ({1})", Username, SteamId);
		}
	}
}