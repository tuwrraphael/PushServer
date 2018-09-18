using DigitPushService.Models;
using Newtonsoft.Json;
using OAuthApiClient.Abstractions;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigitPushService.Client
{
    public class DigitPushServiceClient : IDigitPushServiceClient
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly HttpClient httpClient;

        private async Task<HttpClient> ClientFactory()
        {
            await authenticationProvider.AuthenticateClient(httpClient);
            return httpClient;
        }

        public DigitPushServiceClient(IAuthenticationProvider authenticationProvider,
            HttpClient httpClient)
        {
            this.authenticationProvider = authenticationProvider;
            this.httpClient = httpClient;
        }

        public IPushCollection Push => new PushCollection(ClientFactory);

        private class PushCollection : IPushCollection
        {
            private readonly Func<Task<HttpClient>> clientFactory;

            public PushCollection(Func<Task<HttpClient>> clientFactory)
            {
                this.clientFactory = clientFactory;
            }

            public IPushApi this[string userId] => new PushApi(clientFactory, userId);
        }

        private class PushApi : IPushApi
        {
            private readonly Func<Task<HttpClient>> clientFactory;
            private readonly string userId;

            public PushApi(Func<Task<HttpClient>> clientFactory, string userId)
            {
                this.clientFactory = clientFactory;
                this.userId = userId;
            }

            public async Task Create(PushRequest pushRequest)
            {
                var client = await clientFactory();
                var json = JsonConvert.SerializeObject(pushRequest);
                var res = await client.PostAsync($"api/{userId}/push",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (!res.IsSuccessStatusCode)
                {
                    throw new DigitPushServiceException($"Push creation request resulted in {res.StatusCode}.");
                }
            }
        }
    }
}
