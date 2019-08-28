using Microsoft.Extensions.Options;
using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    internal class FirebasePushProviderFactory : IPushProviderFactory
    {
        private readonly IOptions<FirebaseConfig> options;
        private readonly IPushConfigurationStore pushConfigurationStore;
        private readonly IFirebaseHttpClient firebaseHttpClient;

        public FirebasePushProviderFactory(IOptions<FirebaseConfig> options, IPushConfigurationStore pushConfigurationStore,
            IFirebaseHttpClient firebaseHttpClient)
        {
            this.options = options;
            this.pushConfigurationStore = pushConfigurationStore;
            this.firebaseHttpClient = firebaseHttpClient;
        }

        public string PushChannelType => FirebaseConstants.ChannelType;

        public async Task<IPushProvider> CreateProvider(PushChannelConfiguration config)
        {
            return new FirebasePushProvider(options, firebaseHttpClient, config, await pushConfigurationStore.GetEndpointAsync(config.Id));
        }
    }
}
