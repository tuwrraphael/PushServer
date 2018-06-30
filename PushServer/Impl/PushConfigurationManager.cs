using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using PushServer.Services;
using System.Threading.Tasks;

namespace PushServer.Impl
{
    internal class PushConfigurationManager : IPushConfigurationManager
    {
        private readonly IPushConfigurationStore pushConfigurationStore;
        private readonly IPushProviderFactory pushProviderFactory;

        public PushConfigurationManager(IPushConfigurationStore pushConfigurationStore, IPushProviderFactory pushProviderFactory)
        {
            this.pushConfigurationStore = pushConfigurationStore;
            this.pushProviderFactory = pushProviderFactory;
        }

        public Task<bool> DeleteAsync(string userId, string configurationId)
        {
            return pushConfigurationStore.DeleteAsync(userId, configurationId);
        }

        public Task<PushChannelConfiguration[]> GetAllAsync(string userId)
        {
            return pushConfigurationStore.GetAllAsync(userId);
        }

        public async Task<PushChannelConfiguration> RegisterAsync(string userId, AzureNotificationHubPushChannelRegistration registration)
        {
            var config = await pushConfigurationStore.RegisterAsync(userId, registration);
            var provider = await pushProviderFactory.CreateProvider(config);
            await provider.InitializeAsync();
            return config;
        }

        public Task<PushChannelConfiguration> RegisterAsync(string userId, WebPushChannelRegistration registration)
        {
            return pushConfigurationStore.RegisterAsync(userId, registration);
        }

        public Task UpdateAsync(string userId, string configurationId, WebPushChannelRegistration registration)
        {
            return pushConfigurationStore.UpdateAsync(userId, configurationId, registration);
        }

        public Task UpdateAsync(string userId, string configurationId, AzureNotificationHubPushChannelRegistration registration)
        {
            return pushConfigurationStore.UpdateAsync(userId, configurationId, registration);
        }
    }
}
