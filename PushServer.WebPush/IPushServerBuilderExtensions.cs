using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;
using PushServer.Abstractions.Services;
using System;

namespace PushServer.WebPush
{
    public static class IPushServerBuilderExtensions
    {
        public static IPushServerBuilder AddAzureNotificationHub(this IPushServerBuilder pushServerBuilder, Action<WebPushConfig> configure)
        {
            pushServerBuilder.Services.Configure(configure);
            pushServerBuilder.Services.AddTransient<IPushProviderFactory, WebPushProviderFactory>();
            return pushServerBuilder;
        }
    }
}
