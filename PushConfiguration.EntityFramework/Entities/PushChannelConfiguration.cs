using System;
using System.Collections.Generic;

namespace PushServer.PushConfiguration.EntityFramework.Entities
{
    public class PushChannelConfiguration
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public string EndpointInfo { get; set; }
        public List<PushChannelOption> Options { get; set; }
        public string Endpoint { get; set; }
        public string Type { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
}
