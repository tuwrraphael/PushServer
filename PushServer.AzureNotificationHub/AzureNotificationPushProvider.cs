using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using PushServer.Abstractions;
using PushServer.Abstractions.Services;
using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PushServer.AzureNotificationHub
{
    public class AzureNotificationPushProvider : IPushProvider
    {
        private readonly PushChannelConfiguration config;
        private readonly PushEndpoint endpoint;
        private AzureNotificationHubConfig options;

        public AzureNotificationPushProvider(IOptions<AzureNotificationHubConfig> optionsAccessor, PushChannelConfiguration config, PushEndpoint endpoint)
        {
            options = optionsAccessor.Value;
            this.config = config;
            this.endpoint = endpoint;
        }

        public async Task InitializeAsync()
        {
            //NotificationHubClient client = NotificationHubClient.CreateClientFromConnectionString(options.HubConnection, options.HubName);
            //var reg = await client.GetRegistrationAsync<RegistrationDescription>(endpoint.Endpoint);
            //if (reg.Tags == null)
            //{
            //    reg.Tags = new HashSet<string>() { config.Id };
            //}
            //else
            //{
            //    reg.Tags.Add(config.Id);
            //}
            //await client.UpdateRegistrationAsync(reg);
        }

        private static string GetSASToken(string resourceUri, string keyName, string key)
        {
            var expiry = GetExpiry();
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            return sasToken;
        }

        private static string GetExpiry()
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Convert.ToString((int)sinceEpoch.TotalSeconds + 3600);
        }

        public class OctetStreamStringContent : StringContent
        {
            public OctetStreamStringContent(string content, Encoding encoding, string contentType)
                : base(content, encoding, contentType)
            {
                Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
        }

        public async Task PushAsync(string payload, PushOptions opts)
        {
            var uri = $"https://{options.HubNamespace}.servicebus.windows.net/{options.HubName}/messages/?api-version=2015-01";
            var token = GetSASToken(uri, options.HubSASKeyName, options.HubSASKey);
            var cl = new HttpClient();
            cl.DefaultRequestHeaders.Add("X-WNS-Type", "wns/raw");
            cl.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            cl.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "windows");
            cl.DefaultRequestHeaders.Add("ServiceBusNotification-Tags", config.Id);
            var res = await cl.PostAsync(uri, new OctetStreamStringContent(payload, Encoding.Default,
                "application/octet-stream"));
            if (!res.IsSuccessStatusCode)
            {
                throw new PushException($"Attempted delivery resulted in {res.StatusCode}.");
            }
        }
    }
}
