using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
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


        private ECDsa TransformPrivateKey(string privateKey)
        {
            var key = Convert.FromBase64String(privateKey);
            var privKeyInt = new Org.BouncyCastle.Math.BigInteger(+1, key);
            var parameters = SecNamedCurves.GetByName("secp256r1");
            var ecPoint = parameters.G.Multiply(privKeyInt);
            var privKeyX = ecPoint.Normalize().XCoord.ToBigInteger().ToByteArrayUnsigned();
            var privKeyY = ecPoint.Normalize().YCoord.ToBigInteger().ToByteArrayUnsigned();
            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = privKeyInt.ToByteArrayUnsigned(),
                Q = new ECPoint
                {
                    X = privKeyX,
                    Y = privKeyY
                }
            });
        }

        public Task<string> GetVapidTokenAsync(Uri subscriptionUri)
        {
            var signingCredentials = new SigningCredentials(new ECDsaSecurityKey(TransformPrivateKey(vapidAuthenticationOptions.PrivateKey)), SecurityAlgorithms.EcdsaSha256);
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
