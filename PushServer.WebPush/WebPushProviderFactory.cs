using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;

namespace PushServer.WebPush
{
    internal class WebPushProviderFactory : IPushProviderFactory
    {
        public string PushChannelType => WebPushConstants.ChannelType;

        private readonly IOptions<WebPushConfig> options;
        private readonly IPushConfigurationStore pushConfigurationStore;

        public WebPushProviderFactory(IOptions<WebPushConfig> options, IPushConfigurationStore pushConfigurationStore)
        {
            this.options = options;
            this.pushConfigurationStore = pushConfigurationStore;
        }
       
        public async Task<IPushProvider> CreateProvider(PushChannelConfiguration config)
        {
            return new WebPushProvider(options, config, await pushConfigurationStore.GetEndpointAsync(config.Id));
        }
    }
}