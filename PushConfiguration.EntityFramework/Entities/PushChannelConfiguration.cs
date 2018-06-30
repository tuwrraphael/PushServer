using System;
using System.Collections.Generic;
using PushServer.PushConfiguration.Abstractions.Models;

namespace PushServer.PushConfiguration.EntityFramework.Entities
{
    public class PushChannelConfiguration
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public string EndpointInfo { get; set; }
        public List<PushChannelOption> Options { get; set; }
        public string Endpoint { get; set; }
        public PushChannelType Type { get; set; }
        public string AuthKey { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string P256dhKey { get; set; }
    }
}
