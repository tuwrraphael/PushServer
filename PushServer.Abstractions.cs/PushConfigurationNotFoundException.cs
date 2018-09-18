using System;
using System.Runtime.Serialization;

namespace PushServer.Abstractions
{
    [Serializable]
    public class PushConfigurationNotFoundException : Exception
    {
        public PushConfigurationNotFoundException()
        {
        }

        public PushConfigurationNotFoundException(string message) : base(message)
        {
        }

        public PushConfigurationNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PushConfigurationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}