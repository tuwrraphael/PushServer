using System.Net.Http;
using System.Threading.Tasks;

namespace PushServer.WebPushApiClient
{
    public interface IWebPushClient
    {
        Task<HttpResponseMessage> SendNotificationAsync(PushSubscription subscription, WebPushOptions options = null);
        Task<HttpResponseMessage> SendNotificationAsync(PushSubscription subscription, string payload, WebPushOptions options = null);
    }
}