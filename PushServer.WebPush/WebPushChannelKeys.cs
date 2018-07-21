using System.ComponentModel.DataAnnotations;

namespace PushServer.WebPush
{
    public class WebPushChannelKeys
    {
        [Required]
        public string P256dh { get; set; }
        [Required]
        public string Auth { get; set; }
    }
}
