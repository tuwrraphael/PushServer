using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.AzureNotificationHub
{
    public static class IPushConfigurationManagerExtensions
    {
        private static PushChannelRegistration ToPushChannelRegistration(AzureNotificationHubPushChannelRegistration registration)
        {
            return new PushChannelRegistration()
            {
                Endpoint = registration.Endpoint,
                EndpointInfo = registration.DeviceInfo,
                EndpointOptions = null,
                ExpirationTime = null,
                Options = registration.Options,
                PushChannelType = AzureNotificationHubConstants.ChannelType
            };
        }

        public static async Task<PushChannelConfiguration> RegisterAsync(this IPushConfigurationManager pushConfigurationManager, string userId, AzureNotificationHubPushChannelRegistration registration)
        {
            return await pushConfigurationManager.RegisterAsync(userId, ToPushChannelRegistration(registration));
        }

        public static async Task UpdateAsync(this IPushConfigurationManager pushConfigurationManager, string userId, string configurationId, AzureNotificationHubPushChannelRegistration registration)
        {
            await pushConfigurationManager.UpdateAsync(userId, configurationId, ToPushChannelRegistration(registration));
        }
    }
}
