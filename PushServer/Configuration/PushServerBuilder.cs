using Microsoft.Extensions.DependencyInjection;
using PushServer.Impl;
using PushServer.Services;
using System;

namespace PushServer.Configuration
{
    public class PushServerBuilder : IPushServerBuilder
    {
        public IServiceCollection Services { get; set; }

        public IPushServerBuilder AddAzureNotificationHub(Action<AzureNotificationHubConfig> configure)
        {
            Services.Configure(configure);
            Services.AddTransient<AzureNotificationHubPushProviderFactory, AzureNotificationHubPushProviderFactory>();
            return this;
        }
    }
}
