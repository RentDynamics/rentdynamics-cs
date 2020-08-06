using System.Security.Cryptography;
using System.Text;

namespace RentDynamicsCS
{
    public interface INonceCalculator
    {
        string GetNonce(string apiSecretKey, long unixTimestampMilliseconds, string relativeUrl, string? data = "");
    }

    public class NonceCalculator : INonceCalculator
    {
        public string GetNonce(string apiSecretKey, long unixTimestampMilliseconds, string relativeUrl, string? data = "")
        {
            string nonceString = unixTimestampMilliseconds + relativeUrl + data?.Replace(" ", string.Empty);

            var encoding = Encoding.UTF8;

            HMACSHA1 hmac = new HMACSHA1(encoding.GetBytes(apiSecretKey));

            byte[] buffer = hmac.ComputeHash(encoding.GetBytes(nonceString));

            return buffer.ToHexString().ToLower();
        }
    }
}