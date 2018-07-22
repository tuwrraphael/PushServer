using System;
using System.Threading.Tasks;

namespace PushServer.WebPushApiClient
{
    public interface IVapidAuthenticationProvider
    {
        Task<string> GetVapidTokenAsync(Uri subscriptionUri);
        string GetPublicKeyHeaderValue();
    }
}