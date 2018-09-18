using DigitPushService.Models;
using System.Threading.Tasks;

namespace DigitPushService.Client
{
    public interface IPushApi
    {
        Task Create(PushRequest pushRequest);
    }
}