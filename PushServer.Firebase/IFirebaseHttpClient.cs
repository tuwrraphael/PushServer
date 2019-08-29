using PushServer.Models;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public interface IFirebaseHttpClient
    {
        Task Push(string serverKey, string to, object data, PushUrgency? urgency = PushUrgency.Normal);
    }
}
