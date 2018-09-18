using System;
using System.Net.Http;

namespace PushServer.Abstractions
{
    [Serializable]
    public class PushException : Exception
    {
        public PushException()
        {
        }

        public PushException(string message, HttpResponseMessage responseMessage) : base(message)
        {
            ResponseMessage = responseMessage;
        }

        public HttpResponseMessage ResponseMessage { get; }
    }
}