using PushServer.PushConfiguration.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace PushServer.Firebase
{
    public class FirebasePushChannelRegistration
    {
        [Required]
        public string Token { get; set; }
        public string DeviceInfo { get; set; }
        public PushChannelOptions Options { get; set; }
    }
}
