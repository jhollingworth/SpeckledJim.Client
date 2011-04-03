using System;
using System.Net;
using System.Text;

namespace SpeckledJim.Client
{
    public class MessageSender
    {
        private readonly Uri _messageBrokerUri;

        public MessageSender(Uri messageBrokerUri)
        {
            _messageBrokerUri = messageBrokerUri;
        }

        public void Send(string endpoint)
        {
            SendMessage(endpoint, string.Empty);
        }

        public void Send(string endpoint, object message, string from, string to)
        {
            SendMessage(endpoint, string.Format("from: node/{0}\r\nto: node/{1}\r\nbody: {2}\r\n", from, to, message));
        }

        private void SendMessage(string endpoint, string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            var request = (HttpWebRequest)WebRequest.Create(_messageBrokerUri.AbsoluteUri + endpoint);
            
            request.Method = "POST";
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.GetResponse();
        }
    }
}