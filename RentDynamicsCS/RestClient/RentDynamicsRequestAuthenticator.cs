using System;
using RestSharp;
using RestSharp.Authenticators;

namespace RentDynamicsCS.RestClient
{
    public class RentDynamicsRequestAuthenticator : IAuthenticator
    {
        private readonly RentDynamicsOptions _options;

        public RentDynamicsRequestAuthenticator(RentDynamicsOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public virtual void Authenticate(IRestClient client, IRestRequest request)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            long unixTimestampMilliseconds = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;
            string nonce = NonceHelper.GetNonce(_options.ApiSecretKey, unixTimestampMilliseconds, request.Resource, request.Body?.Value as string ?? "");

            request.AddHeader("x-rd-api-key", _options.ApiKey);
            request.AddHeader("x-rd-timestamp", unixTimestampMilliseconds.ToString());
            request.AddHeader("x-rd-api-nonce", nonce.ToLower());

            if (_options.AuthToken != null)
            {
                request.AddHeader("Authorization", $"TOKEN {_options.AuthToken}");
            }
        }
    }
}