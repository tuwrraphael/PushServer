using PushServer.PushConfiguration.Abstractions.Models;
using System;
using System.Linq;

namespace PushServer.Abstractions
{
    [Serializable]
    public class PushFailedException : AggregateException
    {
        protected static string FormatResults(PushResult[] results) => $"{results.Length} configuration{(results.Length == 1 ? "s" : "")}";

        public PushFailedException(PushResult[] results)
            : base($"Push to {FormatResults(results)} failed.", results.Select(v => v.Exception))
        {
            Failures = results;
        }

        public PushFailedException(PushChannelConfiguration c, Exception e)
            : base($"Push to configuration failed.", e)
        {
            Failures = new[] { new PushResult() { Configuration = c, Exception = e } };
        }

        internal PushFailedException(PushResult[] results, string message)
            : base(message, results.Select(v => v.Exception))
        {
            Failures = results;
        }

        public PushResult[] Failures { get; }

        public class PushResult
        {
            public PushChannelConfiguration Configuration { get; set; }
            public Exception Exception { get; set; }
        }
    }
}