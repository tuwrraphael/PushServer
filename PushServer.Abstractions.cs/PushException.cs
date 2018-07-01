using System;
using System.Runtime.Serialization;

namespace PushServer.Abstractions
{
    [Serializable]
    public class PushException : Exception
    {
        public PushException()
        {
        }

        public PushException(string message) : base(message)
        {
        }

        public PushException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PushException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}