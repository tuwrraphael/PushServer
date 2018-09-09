using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;
using PushServer.Abstractions.Services;
using PushServer.WebPushApiClient;
using System;

namespace PushServer.WebPush
{
    public static class IPushServerBuilderExtensions
    {
        public static IPushServerBuilder AddWebPush(this IPushServerBuilder pushServerBuilder, Action<VapidAuthenticationOptions> configure)
        {
            pushServerBuilder.Services.Configure(configure);
            pushServerBuilder.Services.AddHttpClient();
            pushServerBuilder.Services.AddTransient<IWebPushClient, WebPushClientWrapper>();
            pushServerBuilder.Services.AddTransient<IPushProviderFactory, WebPushProviderFactory>();
            return pushServerBuilder;
        }
    }
}
