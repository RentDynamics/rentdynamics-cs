using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RentDynamics.RdClient
{
    [PublicAPI]
    public interface IRentDynamicsApiClient
    {
        RentDynamicsOptions Options { get; }

        Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token = default, bool useTransientRetryPolicy = true);
        Task<TResult> PostAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default, bool useTransientRetryPolicy = false);
        Task<TResult> PutAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default, bool useTransientRetryPolicy = true);
        Task<TResult> DeleteAsync<TResult>(string requestUri, CancellationToken token = default, bool useTransientRetryPolicy = false);
        Task DeleteAsync(string requestUri, CancellationToken token = default, bool useTransientRetryPolicy = false);
    }
}