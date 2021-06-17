using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public class RentDynamicsApiClient : IRentDynamicsApiClient
    {
        protected HttpClient HttpClient { get; }

        protected JsonMediaTypeFormatter JsonFormatter { get; }
        protected MediaTypeFormatter[] Formatters { get; set; }

        public RentDynamicsOptions Options { get; }

        [UsedImplicitly]
        public RentDynamicsApiClient(HttpClient httpClient, RentDynamicsApiClientSettings settings)
        {
            HttpClient = httpClient;
            Options = settings.Options;

            JsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = settings.JsonSerializerSettings };
            Formatters = new MediaTypeFormatter[] { JsonFormatter };
        }

        private static void SetTransientRetryPolicyProperty(HttpRequestMessage request, bool useTransientRetryPolicy)
        {
            request.Properties[HttpRequestMessageProperties.UseTransientRetryPolicyForRequest] = useTransientRetryPolicy;
        }

        public virtual async Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token = default, bool useTransientRetryPolicy = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            SetTransientRetryPolicyProperty(request, useTransientRetryPolicy);
            
            var response = await HttpClient.SendAsync(request, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> PostAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default, bool useTransientRetryPolicy = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new ObjectContent<TRequest>(data, JsonFormatter)
            };
            SetTransientRetryPolicyProperty(request, useTransientRetryPolicy);
            
            var response = await HttpClient.SendAsync(request, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> PutAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default, bool useTransientRetryPolicy = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = new ObjectContent<TRequest>(data, JsonFormatter)
            };
            SetTransientRetryPolicyProperty(request, useTransientRetryPolicy);
            
            var response = await HttpClient.SendAsync(request, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual async Task<TResult> DeleteAsync<TResult>(string requestUri, CancellationToken token = default, bool useTransientRetryPolicy = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            SetTransientRetryPolicyProperty(request, useTransientRetryPolicy);
            
            var response = await HttpClient.SendAsync(request, token);
            return await response.Content.ReadAsAsync<TResult>(Formatters, token);
        }

        public virtual Task DeleteAsync(string requestUri, CancellationToken token = default, bool useTransientRetryPolicy = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            SetTransientRetryPolicyProperty(request, useTransientRetryPolicy);
            return HttpClient.SendAsync(request, token);
        }
    }
}