using System;
using System.Collections.Generic;

namespace PushServer.PushConfiguration.Abstractions.Models
{
    public class PushChannelRegistration
    {
        public string Endpoint { get; set; }
        public string EndpointInfo { get; set; }
        public PushChannelOptions Options { get; set; }
        public IDictionary<string, string> EndpointOptions { get; set; }
        public string PushChannelType { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
}
