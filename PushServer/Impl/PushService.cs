using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Services;
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
            try
            {
                await (await CreateProvider(config)).PushAsync(payload, options);
            }
            catch (Exception e)
            {
                throw new PushFailedException(config, e);
            }
        }

        private async Task PushWithProvider(PushChannelConfiguration config, string payload, PushOptions options)
        {
            var provider = await CreateProvider(config);
            await provider.PushAsync(payload, options);
        }

        public async Task Push(string userId, IDictionary<string, string> configurationOptions, string payload, PushOptions options)
        {
            var configs = await pushConfigurationStore.GetForOptionsAsync(userId, configurationOptions);
            if (null == configs || configs.Length == 0)
            {
                throw new PushConfigurationNotFoundException();
            }
            var tasks = configs.Select(config => new { config, PushTask = PushWithProvider(config, payload, options) });
            try
            {
                await Task.WhenAll(tasks.Select(v => v.PushTask));
            }
            catch (Exception)
            {
                var succeeded = tasks.Where(v => !v.PushTask.IsFaulted).Select(v => new
                        PushFailedException.PushResult
                { Configuration = v.config, Exception = null }).ToArray();
                var failed = tasks.Where(v => v.PushTask.IsFaulted).Select(v => new
                        PushFailedException.PushResult
                { Configuration = v.config, Exception = v.PushTask.Exception.InnerException }).ToArray();
                if (succeeded.Length > 0)
                {
                    throw new PushPartiallyFailedException(succeeded, failed);
                }
                throw new PushFailedException(failed);
            }
        }
    }
}
