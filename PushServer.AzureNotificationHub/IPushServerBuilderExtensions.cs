using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;
using PushServer.Abstractions.Services;
using System;

namespace PushServer.AzureNotificationHub
{
    public static class IPushServerBuilderExtensions
    {
        public static IPushServerBuilder AddAzureNotificationHub(this IPushServerBuilder pushServerBuilder, Action<AzureNotificationHubConfig> configure)
        {
            pushServerBuilder.Services.Configure(configure);
            pushServerBuilder.Services.AddTransient<IPushProviderFactory, AzureNotificationHubPushProviderFactory>();
            return pushServerBuilder;
        }
    }
}
