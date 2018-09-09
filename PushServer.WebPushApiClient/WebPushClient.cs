using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
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
        private readonly IAuthenticatedEncryptorFactory authenticatedEncryptor;

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

        public async Task<HttpResponseMessage> SendNotificationAsync(PushSubscription subscription, string payload, WebPushOptions options = null)
        {
            var client = httpClientFactory.CreateClient();
            var request = await CreateRequestAsync(subscription, options ?? WebPushOptions.Defaults);
            if (string.IsNullOrEmpty(subscription.P256dh) || string.IsNullOrEmpty(subscription.Auth))
            {
                throw new ArgumentException(
                    @"Unable to send a message with payload to this subscription since it doesn't have the required encryption key");
            }
            var encryptedPayload = Encryptor.Encrypt(subscription.P256dh, subscription.Auth, payload);
            request.Content = new ByteArrayContent(encryptedPayload.Payload);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content.Headers.ContentLength = encryptedPayload.Payload.Length;
            request.Content.Headers.ContentEncoding.Add("aesgcm");
            request.Headers.Add("Encryption", "salt=" + encryptedPayload.Salt);
            request.Headers.Add("Crypto-Key", @"dh=" + encryptedPayload.PublicKey);
            var response = await client.SendAsync(request);
            var str = await response.Content.ReadAsStringAsync();
            return response;
        }
    }
}
