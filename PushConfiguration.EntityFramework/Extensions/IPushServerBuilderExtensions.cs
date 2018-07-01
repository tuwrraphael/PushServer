using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;
using PushServer.PushConfiguration.Abstractions.Services;
using System;

namespace PushServer.PushConfiguration.EntityFramework.Extensions
{
    public static class IPushServerBuilderExtensions
    {
        public static IPushServerBuilder AddConfigurationStore(this IPushServerBuilder pushServerBuilder,
            Action<DbContextOptionsBuilder> ConfigureDbContext)
        {
            pushServerBuilder.Services.AddTransient<IPushConfigurationStore, PushConfigurationStore>();
            pushServerBuilder.Services.AddDbContext<ConfigurationDbContext>(ConfigureDbContext);
            return pushServerBuilder;
        }
    }
}
