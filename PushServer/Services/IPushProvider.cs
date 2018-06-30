using PushServer.Models;
using System.Threading.Tasks;

namespace PushServer.Services
{
    public interface IPushProvider
    {
        Task InitializeAsync();
        Task PushAsync(string payload, PushOptions options);
    }
}
