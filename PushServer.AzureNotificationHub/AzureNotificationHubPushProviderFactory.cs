using Microsoft.Extensions.Options;
using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using System.Threading.Tasks;

namespace PushServer.AzureNotificationHub
{
    internal class AzureNotificationHubPushProviderFactory : IPushProviderFactory
    {
        private readonly IOptions<AzureNotificationHubConfig> options;
        private readonly IPushConfigurationStore pushConfigurationStore;

        public AzureNotificationHubPushProviderFactory(IOptions<AzureNotificationHubConfig> options, IPushConfigurationStore pushConfigurationStore)
        {
            this.options = options;
            this.pushConfigurationStore = pushConfigurationStore;
        }

        public string PushChannelType => AzureNotificationHubConstants.ChannelType;

        public async Task<IPushProvider> CreateProvider(PushChannelConfiguration config)
        {
            return new AzureNotificationPushProvider(options, config, await pushConfigurationStore.GetEndpointAsync(config.Id));
        }
    }
}
