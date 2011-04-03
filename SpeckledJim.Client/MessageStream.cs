using System;
using System.Net;
using System.Net.NetworkInformation;
using Hammock;
using Hammock.Retries;
using Hammock.Streaming;
using Hammock.Web;

namespace SpeckledJim.Client
{
    internal class MessageStream
    {
        private readonly RestClient _client;
        private IAsyncResult _asyncState;

        public IAsyncResult AsyncState
        {
            get { return _asyncState; }
        }

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved = delegate { };

        public MessageStream(Uri messageBrokerUri)
        {
            ValidateBrokerUri(messageBrokerUri);

            _client = new RestClient
            {
                Authority = messageBrokerUri.AbsoluteUri,
                RetryPolicy = new RetryPolicy {  RetryCount = 2}
            };

            _client.BeforeRetry += BeforeRetry;
        }

        private static void ValidateBrokerUri(Uri messageBrokerUri)
        {
            try
            {
                var request = WebRequest.Create(messageBrokerUri.AbsoluteUri + "ping");
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();

                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    var message = "Failed to connect to message broker";

                    if (response != null)
                    {
                        message += "(" + (int)response.StatusCode + ")";
                    }

                    throw new WebException(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid broker uri: " + ex.Message);
                throw;
            }
            Console.WriteLine("Message broker is online");
        }

        public void Connect(string id)
        {
            
            var request = new RestRequest()
            {
                Path = "messages/" + id,
                Method = WebMethod.Get,
                StreamOptions = new StreamOptions { ResultsPerCallback = 1 },
                Proxy = "127.0.0.1:8888"
            };

            try
            {
                _client.BeginRequest(request, new RestCallback((req, res, state) =>
                {
                    if (res != null)
                    {
                        MessageRecieved(this, new MessageRecievedEventArgs(res.Content));
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        void BeforeRetry(object sender, RetryEventArgs e)
        {
            Console.WriteLine("Retrying...");
        }
    }

    public class MessageRecievedEventArgs : EventArgs
    {
        public string Content { get; set; }

        public MessageRecievedEventArgs(string content)
        {
            Content = content;
        }
    }
}