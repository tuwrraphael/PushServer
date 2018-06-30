using System.Text;

namespace PushServer.Models
{
    public class PushOptions
    {
        /// <summary>
        /// in seconds
        /// </summary>
        public int? TTL { get; set; }
        public string ContentType { get; set; }
        public Encoding Encoding { get; set; }
        public PushUrgency? Urgency { get; set; }
    }
}
