using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PushServer.Abstractions.Services;
using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public class FirebasePushProvider : IPushProvider
    {
        private readonly IFirebaseHttpClient firebaseHttpClient;
        private readonly PushChannelConfiguration config;
        private readonly PushEndpoint endpoint;
        private FirebaseConfig options;

        public FirebasePushProvider(IOptions<FirebaseConfig> optionsAccessor,
            IFirebaseHttpClient firebaseHttpClient,
            PushChannelConfiguration config,
            PushEndpoint endpoint)
        {
            options = optionsAccessor.Value;
            this.firebaseHttpClient = firebaseHttpClient;
            this.config = config;
            this.endpoint = endpoint;
        }

        public async Task InitializeAsync()
        {

        }

        public async Task PushAsync(string payload, PushOptions opts)
        {
            object objData = null;
            try
            {
                objData = JsonConvert.DeserializeObject(payload);
            }
            catch
            {

            }
            await firebaseHttpClient.Push(options.ServerKey, endpoint.Endpoint, objData ?? payload, opts?.Urgency);
        }
    }
}
