using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RentDynamicsCS.HttpApiClient
{
    public class RentDynamicsHttpClientAuthenticationHandler : DelegatingHandler
    {
        private readonly RentDynamicsOptions _options;
        private readonly INonceCalculator _nonceCalculator;

        public RentDynamicsHttpClientAuthenticationHandler(RentDynamicsOptions options, INonceCalculator? nonceCalculator = null)
        {
            _options = options;
            _nonceCalculator = nonceCalculator ?? new NonceCalculator();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            long unixTimestampMilliseconds = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;

            string requestContent = await request.Content.ReadAsStringAsync();
            string nonce = _nonceCalculator.GetNonce(_options.ApiSecretKey, unixTimestampMilliseconds, request.RequestUri.AbsolutePath, requestContent);

            request.Headers.Add("x-rd-api-key", _options.ApiKey);
            request.Headers.Add("x-rd-timestamp", unixTimestampMilliseconds.ToString());
            request.Headers.Add("x-rd-api-nonce", nonce);

            //TODO: Is refresh token behavior required?
            if (_options.AuthToken != null)
            {
                request.Headers.Add("Authorization", $"TOKEN {_options.AuthToken}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}