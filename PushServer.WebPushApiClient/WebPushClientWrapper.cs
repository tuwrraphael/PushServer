using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebPush;

namespace PushServer.WebPushApiClient
{
    public class WebPushClientWrapper : IWebPushClient
    {
        private readonly VapidAuthenticationOptions vapidAuthenticationOptions;
        private readonly IHttpClientFactory httpClientFactory;

        public WebPushClientWrapper(IHttpClientFactory httpClientFactory,
            IOptions<VapidAuthenticationOptions> vapidAuthenticationOptionsAccessor)
        {
            vapidAuthenticationOptions = vapidAuthenticationOptionsAccessor.Value;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> SendNotificationAsync(PushSubscription subscription, WebPushOptions options = null)
        {
            var client = new WebPushClient();
            var httpClient = httpClientFactory.CreateClient();
            client.SetVapidDetails(vapidAuthenticationOptions.Subject, vapidAuthenticationOptions.PublicKey, vapidAuthenticationOptions.PrivateKey);
            var request = client.GenerateRequestDetails(new WebPush.PushSubscription()
            {
                Auth = subscription.Auth,
                Endpoint = subscription.Endpoint.ToString(),
                P256DH = subscription.P256dh
            }, null);
            return await httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SendNotificationAsync(PushSubscription subscription, string payload, WebPushOptions options = null)
        {
            var client = new WebPushClient();
            var httpClient = httpClientFactory.CreateClient();
            client.SetVapidDetails(vapidAuthenticationOptions.Subject, vapidAuthenticationOptions.PublicKey, vapidAuthenticationOptions.PrivateKey);
            var request = client.GenerateRequestDetails(new WebPush.PushSubscription()
            {
                Auth = subscription.Auth,
                Endpoint = subscription.Endpoint.ToString(),
                P256DH = subscription.P256dh
            }, payload);
            return await httpClient.SendAsync(request);
        }
    }
}
