using System;
using System.Text;

namespace PushServer.Models
{
    public class PushOptions
    {
        public TimeSpan? TimeToLive { get; set; }
        public string ContentType { get; set; }
        public Encoding Encoding { get; set; }
        public PushUrgency? Urgency { get; set; }
    }
}
