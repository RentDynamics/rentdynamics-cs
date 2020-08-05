using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RentDynamicsCS
{
    public class RentDynamicsHttpClientAuthenticator : DelegatingHandler
    {
        private readonly RentDynamicsOptions _options;

        public RentDynamicsHttpClientAuthenticator(RentDynamicsOptions options)
        {
            _options = options;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            long unixTimestampMilliseconds = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;

            string requestContent = await request.Content.ReadAsStringAsync();
            string nonce = NonceHelper.GetNonce(_options.ApiSecretKey, unixTimestampMilliseconds, request.RequestUri.AbsolutePath, requestContent ?? "");

            request.Headers.Add("x-rd-api-key", _options.ApiKey);
            request.Headers.Add("x-rd-timestamp", unixTimestampMilliseconds.ToString());
            request.Headers.Add("x-rd-api-nonce", nonce.ToLower());

            if (_options.AuthToken != null)
            {
                request.Headers.Add("Authorization", $"TOKEN {_options.AuthToken}");
            }

            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

            return httpResponseMessage;
        }
    }
}