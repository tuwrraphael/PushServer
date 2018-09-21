namespace PushServer.WebPushApiClient
{
    public class VapidAuthenticationOptions
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Issuer { get; set; }
        public string Subject { get; set; }
    }
}
