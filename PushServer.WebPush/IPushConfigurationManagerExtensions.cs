using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PushServer.WebPush
{
    public static class IPushConfigurationManagerExtensions
    {
        private static PushChannelRegistration ToPushChannelRegistration(WebPushChannelRegistration registration)
        {
            return new PushChannelRegistration()
            {
                Endpoint = registration.Endpoint,
                EndpointInfo = registration.BrowserInfo,
                EndpointOptions = new Dictionary<string, string>() {
                    { "P256dhKey", registration.Keys.P256dh },
                    { "AuthKey", registration.Keys.Auth },
                },
                ExpirationTime = null, //TODO,
                Options = registration.Options,
                PushChannelType = WebPushConstants.ChannelType
            };
        }

        public static async Task<PushChannelConfiguration> RegisterAsync(this IPushConfigurationManager pushConfigurationManager, string userId, WebPushChannelRegistration registration)
        {
            return await pushConfigurationManager.RegisterAsync(userId, ToPushChannelRegistration(registration));
        }

        public static async Task UpdateAsync(this IPushConfigurationManager pushConfigurationManager, string userId, string configurationId, WebPushChannelRegistration registration)
        {
            await pushConfigurationManager.UpdateAsync(userId, configurationId, ToPushChannelRegistration(registration));
        }
    }
}
