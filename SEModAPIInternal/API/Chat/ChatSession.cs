namespace SEModAPIInternal.API.Chat
{
	using System;
	using System.Collections.Generic;

	public class ChatSession
	{
		private List<ChatMessage> _messages = new List<ChatMessage>( );
		private DateTimeOffset _lastUpdatedTime = DateTimeOffset.Now;
		private Guid _id = Guid.NewGuid( );
		
		/// <summary>
		/// Gets or sets the identifier for this <see cref="ChatSession"/>
		/// </summary>
		public Guid Id { get { return _id; } set { _id = value; } }

		/// <summary>
		/// Gets or sets a list of <see cref="ChatMessage"/>s for this <see cref="ChatSession"/>.
		/// </summary>
		/// <remarks>Should be cleared after sending to client</remarks>
		public List<ChatMessage> Messages { get { return _messages; } set { _messages = value; } }

		/// <summary>
		/// Gets or sets the last time this <see cref="ChatSession"/> was updated. Used for aging.
		/// </summary>
		public DateTimeOffset LastUpdatedTime { get { return _lastUpdatedTime; } set { _lastUpdatedTime = value; } }
	}
}
