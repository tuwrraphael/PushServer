namespace PushServer.PushConfiguration.Abstractions.Models
{
    public class PushChannelConfiguration
    {
        public string Id { get; set; }
        public string EndpointInfo { get; set; }
        public string ChannelType { get; set; }
        public PushChannelOptions Options { get; set; }
    }
}
