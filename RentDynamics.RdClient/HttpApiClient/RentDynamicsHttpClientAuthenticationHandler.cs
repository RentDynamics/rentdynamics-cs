using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RentDynamics.RdClient.HttpApiClient
{
    public class RentDynamicsHttpClientAuthenticationHandler<TClientSettings> : DelegatingHandler
        where TClientSettings : IRentDynamicsApiClientSettings
    {
        private readonly TClientSettings _settings;
        private readonly INonceCalculator _nonceCalculator;

        private RentDynamicsOptions Options => _settings.Options;

        public RentDynamicsHttpClientAuthenticationHandler(TClientSettings settings, INonceCalculator? nonceCalculator = null)
        {
            _settings = settings;
            _nonceCalculator = nonceCalculator ?? new NonceCalculator();
        }

        private static async Task<StringReader?> GetContentReaderAsync(HttpContent? content)
        {
            if (content == null) return null;

            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(false);
            return new StringReader(stringContent);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            long unixTimestampMilliseconds = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;

            using StringReader? contentReader = await GetContentReaderAsync(request.Content).ConfigureAwait(false); 
            string unescapedPathAndQuery = RdUriEscapeHelper.UnescapeSpecialRdApiCharacters(request.RequestUri.PathAndQuery);
            string nonce = await _nonceCalculator.GetNonceAsync(Options.ApiSecretKey, unixTimestampMilliseconds, unescapedPathAndQuery, contentReader).ConfigureAwait(false);

            request.Headers.Add("x-rd-api-key", Options.ApiKey);
            request.Headers.Add("x-rd-timestamp", unixTimestampMilliseconds.ToString());
            request.Headers.Add("x-rd-api-nonce", nonce);

            var userAuthentication = Options.UserAuthentication;
            if (userAuthentication.IsAuthenticated)
            {
                request.Headers.Add("Authorization", $"TOKEN {userAuthentication.AuthenticationToken}");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}