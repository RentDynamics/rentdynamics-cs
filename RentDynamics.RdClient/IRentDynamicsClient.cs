using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient
{
    [PublicAPI]
    public interface IRentDynamicsApiClient
    {
        RentDynamicsOptions Options { get; }

        Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token = default);
        Task<TResult> PostAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default);
        Task<TResult> PutAsync<TRequest, TResult>(string requestUri, TRequest data, CancellationToken token = default);
        Task<TResult> DeleteAsync<TResult>(string requestUri, CancellationToken token = default);
        Task DeleteAsync(string requestUri, CancellationToken token = default);
    }

    public interface IRentDynamicsApiClient<[UsedImplicitly] TSettings> : IRentDynamicsApiClient
        where TSettings : IRentDynamicsApiClientSettings
    {
    }
}