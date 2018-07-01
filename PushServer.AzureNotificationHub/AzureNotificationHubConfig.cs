namespace PushServer.AzureNotificationHub
{
    public class AzureNotificationHubConfig
    {
        public string HubConnection { get; set; }
        public string HubName { get; set; }
        public string HubNamespace { get; set; }
        public string HubSASKey { get; set; }
        public string HubSASKeyName { get; set; }
    }
}
