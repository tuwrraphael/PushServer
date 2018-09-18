using PushServer.Models;
using System.Collections.Generic;

namespace DigitPushService.Models
{
    public class PushRequest
    {
        public string ChannelId { get; set; }
        public IDictionary<string, string> ChannelOptions { get; set; }
        //public PushOptions Options { get; set; }
        public string Payload { get; set; }
    }
}
