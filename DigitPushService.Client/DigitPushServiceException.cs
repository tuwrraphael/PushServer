using System;
using System.Runtime.Serialization;

namespace DigitPushService.Client
{
    [Serializable]
    internal class DigitPushServiceException : Exception
    {
        public DigitPushServiceException()
        {
        }

        public DigitPushServiceException(string message) : base(message)
        {
        }

        public DigitPushServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DigitPushServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}