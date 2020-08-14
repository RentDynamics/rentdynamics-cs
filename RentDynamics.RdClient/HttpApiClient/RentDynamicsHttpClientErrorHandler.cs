using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RentDynamics.RdClient.Models;

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
            bool hasContentLengthHeader = responseMessage.Headers.TryGetValues("Content-Length", out var contentLengthHeaders);
            if (!hasContentLengthHeader) return true;

            bool readContentLength = long.TryParse(contentLengthHeaders.First(), out long contentLength);
            if (!readContentLength) return true;

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
                        apiError = JsonConvert.DeserializeObject<ApiError>(responseBody, _settings.JsonSerializerSettings ?? RentDynamicsDefaultSettings.DefaultSerializerSettings);
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