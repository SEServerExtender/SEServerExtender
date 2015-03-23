namespace SEModAPIInternal.API.Chat
{
	using System;
	using System.Collections.Generic;

	public class ChatSessionManager
	{
		public static ChatSessionManager Instance { get { return _instance ?? new ChatSessionManager( ); } }
		private static ChatSessionManager _instance;
		private Dictionary<Guid, ChatSession> _sessions = new Dictionary<Guid, ChatSession>( );

		public static object SessionsMutex = new object( );

		private ChatSessionManager( )
		{
			_instance = this;
		}

		public Dictionary<Guid, ChatSession> Sessions { get { return _sessions; } set { _sessions = value; } }
	}
}
