using Microsoft.Extensions.DependencyInjection;

namespace PushServer.Abstractions.Configuration
{
    public interface IPushServerBuilder
    {
        IServiceCollection Services { get; }
    }
}
