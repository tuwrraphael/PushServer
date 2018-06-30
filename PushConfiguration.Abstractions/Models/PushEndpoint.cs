namespace PushServer.PushConfiguration.Abstractions.Models
{
    public class PushEndpoint
    {
        public string Endpoint { get; set; }
        public PushChannelType ChannelType { get; set; }
        public string AuthKey { get; set; }
        public string P256dhKey { get; set; }
    }
}
