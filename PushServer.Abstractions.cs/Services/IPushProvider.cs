using PushServer.Models;
using System.Threading.Tasks;

namespace PushServer.Abstractions.Services
{
    public interface IPushProvider
    {
        Task InitializeAsync();
        Task PushAsync(string payload, PushOptions options);
    }
}
