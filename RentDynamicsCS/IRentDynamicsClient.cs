using System.Threading;
using System.Threading.Tasks;

namespace RentDynamicsCS
{
    public interface IRentDynamicsApiClient
    {
        RentDynamicsOptions Options { get; }

        Task<TResult> GetJsonAsync<TResult>(string url, CancellationToken token = default);
        Task<TResult> PostJsonAsync<TRequest, TResult>(string url, TRequest data, CancellationToken token = default);
        Task<TResult> PutJsonAsync<TRequest, TResult>(string url, TRequest data, CancellationToken token = default);
        Task<TResult> DeleteJsonAsync<TResult>(string url, CancellationToken token = default);
    }

    public static class RentDynamicsApiClientSyncMethodExtensions
    {
        public static TResult GetJson<TResult>(this IRentDynamicsApiClient apiClient, string url)
            => apiClient.GetJsonAsync<TResult>(url).GetAwaiter().GetResult();

        public static TResult PostJson<TRequest, TResult>(this IRentDynamicsApiClient apiClient, string url, TRequest data)
            => apiClient.PostJsonAsync<TRequest, TResult>(url, data).GetAwaiter().GetResult();

        public static TResult PutJson<TRequest, TResult>(this IRentDynamicsApiClient apiClient, string url, TRequest data)
            => apiClient.PutJsonAsync<TRequest, TResult>(url, data).GetAwaiter().GetResult();

        public static TResult DeleteJson<TResult>(this IRentDynamicsApiClient apiClient, string url)
            => apiClient.DeleteJsonAsync<TResult>(url).GetAwaiter().GetResult();
    }
}