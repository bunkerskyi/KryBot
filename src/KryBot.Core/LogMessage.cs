/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System;
using System.Drawing;

namespace KryBot.Core
{
    public class LogMessage
    {
        /// <summary>
        ///     Gets the instance of the class.
        /// </summary>
        public static LogMessage Instance { get; } = new LogMessage();

        public event EventHandler HandleMessage;

        /// <summary>
        ///     Notifies any of the subscribers that a new message has been received.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        private void NotifyNewMessage(string message, Color color)
        {
            var handler = HandleMessage;
            // This will call the any form that is currently "wired" to the event, notifying them
            // of the new message.
            handler?.Invoke(this, new MessageEventArgs(message, color));
        }

        /// <summary>
        ///     Adds a new messages to the "central" list
        /// </summary>
        /// <param name="log">The message.</param>
        public void AddMessage(Log log)
        {
            if (log != null)
            {
                NotifyNewMessage(log.Content, log.Color);
            }
        }
    }

    /// <summary>
    ///     Special Event Args used to pass the message data to the subscribers.
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