using System;
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
        internal static async Task<string?> GetSortedJsonAsync(TextReader unsortedJsonReader)
        {
            using var reader = new JsonTextReader(unsortedJsonReader)
            {
                CloseInput = false,                        //The underlying reader should not be closed as it may be needed by the logic outside of this call.
                DateParseHandling = DateParseHandling.None //Prevent DateTime values from being converted to local timezone,
            };

            JToken? jToken;
            try
            {
                jToken = await JToken.LoadAsync(reader).ConfigureAwait(false);
            }
            catch (JsonReaderException)
            {
                // Calling LoadAsync on a null or empty string will throw an exception.
                // This can happen when we call POST/PUT/DELETE with URL arg's and no payload
                return null;
            }

            JsonSortHelper.Sort(jToken);
            return jToken.ToString(Formatting.None);
        }

        private static async Task<string?> PrepareBodyAsync(TextReader? dataReader)
        {
            if (dataReader == null) return null;

            string? sortedJson = await GetSortedJsonAsync(dataReader).ConfigureAwait(false);
            string? sortedJsonWithoutWhitespace = sortedJson?.Replace(" ", string.Empty);

            return ConvertNullTypeBodyToNull(sortedJsonWithoutWhitespace);
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

        internal static string? ConvertNullTypeBodyToNull(string? body)
        {
            return body != "{}" ? body : null;
        }
    }
}