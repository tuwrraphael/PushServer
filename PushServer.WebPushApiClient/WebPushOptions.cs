using System;

namespace PushServer.WebPushApiClient
{
    public class WebPushOptions
    {
        public static WebPushOptions Defaults => new WebPushOptions()
        {
            TimeToLive = new TimeSpan(28, 0, 0, 0, 0)
        };

        public TimeSpan TimeToLive { get; set; }
    }
}
