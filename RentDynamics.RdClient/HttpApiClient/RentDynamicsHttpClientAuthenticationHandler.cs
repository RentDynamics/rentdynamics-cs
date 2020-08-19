using System;
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

        private static string UnescapeSpecialRdApiCharacters(string url)
        {
            //Pipe '|' character is treated specially in RD api. Details: https://github.com/Skylude/django-rest-framework-signature/blob/master/rest_framework_signature/authentication.py#L132
            return url.Replace("%7C", "|");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            long unixTimestampMilliseconds = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;

            string? requestContent = request.Content == null
                ? null
                : await request.Content.ReadAsStringAsync();

            string unescapedPathAndQuery = UnescapeSpecialRdApiCharacters(request.RequestUri.PathAndQuery);
            string nonce = _nonceCalculator.GetNonce(Options.ApiSecretKey, unixTimestampMilliseconds, unescapedPathAndQuery, requestContent);

            request.Headers.Add("x-rd-api-key", Options.ApiKey);
            request.Headers.Add("x-rd-timestamp", unixTimestampMilliseconds.ToString());
            request.Headers.Add("x-rd-api-nonce", nonce);

            var userAuthentication = Options.UserAuthentication;
            if (userAuthentication.IsAuthenticated)
            {
                request.Headers.Add("Authorization", $"TOKEN {userAuthentication.AuthenticationToken}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}