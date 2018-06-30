using Microsoft.Extensions.DependencyInjection;

namespace PushServer.Configuration
{
    public static class PushServerServiceCollectionExtensions
    {
        public static IPushServerBuilder AddPushServer(this IServiceCollection services)
        {
            return new PushServerBuilder()
            {
                Services = services
            };
        }
    }
}
