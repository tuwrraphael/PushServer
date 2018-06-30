using Microsoft.Extensions.DependencyInjection;
using System;

namespace PushServer.Configuration
{
    public interface IPushServerBuilder
    {
        IServiceCollection Services { get; }

        IPushServerBuilder AddAzureNotificationHub(Action<AzureNotificationHubConfig> configure);
    }
}
