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

        Task<TResult> GetAsync<TResult>(string url, CancellationToken token = default);
        Task<TResult> PostAsync<TRequest, TResult>(string url, TRequest data, CancellationToken token = default);
        Task<TResult> PutAsync<TRequest, TResult>(string url, TRequest data, CancellationToken token = default);
        Task<TResult> DeleteAsync<TResult>(string url, CancellationToken token = default);
        Task DeleteAsync(string uri, CancellationToken token = default);
    }

    public interface IRentDynamicsApiClient<[UsedImplicitly] TSettings> : IRentDynamicsApiClient
        where TSettings : IRentDynamicsApiClientSettings
    {
    }
}