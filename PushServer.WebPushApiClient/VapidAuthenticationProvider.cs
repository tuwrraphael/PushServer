using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PushServer.WebPushApiClient
{
    public class VapidAuthenticationProvider : IVapidAuthenticationProvider
    {
        private readonly VapidAuthenticationOptions vapidAuthenticationOptions;

        public VapidAuthenticationProvider(IOptions<VapidAuthenticationOptions> vapidAuthenticationOptionsAccessor)
        {
            vapidAuthenticationOptions = vapidAuthenticationOptionsAccessor.Value;
        }

        private static string TransformBase64UrlEncoded(string str)
        {
            return $"{str.Replace('-', '+').Replace('_', '/')}{new String('=', (4 - str.Length % 4) % 4)}";
        }

        private ECDsa GetECDsa(string privateKey, string publicKey)
        {
            var privateBytes = Convert.FromBase64String(TransformBase64UrlEncoded(privateKey));
            Span<byte> publicBytes = Convert.FromBase64String(TransformBase64UrlEncoded(publicKey));
            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = privateBytes,
                Q = new ECPoint
                {
                    X = publicBytes.Slice(1, 32).ToArray(),
                    Y = publicBytes.Slice(33).ToArray()
                }
            });
        }

        public Task<string> GetVapidTokenAsync(Uri subscriptionUri)
        {
            var securityKey = new ECDsaSecurityKey(GetECDsa(vapidAuthenticationOptions.PrivateKey, vapidAuthenticationOptions.PublicKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);
            var claims = new[] {
                    new Claim(ClaimTypes.Name, vapidAuthenticationOptions.Subject)
                };
            var token = new JwtSecurityToken(
                new JwtHeader(signingCredentials), new JwtPayload(vapidAuthenticationOptions.Issuer,
                $"{subscriptionUri.Scheme}{Uri.SchemeDelimiter}{subscriptionUri.Host}",
                claims, null, DateTime.UtcNow.AddHours(12)));
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public string GetPublicKeyHeaderValue()
        {
            return $"p256ecdsa={vapidAuthenticationOptions.PublicKey}";
        }
    }
}
