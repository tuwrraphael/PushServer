using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace PushServer.WebPushApiClient
{

    public static class StringExtension
    {
        public static byte[] FromUrlSafeBase64String(this string str)
        {
            return Convert.FromBase64String($"{str.Replace('-', '+').Replace('_', '/')}{new String('=', (4 - str.Length % 4) % 4)}");
        }
    }

    public static class ByteArrayExtension
    {
        public static string ToUrlSafeBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    }
    public class EncryptionResult
    {
        public string PublicKey { get; set; }
        public byte[] Payload { get; set; }
        public string Salt { get; set; }
    }

    internal static class Encryptor
    {

        private class SubscriptionPublicKey : ECDiffieHellmanPublicKey
        {
            private readonly byte[] keyBlob;

            private static byte[] TransformBase64UrlEncoded(string str)
            {
                return Convert.FromBase64String($"{str.Replace('-', '+').Replace('_', '/')}{new String('=', (4 - str.Length % 4) % 4)}");
            }

            public SubscriptionPublicKey(string pd256h) : this(TransformBase64UrlEncoded(pd256h))
            {

            }

            public SubscriptionPublicKey(byte[] keyBlob) : base()
            {
                this.keyBlob = keyBlob;
            }
            public override ECParameters ExportParameters()
            {
                Span<byte> span = keyBlob;
                return new ECParameters()
                {
                    Curve = ECCurve.NamedCurves.nistP256,
                    D = null,
                    Q = new ECPoint()
                    {
                        X = span.Slice(1, 32).ToArray(),
                        Y = span.Slice(33).ToArray()
                    }
                };
            }
        }

        private const ushort PseudoRandomKeyLength = 32;
        private const ushort EncryptionKeyLength = 16;
        private const ushort EncryptionNonceLength = 12;

        public static EncryptionResult Encrypt(string subscriptionPD256H, string userSecret, string payload)
        {
            var sendEcdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            var sendPublicKey = sendEcdh.PublicKey;
            var subscriptionKey = new SubscriptionPublicKey(subscriptionPD256H);
            var derivedKey = sendEcdh.DeriveKeyFromHmac(subscriptionKey, HashAlgorithmName.SHA256, userSecret.FromUrlSafeBase64String());
            var pseudoRandomKey = HKDFSecondStep(derivedKey, Encoding.UTF8.GetBytes("Content-Encoding: auth\0"), PseudoRandomKeyLength);

            var salt = new byte[16];
            var generator = RandomNumberGenerator.Create();
            generator.GetBytes(salt, 0, 16);


            var encryptionKey = HKDF(salt, pseudoRandomKey, CreateInfoChunk("aesgcm", subscriptionKey, sendPublicKey), EncryptionKeyLength);
            var encryptionNonce = HKDF(salt, pseudoRandomKey, CreateInfoChunk("nonce", subscriptionKey, sendPublicKey), EncryptionNonceLength);

            var input = AddPaddingToInput(Encoding.UTF8.GetBytes(payload));
            var encryptedMessage = EncryptAes(encryptionNonce, encryptionKey, input);
            return new EncryptionResult
            {
                Salt = salt.ToUrlSafeBase64String(),
                Payload = encryptedMessage,
                PublicKey = sendPublicKey.ToByteArray().ToUrlSafeBase64String()
            };
        }

        private static byte[] AddPaddingToInput(byte[] data)
        {
            var input = new byte[0 + 2 + data.Length];
            Buffer.BlockCopy(ConvertInt(0), 0, input, 0, 2);
            Buffer.BlockCopy(data, 0, input, 0 + 2, data.Length);
            return input;
        }

        private static byte[] EncryptAes(byte[] nonce, byte[] cek, byte[] message)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(cek), 128, nonce);
            cipher.Init(true, parameters);

            //Generate Cipher Text With Auth Tag
            var cipherText = new byte[cipher.GetOutputSize(message.Length)];
            var len = cipher.ProcessBytes(message, 0, message.Length, cipherText, 0);
            cipher.DoFinal(cipherText, len);

            //byte[] tag = cipher.GetMac();
            return cipherText;
        }

        public static byte[] HKDFSecondStep(byte[] key, byte[] info, ushort length)
        {
            //var hmac = new HmacSha256(key);
            var hmac = new HMACSHA256(key);
            var infoAndOne = info.Concat(new byte[] { 0x01 }).ToArray();
            var result = hmac.ComputeHash(infoAndOne);

            if (result.Length > length)
            {
                Array.Resize(ref result, length);
            }
            return result;
        }

        public static byte[] HKDF(byte[] salt, byte[] prk, byte[] info, ushort length)
        {
            //var hmac = new HmacSha256(salt);
            var hmac2 = new HMACSHA256(salt);
            hmac2.ComputeHash(prk);
            var key = hmac2.ComputeHash(prk);
            //var key = hmac.ComputeHash(prk);

            return HKDFSecondStep(key, info, length);
        }

        public static byte[] ConvertInt(int number)
        {
            var output = BitConverter.GetBytes(Convert.ToUInt16(number));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(output);
            }
            return output;
        }

        public static byte[] CreateInfoChunk(string type, ECDiffieHellmanPublicKey subscriptionKey, ECDiffieHellmanPublicKey senderKey)
        {
            var output = new List<byte>();
            var exportedSubscriptionKey = subscriptionKey.ToByteArray();
            var exportedSenderKey = senderKey.ToByteArray();
            output.AddRange(Encoding.UTF8.GetBytes($"Content-Encoding: {type}\0P-256\0"));
            output.AddRange(ConvertInt(exportedSubscriptionKey.Length));
            output.AddRange(exportedSubscriptionKey);
            output.AddRange(ConvertInt(exportedSenderKey.Length));
            output.AddRange(exportedSenderKey);
            return output.ToArray();
        }
    }
}
