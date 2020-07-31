using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RentDynamicsCS
{
    public static class NonceHelper
    {
        public static string GetNonce(string apiSecretKey, long unixTimestampMilliseconds, string url, string? data = "")
        {
            var nonceString = unixTimestampMilliseconds + url + data?.Replace(" ", string.Empty);
            var encoding = Encoding.UTF8;

            HMACSHA1 hmac = new HMACSHA1(encoding.GetBytes(apiSecretKey));

            byte[] buffer = hmac.ComputeHash(encoding.GetBytes(nonceString));

            return ByteToString(buffer);
        }

        private static string ByteToString(byte[] buffer)
        {
            return buffer.Aggregate(new StringBuilder(buffer.Length * 2),
                                    (current, t) =>
                                    {
                                        current.Append(t.ToString("X2"));
                                        return current;
                                    })
                         .ToString();
        }
    }
}