using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;
using PushServer.Abstractions.Services;
using PushServer.Impl;
using PushServer.Services;

namespace PushServer.Configuration
{
    public static class PushServerServiceCollectionExtensions
    {
        public static IPushServerBuilder AddPushServer(this IServiceCollection services)
        {
            services.AddTransient<IPushService, PushService>();
            services.AddTransient<IPushConfigurationManager, PushConfigurationManager>();
            return new PushServerBuilder()
            {
                Services = services
            };
        }
    }
}
