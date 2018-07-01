using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PushServer.Abstractions.Services;
using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Models;

namespace PushServer.WebPush
{
    internal class WebPushProvider : IPushProvider
    {
        private IOptions<WebPushConfig> options;
        private PushChannelConfiguration config;
        private PushEndpoint pushEndpoint;

        public WebPushProvider(IOptions<WebPushConfig> options, PushChannelConfiguration config, PushEndpoint pushEndpoint)
        {
            this.options = options;
            this.config = config;
            this.pushEndpoint = pushEndpoint;
        }

        public Task InitializeAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task PushAsync(string payload, PushOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}