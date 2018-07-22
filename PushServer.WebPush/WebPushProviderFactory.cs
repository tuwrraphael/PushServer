using System;
using System.Threading.Tasks;
using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using PushServer.WebPushApiClient;

namespace PushServer.WebPush
{
    internal class WebPushProviderFactory : IPushProviderFactory
    {
        public string PushChannelType => WebPushConstants.ChannelType;

        private readonly IPushConfigurationStore pushConfigurationStore;
        private readonly IWebPushClient webPushClient;

        public WebPushProviderFactory(IPushConfigurationStore pushConfigurationStore, IWebPushClient webPushClient)
        {
            this.pushConfigurationStore = pushConfigurationStore;
            this.webPushClient = webPushClient;
        }

        public async Task<IPushProvider> CreateProvider(PushChannelConfiguration config)
        {
            var endpoint = await pushConfigurationStore.GetEndpointAsync(config.Id);
            var subscription = new PushSubscription()
            {
                Auth = endpoint.EndpointOptions["AuthKey"],
                Endpoint = new Uri(endpoint.Endpoint),
                P256dh = endpoint.EndpointOptions["P256dhKey"]
            };
            return new WebPushProvider(subscription, webPushClient);
        }
    }
}