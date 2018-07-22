using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PushServer.WebPushApiClient
{
    public class WebPushClient : IWebPushClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IVapidAuthenticationProvider vapidAuthenticationProvider;

        public WebPushClient(IHttpClientFactory httpClientFactory, IVapidAuthenticationProvider vapidAuthenticationProvider)
        {
            this.httpClientFactory = httpClientFactory;
            this.vapidAuthenticationProvider = vapidAuthenticationProvider;
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(PushSubscription subscription, WebPushOptions options)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Endpoint);
            //request.Headers.Authorization = new AuthenticationHeaderValue();
            request.Headers.Add("Authorization", $"WebPush {await vapidAuthenticationProvider.GetVapidTokenAsync(subscription.Endpoint)}");
            request.Headers.Add("Crypto-Key", vapidAuthenticationProvider.GetPublicKeyHeaderValue());
            request.Headers.Add("TTL", Math.Floor(options.TimeToLive.TotalSeconds).ToString());
            return request;
        }

        public async Task<HttpResponseMessage> SendNotificationAsync(PushSubscription subscription, WebPushOptions options = null)
        {
            var client = httpClientFactory.CreateClient();
            var request = await CreateRequestAsync(subscription, options ?? WebPushOptions.Defaults);
            request.Content = new ByteArrayContent(new byte[0]);
            request.Content.Headers.ContentLength = 0;
            var response = await client.SendAsync(request);
            var str = await response.Content.ReadAsStringAsync();
            return response;
        }
    }
}
