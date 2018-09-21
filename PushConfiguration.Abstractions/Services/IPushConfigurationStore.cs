using PushServer.PushConfiguration.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PushServer.PushConfiguration.Abstractions.Services
{
    public interface IPushConfigurationStore
    {
        Task<PushChannelConfiguration> RegisterAsync(string userId, PushChannelRegistration registration);
        Task<PushChannelConfiguration[]> GetAllAsync(string userId);
        Task<bool> DeleteAsync(string userId, string configurationId);
        Task UpdateAsync(string userId, string configurationId, PushChannelRegistration options);
        Task<PushChannelConfiguration> GetAsync(string configurationId);
        Task<PushEndpoint> GetEndpointAsync(string configurationId);
        Task<PushChannelConfiguration[]> GetForOptionsAsync(string userId, IDictionary<string, string> configurationOptions);
    }
}
