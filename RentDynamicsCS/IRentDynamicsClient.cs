using System.Threading;
using System.Threading.Tasks;

namespace RentDynamicsCS
{
    public interface IRentDynamicsClient
    {
        Task<TResult> GetJsonAsync<TResult>(string url, CancellationToken token = default);
        TResult GetJson<TResult>(string url);
        Task<TResult> PostJsonAsync<TResult>(string url, object data, CancellationToken token = default);
        TResult PostJson<TResult>(string url, object data);
        Task<TResult> PutJsonAsync<TResult>(string url, object data, CancellationToken token = default);
        TResult PutJson<TResult>(string url, object data);

        Task<string> LoginAsync(string username, string password, CancellationToken token = default);
        Task LogoutAsync(CancellationToken token = default);
    }
}