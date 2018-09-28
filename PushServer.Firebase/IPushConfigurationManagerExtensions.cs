using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public static class IPushConfigurationManagerExtensions
    {
        private static PushChannelRegistration ToPushChannelRegistration(FirebasePushChannelRegistration registration)
        {
            return new PushChannelRegistration()
            {
                Endpoint = registration.Token,
                EndpointInfo = registration.DeviceInfo,
                EndpointOptions = null,
                ExpirationTime = null,
                Options = registration.Options,
                PushChannelType = FirebaseConstants.ChannelType
            };
        }

        public static async Task<PushChannelConfiguration> RegisterAsync(this IPushConfigurationManager pushConfigurationManager, string userId, FirebasePushChannelRegistration registration)
        {
            return await pushConfigurationManager.RegisterAsync(userId, ToPushChannelRegistration(registration));
        }

        public static async Task UpdateAsync(this IPushConfigurationManager pushConfigurationManager, string userId, string configurationId, FirebasePushChannelRegistration registration)
        {
            await pushConfigurationManager.UpdateAsync(userId, configurationId, ToPushChannelRegistration(registration));
        }
    }
}
