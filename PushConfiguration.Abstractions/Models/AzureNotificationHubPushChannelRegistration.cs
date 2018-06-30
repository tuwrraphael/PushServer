using System.ComponentModel.DataAnnotations;

namespace PushServer.PushConfiguration.Abstractions.Models
{
    public class AzureNotificationHubPushChannelRegistration
    {
        [Required]
        public string Endpoint { get; set; }
        public string DeviceInfo { get; set; }
        public PushChannelOptions Options { get; set; }
    }
}
