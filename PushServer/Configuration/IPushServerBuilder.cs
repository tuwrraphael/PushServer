using Microsoft.Extensions.DependencyInjection;

namespace PushServer.Configuration
{
    public interface IPushServerBuilder
    {
        IServiceCollection Services { get; }
    }
}
