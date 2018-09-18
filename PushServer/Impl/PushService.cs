using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using PushServer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.Abstractions.Services;
using PushServer.Abstractions;

namespace PushServer.Impl
{
    public class PushService : IPushService
    {
        private readonly IPushConfigurationStore pushConfigurationStore;
        private readonly IEnumerable<IPushProviderFactory> pushProviderFactories;

        public PushService(IPushConfigurationStore pushConfigurationStore, IEnumerable<IPushProviderFactory> pushProviderFactories)
        {
            this.pushConfigurationStore = pushConfigurationStore;
            this.pushProviderFactories = pushProviderFactories;
        }

        private async Task<IPushProvider> CreateProvider(PushChannelConfiguration config)
        {
            var providerFactory = pushProviderFactories.SingleOrDefault(v => v.PushChannelType == config.ChannelType);
            if (null == providerFactory)
            {
                throw new NotSupportedException($"Provider for push type {config.ChannelType} was not configured.");
            }
            return (await providerFactory.CreateProvider(config));
        }

        public async Task Push(string configurationId, string payload, PushOptions options)
        {
            var config = await pushConfigurationStore.GetAsync(configurationId);
            await (await CreateProvider(config)).PushAsync(payload, options);
        }

        public async Task Push(string userId, IDictionary<string, string> configurationOptions, string payload, PushOptions options)
        {
            var config = await pushConfigurationStore.GetForOptionsAsync(userId, configurationOptions);
            if (null == config)
            {
                throw new PushConfigurationNotFoundException();

            }
    await (await CreateProvider(config)).PushAsync(payload, options);
        }
    }
}
