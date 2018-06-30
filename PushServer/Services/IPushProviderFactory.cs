using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace PushServer.Services
{
    public interface IPushProviderFactory
    {
        PushChannelType Type { get; }
        Task<IPushProvider> CreateProvider(PushChannelConfiguration config);
    }
}
