using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public class RentDynamicsApiClient : RentDynamicsApiClient<RentDynamicsApiClientSettings>
    {
        public RentDynamicsApiClient(HttpClient httpClient, RentDynamicsApiClientSettings settings)
            : base(httpClient, settings)
        {
        }
    }

    [PublicAPI]
    public class RentDynamicsApiClient<TSettings> : IRentDynamicsApiClient<TSettings>
        where TSettings : IRentDynamicsApiClientSettings
    {
        protected HttpClient HttpClient { get; }

        protected JsonMediaTypeFormatter JsonFormatter { get; }
        protected MediaTypeFormatter[] Formatters { get; set; }

        public RentDynamicsOptions Options { get; }

        [UsedImplicitly]
        public RentDynamicsApiClient(HttpClient httpClient, TSettings settings)
        {
            HttpClient = httpClient;
            Options = settings.Options;

            JsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = settings.JsonSerializerSettings };
            Formatters = new MediaTypeFormatter[] { JsonFormatter };
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

        public virtual Task DeleteAsync(string requestUri, CancellationToken token = default)
        {
            return HttpClient.DeleteAsync(requestUri, token);
        }
    }
}