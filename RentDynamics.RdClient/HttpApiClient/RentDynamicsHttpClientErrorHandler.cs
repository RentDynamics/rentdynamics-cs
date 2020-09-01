using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RentDynamics.RdClient.HttpApiClient
{
    public class RentDynamicsHttpClientErrorHandler<TClientSettings> : DelegatingHandler
        where TClientSettings : IRentDynamicsApiClientSettings
    {
        private readonly TClientSettings _settings;
        private readonly ILogger<RentDynamicsHttpClientErrorHandler<TClientSettings>> _logger;

        public RentDynamicsHttpClientErrorHandler(TClientSettings settings, ILogger<RentDynamicsHttpClientErrorHandler<TClientSettings>> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        protected virtual bool ShouldTryReadResponseBody(HttpResponseMessage responseMessage)
        {
            HttpContent? content = responseMessage.Content;
            if (content == null) return false;

            long? contentLength = content.Headers.ContentLength;

            if (contentLength == null) return true;
            return contentLength <= 1024 * 30; //30 KB
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponseMessage = await base.SendAsync(request, cancellationToken);

            if (httpResponseMessage.IsSuccessStatusCode) return httpResponseMessage;

            string? responseBody = null;
            ApiError? apiError = null;

            if (ShouldTryReadResponseBody(httpResponseMessage))
            {
                responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                if (responseBody != null)
                {
                    try
                    {
                        apiError = JsonConvert.DeserializeObject<ApiError>(responseBody, _settings.JsonSerializerSettings);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Failed to deserialize error response body into ApiError. Body: {errorResponseBody}", responseBody);
                    }
                }
            }

            throw new RentDynamicsHttpRequestException("Response status code does not indicate success.", httpResponseMessage, responseBody, apiError);
        }
    }
}