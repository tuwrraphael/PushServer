using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.Abstractions.Services
{
    public interface IPushProviderFactory
    {
        string PushChannelType { get; }
        Task<IPushProvider> CreateProvider(PushChannelConfiguration config);
    }
}
