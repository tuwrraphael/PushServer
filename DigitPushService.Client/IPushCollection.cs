namespace DigitPushService.Client
{
    public interface IPushCollection
    {
        IPushApi this[string userId] { get; }
    }
}