using Microsoft.Extensions.DependencyInjection;
using OAuthApiClient.Abstractions;
using System;
using System.Net.Http;

namespace DigitPushService.Client
{
    public static class DigitPushServiceClientServiceCollectionExtension

    {
        public static void AddDigitPushServiceClient(this IServiceCollection services,
            Uri baseUri,
            IAuthenticationProviderBuilder authenticationProviderBuilder)
        {
            var factory = authenticationProviderBuilder.GetFactory();
            services.AddHttpClient(nameof(DigitPushServiceClient), client =>
            {
                client.BaseAddress = baseUri;
            });
            services.AddTransient<IDigitPushServiceClient>(v =>
            {
                var client = v.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(DigitPushServiceClient));
                return new DigitPushServiceClient(factory(v), client);
            });
        }
    }
}
