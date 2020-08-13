using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RentDynamics.RdClient
{
    public interface INonceCalculator
    {
        string GetNonce(string apiSecretKey, long unixTimestampMilliseconds, string relativeUrl, string? data = "");
    }

    public class NonceCalculator : INonceCalculator
    {
        private static string GetSortedJson(string unsortedJson)
        {
            using var reader = new JsonTextReader(new StringReader(unsortedJson))
            {
                DateParseHandling = DateParseHandling.None //Prevent DateTime values from being converted to local timezone
            };
            var jObject = JObject.Load(reader);

            JsonSortHelper.Sort(jObject);

            return jObject.ToString(Formatting.None);
        }

        private static string? PrepareBody(string? data)
        {
            if (data == null) return null;

            data = data.Replace(" ", string.Empty);

            data = GetSortedJson(data);

            return data;
        }

        public string GetNonce(string apiSecretKey, long unixTimestampMilliseconds, string relativeUrl, string? data = "")
        {
            string nonceString = unixTimestampMilliseconds + relativeUrl + PrepareBody(data);

            var encoding = Encoding.UTF8;

            HMACSHA1 hmac = new HMACSHA1(encoding.GetBytes(apiSecretKey));

            byte[] buffer = hmac.ComputeHash(encoding.GetBytes(nonceString));

            return buffer.ToHexString().ToLower();
        }
    }
}