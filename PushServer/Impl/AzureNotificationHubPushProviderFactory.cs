using Microsoft.Extensions.Options;
using PushServer.Configuration;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using PushServer.Services;
using System.Threading.Tasks;

namespace PushServer.Impl
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

        public PushChannelType Type => PushChannelType.AzureNotificationHub;

        public async Task<IPushProvider> CreateProvider(PushChannelConfiguration config)
        {
            return new AzureNotificationPushProvider(options, config, await pushConfigurationStore.GetEndpointAsync(config.Id));
        }
    }
}
