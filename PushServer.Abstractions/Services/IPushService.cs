using System.Collections.Generic;
using System.Threading.Tasks;
using PushServer.Models;

namespace PushServer.Abstractions.Services
{
    public interface IPushService
    {
        Task Push(string configurationId, string payload, PushOptions options);
        Task Push(string userId, IDictionary<string, string> configurationOptions, string payload, PushOptions options);
    }
}
