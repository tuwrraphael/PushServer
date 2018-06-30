using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.PushConfiguration.Abstractions.Services
{
    public interface IPushConfigurationStore
    {
        Task<PushChannelConfiguration> RegisterAsync(string userId, AzureNotificationHubPushChannelRegistration registration);
        Task<PushChannelConfiguration> RegisterAsync(string userId, WebPushChannelRegistration registration);
        Task<PushChannelConfiguration[]> GetAllAsync(string userId);
        Task<bool> DeleteAsync(string userId, string configurationId);
        Task UpdateAsync(string userId, string configurationId, WebPushChannelRegistration options);
        Task UpdateAsync(string userId, string configurationId, AzureNotificationHubPushChannelRegistration options);
    }
}
