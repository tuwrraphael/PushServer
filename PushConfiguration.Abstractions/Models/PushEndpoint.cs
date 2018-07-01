using System.Collections.Generic;

namespace PushServer.PushConfiguration.Abstractions.Models
{
    public class PushEndpoint
    {
        public string Endpoint { get; set; }
        public IDictionary<string, string> EndpointOptions { get; set; }
    }
}
