using PushServer.Models;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public interface IFirebaseHttpClient
    {
        Task Push(string serverKey, string[] registration_ids, object data, PushUrgency? urgency = PushUrgency.Normal);
    }
}
