using System;
using System.Threading;

namespace SpeckledJim.Client
{
    public class SpeckledJimClient : IDisposable
    {
        private readonly MessageStream _stream;
        private readonly MessageSender _messages;
        private string _id;

        public SpeckledJimClient(Uri messageBrokerUri)
        {
            _stream = new MessageStream(messageBrokerUri);
            _messages = new MessageSender(messageBrokerUri);
            _stream.MessageRecieved += MessageRecieved;
        }

        private void MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.Content.Equals("connected: true"))
            {
                Console.WriteLine("Connected");
            }
            else
            {
                if (e.Content.Trim().Equals("ping"))
                {
                    Console.WriteLine("PING");
                    Send("pong", "pong");
                }
            }
        }

        public void Connect(string id)
        {
            _id = id;
            _stream.Connect(id);    
        }

        public void Send(object message, string to)
        {  
            _messages.Send("messages", message, _id, to);
        }

        public void Dispose()
        {
            _messages.Send("disconnect/" + _id);
            _stream.MessageRecieved -= MessageRecieved;
        }
    }
}