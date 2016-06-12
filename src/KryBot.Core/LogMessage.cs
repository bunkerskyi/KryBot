using System;
using System.Collections.Generic;
using System.Drawing;

namespace KryBot.Core
{
	public class LogMessage
	{
		private readonly List<string> _messages = new List<string>();
		public event EventHandler HandleMessage;

		/// <summary>
		/// Prevents a default instance of the <see cref="Messages"/> class from being created.
		/// </summary>
		private LogMessage(){}

		/// <summary>
		/// Gets the instance of the class.
		/// </summary>
		public static LogMessage Instance { get; } = new LogMessage();

		/// <summary>
		/// Gets the current messages list.
		/// </summary>
		public List<string> CurrentMessages => _messages;

		/// <summary>
		/// Notifies any of the subscribers that a new message has been received.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="color">The color.</param>
		private void NotifyNewMessage(string message, Color color)
		{
			EventHandler handler = HandleMessage;
			// This will call the any form that is currently "wired" to the event, notifying them
			// of the new message.
			handler?.Invoke(this, new MessageEventArgs(message, color));
		}

		/// <summary>
		/// Adds a new messages to the "central" list
		/// </summary>
		/// <param name="log">The message.</param>
		public void AddMessage(Log log)
		{
			_messages.Add(log.Content);
			NotifyNewMessage(log.Content, log.Color);
		}
	}

	/// <summary>
	/// Special Event Args used to pass the message data to the subscribers.
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(string message, Color color)
		{
			Message = message;
			Color = color;
		}

		public string Message { get; }
		public Color Color { get; }
	}
}
