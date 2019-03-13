using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.Abstractions.Services
{
    public interface IPushConfigurationManager
    {
        Task<PushChannelConfiguration> RegisterAsync(string userId, PushChannelRegistration registration);
        Task<PushChannelConfiguration[]> GetAllAsync(string userId);
        Task<bool> DeleteAsync(string userId, string configurationId);
        Task UpdateAsync(string userId, string configurationId, PushChannelRegistration registration);
        Task UpdateOptionsAsync(string userId, string configurationId, PushChannelOptions options);
    }
}
