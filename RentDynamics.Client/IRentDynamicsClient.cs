using System.Threading;
using System.Threading.Tasks;

namespace RentDynamics.Client
{
    public interface IRentDynamicsApiClient
    {
        RentDynamicsOptions Options { get; }

        Task<TResult> GetAsync<TResult>(string url, CancellationToken token = default);
        Task<TResult> PostAsync<TRequest, TResult>(string url, TRequest data, CancellationToken token = default);
        Task<TResult> PutAsync<TRequest, TResult>(string url, TRequest data, CancellationToken token = default);
        Task<TResult> DeleteAsync<TResult>(string url, CancellationToken token = default);
    }

    public static class RentDynamicsApiClientSyncMethodExtensions
    {
        public static TResult Get<TResult>(this IRentDynamicsApiClient apiClient, string url)
            => apiClient.GetAsync<TResult>(url).GetAwaiter().GetResult();

        public static TResult Post<TRequest, TResult>(this IRentDynamicsApiClient apiClient, string url, TRequest data)
            => apiClient.PostAsync<TRequest, TResult>(url, data).GetAwaiter().GetResult();

        public static TResult Put<TRequest, TResult>(this IRentDynamicsApiClient apiClient, string url, TRequest data)
            => apiClient.PutAsync<TRequest, TResult>(url, data).GetAwaiter().GetResult();

        public static TResult Delete<TResult>(this IRentDynamicsApiClient apiClient, string url)
            => apiClient.DeleteAsync<TResult>(url).GetAwaiter().GetResult();
    }
}