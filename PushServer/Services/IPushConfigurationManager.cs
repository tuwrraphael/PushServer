using PushServer.PushConfiguration.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PushServer.Services
{
    public interface IPushConfigurationManager
    {
        Task<PushChannelConfiguration> RegisterAsync(string userId, AzureNotificationHubPushChannelRegistration registration);
        Task<PushChannelConfiguration> RegisterAsync(string userId, WebPushChannelRegistration registration);
        Task<PushChannelConfiguration[]> GetAllAsync(string userId);
        Task<bool> DeleteAsync(string userId, string configurationId);
        Task UpdateAsync(string userId, string configurationId, WebPushChannelRegistration registration);
        Task UpdateAsync(string userId, string configurationId, AzureNotificationHubPushChannelRegistration registration);
    }
}
