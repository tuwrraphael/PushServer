using Microsoft.Extensions.DependencyInjection;
using PushServer.Abstractions.Configuration;

namespace PushServer.Configuration
{
    public class PushServerBuilder : IPushServerBuilder
    {
        public IServiceCollection Services { get; set; }
    }
}
