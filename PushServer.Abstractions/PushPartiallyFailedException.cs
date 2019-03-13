using System;

namespace PushServer.Abstractions
{
    [Serializable]
    public class PushPartiallyFailedException : PushFailedException
    {
        public PushPartiallyFailedException(PushResult[] succeeded, PushResult[] failed)
            : base(failed, $"Push succeeded to {FormatResults(succeeded)} but {FormatResults(failed)} failed.")
        {
            Succeeded = succeeded;
        }

        public PushResult[] Succeeded { get; }
    }
}