using System.Threading;
using System.Threading.Tasks;
using RentDynamicsCS.Models;

namespace RentDynamicsCS.Resources
{
    public interface IAuthenticationResource
    {
        Task<LoginResponse> LoginAsync(string username, string password, CancellationToken token = default);
        Task LogoutAsync(CancellationToken cancellationToken = default);
    }
}