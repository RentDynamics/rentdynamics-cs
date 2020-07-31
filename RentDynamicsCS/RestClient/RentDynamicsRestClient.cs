using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace RentDynamicsCS.RestClient
{
    public class RentDynamicsRestClient : IRentDynamicsClient
    {
        protected RentDynamicsOptions Options { get; }
        protected RestSharp.RestClient RestClient { get; }

        public RentDynamicsRestClient(RentDynamicsOptions options, Action<RestSharp.RestClient> configureRestClient)
        {
            Options = options;
            RestClient = new RestSharp.RestClient(options.BaseUrl);
            configureRestClient(RestClient);
        }

        public RentDynamicsRestClient(RentDynamicsOptions options)
            : this(options, client => RentDynamicsRestClientConfigurationHelper.ConfigureClient(client, options))
        {
        }

        private static RestRequest CreateRequest(string endpoint) => new RestRequest(new Uri(endpoint, UriKind.Relative));

        public Task<TResult> GetJsonAsync<TResult>(string url, CancellationToken token = default)
            => RestClient.GetAsync<TResult>(CreateRequest(url), token);

        public TResult GetJson<TResult>(string url)
            => RestClient.Get<TResult>(CreateRequest(url)).Data;

        public Task<TResult> PostJsonAsync<TResult>(string url, object data, CancellationToken token = default)
            => RestClient.PostAsync<TResult>(CreateRequest(url).AddJsonBody(data), token);

        public TResult PostJson<TResult>(string url, object data)
            => RestClient.Post<TResult>(CreateRequest(url).AddJsonBody(data)).Data;

        public Task<TResult> PutJsonAsync<TResult>(string url, object data, CancellationToken token = default)
            => RestClient.PutAsync<TResult>(CreateRequest(url).AddJsonBody(data), token);

        public TResult PutJson<TResult>(string url, object data)
            => RestClient.Put<TResult>(CreateRequest(url).AddJsonBody(data)).Data;

        public Task<TResult> DeleteAsync<TResult>(string url, CancellationToken token)
            => RestClient.DeleteAsync<TResult>(CreateRequest(url), token);

        public TResult Delete<TResult>(string url)
            => RestClient.Delete<TResult>(CreateRequest(url)).Data;

        public async Task<string> LoginAsync(string username, string password, CancellationToken token = default)
        {
            //TODO: send password hash
            string authToken = await PostJsonAsync<string>("/auth/login/", new { username, password }, token);
            Options.AuthToken = authToken;
            return authToken;
        }

        public Task LogoutAsync(CancellationToken token = default)
            => PostJsonAsync<object>("/auth/logout", new { authToken = Options.AuthToken }, token);
    }
}