namespace Live.Log.Extractor.Web.Helper
{
    using System.Threading.Tasks;
    using SignalR.Hubs;
    using System;

    /// <summary>
    /// Chat Class to chat with browser(display messages in real time)
    /// </summary>
    public class Chat : Hub
    {
        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(string connectionId, string message)
        {
            Clients.addMessage(connectionId, message);
        }
    }
}