namespace SEModAPIExtensions.API
{
	using System.Collections.Generic;
	using System.ServiceModel;

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Single,
		IncludeExceptionDetailInFaults = true,
		IgnoreExtensionDataObject = true
		)]
	public class ChatService : IChatServiceContract
	{
		public List<string> GetChatMessages()
		{
			return ChatManager.Instance.ChatMessages;
		}

		public void SendPrivateChatMessage(ulong remoteUserId, string message)
		{
			ChatManager.Instance.SendPrivateChatMessage(remoteUserId, message);
		}

		public void SendPublicChatMessage(string message)
		{
			ChatManager.Instance.SendPublicChatMessage(message);
		}
	}
}