using System;

namespace PushServer.WebPushApiClient
{
    public class PushSubscription
    {
        public Uri Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }
}
