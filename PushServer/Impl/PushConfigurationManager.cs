using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
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

        public async Task<PushChannelConfiguration> RegisterAsync(string userId, PushChannelRegistration registration)
        {
            var config = await pushConfigurationStore.RegisterAsync(userId, registration);
            var provider = await pushProviderFactory.CreateProvider(config);
            await provider.InitializeAsync();
            return config;
        }

        public Task UpdateAsync(string userId, string configurationId, PushChannelRegistration registration)
        {
            return pushConfigurationStore.UpdateAsync(userId, configurationId, registration);
        }
    }
}
