namespace PushServer.PushConfiguration.EntityFramework.Entities
{
    public class PushChannelOption
    {
        public string ID { get; set; }
        public string PushChannelConfigurationID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool EndpointOption { get; set; }
        public PushChannelConfiguration PushChannelConfiguration { get; set; }
    }
}
