namespace SEModAPIExtensions.API
{
	using System.Collections.Generic;
	using System.ServiceModel;

	[ServiceContract]
	public interface IChatServiceContract
	{
		[OperationContract]
		List<string> GetChatMessages();

		[OperationContract]
		void SendPrivateChatMessage(ulong remoteUserId, string message);

		[OperationContract]
		void SendPublicChatMessage(string message);
	}
}