using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RentDynamics.RdClient
{
    public interface INonceCalculator
    {
        Task<string> GetNonceAsync(string apiSecretKey, long unixTimestampMilliseconds, string relativeUrl, TextReader? dataReader = null);
    }

    public class NonceCalculator : INonceCalculator
    {
        private static async Task<string> GetSortedJsonAsync(TextReader unsortedJsonReader)
        {
            using var reader = new JsonTextReader(unsortedJsonReader)
            {
                DateParseHandling = DateParseHandling.None //Prevent DateTime values from being converted to local timezone
            };

            JObject jObject = await JObject.LoadAsync(reader).ConfigureAwait(false);

            JsonSortHelper.Sort(jObject);

            return jObject.ToString(Formatting.None);
        }

        private static async Task<string?> PrepareBodyAsync(TextReader? dataReader)
        {
            if (dataReader == null) return null;

            string sortedJson = await GetSortedJsonAsync(dataReader).ConfigureAwait(false);

            return sortedJson.Replace(" ", string.Empty);
        }

        private static string ComputeHash(string apiSecretKey, string nonceString)
        {
            var encoding = Encoding.UTF8;

            HMACSHA1 hmac = new HMACSHA1(encoding.GetBytes(apiSecretKey));

            byte[] buffer = hmac.ComputeHash(encoding.GetBytes(nonceString));

            return buffer.ToHexString().ToLower();
        }

        public async Task<string> GetNonceAsync(string apiSecretKey, long unixTimestampMilliseconds, string relativeUrl, TextReader? dataReader = null)
        {
            string? body = await PrepareBodyAsync(dataReader).ConfigureAwait(false);
            string nonceString = unixTimestampMilliseconds + relativeUrl + body;

            return ComputeHash(apiSecretKey, nonceString);
        }
    }
}