using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RentDynamics.Client.Models;

namespace RentDynamics.Client.HttpApiClient
{
    public class RentDynamicsHttpClientErrorHandler<TClientSettings> : DelegatingHandler
        where TClientSettings : IRentDynamicsApiClientSettings
    {
        private readonly TClientSettings _settings;

        public RentDynamicsHttpClientErrorHandler(TClientSettings settings)
        {
            _settings = settings;
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
                        Console.WriteLine(e); //TODO: Use logger
                    }
                }
            }

            throw new RentDynamicsHttpRequestException("Response status code does not indicate success.", httpResponseMessage, responseBody, apiError);
        }
    }
}