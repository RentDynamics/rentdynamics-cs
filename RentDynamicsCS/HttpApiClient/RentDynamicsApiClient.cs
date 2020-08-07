using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RentDynamicsCS.HttpApiClient
{
    public class RentDynamicsApiClient : IRentDynamicsApiClient
    {
        protected HttpClient HttpClient { get; }

        protected JsonMediaTypeFormatter JsonFormatter { get; }
        protected MediaTypeFormatter[] Formatters { get; }

        public RentDynamicsOptions Options { get; }
        
        public RentDynamicsApiClient(HttpClient httpClient, RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            HttpClient = httpClient;
            Options = options;

            JsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = jsonSerializerSettings ?? RentDynamicsDefaultSettings.DefaultSerializerSettings };
            Formatters = new MediaTypeFormatter[] { JsonFormatter };
        }

        public RentDynamicsApiClient(RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings = null)
            : this(RentDynamicsHttpClientFactory.Create(options, jsonSerializerSettings), options, jsonSerializerSettings)
        {
        }

        public virtual async Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token = default)
        {
            var response = await HttpClient.GetAsync(requestUri, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> PostAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(requestUri, data, JsonFormatter, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> PutAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default)
        {
            var response = await HttpClient.PutAsync(requestUri, data, JsonFormatter, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> DeleteAsync<TResult>(string requestUri, CancellationToken token = default)
        {
            var response = await HttpClient.DeleteAsync(requestUri, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }
    }
}