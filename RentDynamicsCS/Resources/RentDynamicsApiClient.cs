using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RentDynamicsCS.RestClient;

namespace RentDynamicsCS.Resources
{
    public class RentDynamicsApiClient
    {
        protected HttpClient HttpClient { get; }
        protected JsonSerializerSettings JsonSerializerSettings { get; }

        protected JsonMediaTypeFormatter JsonFormatter { get; }
        protected MediaTypeFormatter[] Formatters { get; }

        public RentDynamicsApiClient(HttpClient httpClient, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            HttpClient = httpClient;
            JsonSerializerSettings = jsonSerializerSettings ?? GetDefaultSerializerSettings();

            JsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = JsonSerializerSettings };
            Formatters = new MediaTypeFormatter[] { JsonFormatter };
        }

        public RentDynamicsApiClient(RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings = null)
            : this(CreateDefaultClient(options, jsonSerializerSettings), jsonSerializerSettings)
        {
        }

        private static HttpClient CreateDefaultClient(RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings)
        {
            return RentDynamicsHttpClientFactory.Create(options, jsonSerializerSettings ?? GetDefaultSerializerSettings());
        }

        private static JsonSerializerSettings GetDefaultSerializerSettings()
        {
            return RentDynamicsRestClientConfigurationHelper.DefaultSerializerSettings;
        }

        public virtual async Task<TResult> GetJsonAsync<TResult>(string requestUri, CancellationToken token = default)
        {
            var response = await HttpClient.GetAsync(requestUri, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> PostJsonAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(requestUri, data, JsonFormatter, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> PutJsonAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default)
        {
            var response = await HttpClient.PutAsync(requestUri, data, JsonFormatter, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }
        
        public virtual async Task<TResult> DeleteJsonAsync<TResult>(string requestUri, CancellationToken token = default)
        {
            var response = await HttpClient.DeleteAsync(requestUri, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }
    }
}