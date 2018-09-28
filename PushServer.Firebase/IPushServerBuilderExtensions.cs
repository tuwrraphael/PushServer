using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;
using PushServer.Abstractions.Services;
using System;

namespace PushServer.Firebase
{
    public static class IPushServerBuilderExtensions
    {
        public static IPushServerBuilder AddFirebase(this IPushServerBuilder pushServerBuilder, Action<FirebaseConfig> configure)
        {
            pushServerBuilder.Services.Configure(configure);
            pushServerBuilder.Services.AddTransient<IPushProviderFactory, FirebasePushProviderFactory>();
            return pushServerBuilder;
        }
    }
}
