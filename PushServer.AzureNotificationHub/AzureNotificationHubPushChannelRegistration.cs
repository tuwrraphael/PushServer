using PushServer.PushConfiguration.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace PushServer.AzureNotificationHub
{
    public class AzureNotificationHubPushChannelRegistration
    {
        [Required]
        public string Endpoint { get; set; }
        public string DeviceInfo { get; set; }
        public PushChannelOptions Options { get; set; }
    }
}
