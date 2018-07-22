using System.Threading.Tasks;
using PushServer.Abstractions;
using PushServer.Abstractions.Services;
using PushServer.Models;
using PushServer.WebPushApiClient;

namespace PushServer.WebPush
{
    public class WebPushProvider : IPushProvider
    {
        private PushSubscription pushSubscription;
        private readonly IWebPushClient webPushClient;

        public WebPushProvider(PushSubscription pushSubscription,
            IWebPushClient webPushClient)
        {
            this.pushSubscription = pushSubscription;
            this.webPushClient = webPushClient;
        }

        public Task InitializeAsync() { return Task.CompletedTask; }


        public async Task PushAsync(string payload, PushOptions options)
        {
            var webPushOptions = WebPushOptions.Defaults;
            if (options.TimeToLive.HasValue)
            {
                webPushOptions.TimeToLive = options.TimeToLive.Value;
            }
            var res = await webPushClient.SendNotificationAsync(pushSubscription, webPushOptions);
            if (!res.IsSuccessStatusCode)
            {
                throw new PushException($"Attempted delivery resulted in {res.StatusCode}.");
            }
        }
    }
}