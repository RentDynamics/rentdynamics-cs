using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace RentDynamics.RdClient.HttpApiClient
{
    public class RentDynamicsHttpClientAuthenticationHandler : DelegatingHandler
    {
        private readonly RentDynamicsApiClientSettings _settings;
        private readonly INonceCalculator _nonceCalculator;

        private RentDynamicsOptions Options => _settings.Options!;

        public RentDynamicsHttpClientAuthenticationHandler(RentDynamicsApiClientSettings settings, INonceCalculator? nonceCalculator = null)
        {
            _settings = settings;
            _nonceCalculator = nonceCalculator ?? new NonceCalculator();
        }

        private static void ReplaceHeader(HttpHeaders headers, string headerName, string headerValue)
        {
            headers.Remove(headerName);
            headers.Add(headerName, headerValue);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            long unixTimestampMilliseconds = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;

            Stream? contentStream = request.Content != null ? await request.Content.ReadAsStreamAsync().ConfigureAwait(false) : null;
            
            // Because of retry-policy, this code may be invoked multiple times
            // We need to reset content stream so that it can be used again on the next retry attempts
            using var streamRewindScope = new StreamRewindScope(contentStream);
            
            //Do not dispose this object because it will close the underlying stream and break retry-policy
            StreamReader? contentReader = contentStream != null ? new StreamReader(contentStream) : null;

            string unescapedPathAndQuery = RdUriEscapeHelper.UnescapeSpecialRdApiCharacters(request.RequestUri.PathAndQuery);
            string nonce = await _nonceCalculator.GetNonceAsync(Options.ApiSecretKey, unixTimestampMilliseconds, unescapedPathAndQuery, contentReader).ConfigureAwait(false);

            //Because of retry policy, we must replace older header with new ones every time
            ReplaceHeader(request.Headers, RdHeaderNames.ApiKey, Options.ApiKey);
            ReplaceHeader(request.Headers, RdHeaderNames.Timestamp, unixTimestampMilliseconds.ToString());
            ReplaceHeader(request.Headers, RdHeaderNames.Nonce, nonce);

            var userAuthentication = Options.UserAuthentication;
            if (userAuthentication.IsAuthenticated)
            {
                ReplaceHeader(request.Headers, "Authorization", $"TOKEN {userAuthentication.AuthenticationToken}");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}