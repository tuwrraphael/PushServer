using Microsoft.Extensions.DependencyInjection;

namespace PushServer.Configuration
{
    public class PushServerBuilder : IPushServerBuilder
    {
        public IServiceCollection Services { get; set; }
    }
}
