using JetBrains.Annotations;

namespace RentDynamics.RdClient
{
    [PublicAPI]
    public static class RentDynamicsApiClientExtensions
    {
        public static TResult Get<TResult>(this IRentDynamicsApiClient apiClient, string requestUri)
            => apiClient.GetAsync<TResult>(requestUri).GetAwaiter().GetResult();

        public static TResult Post<TRequest, TResult>(this IRentDynamicsApiClient apiClient, string requestUri, TRequest data)
            => apiClient.PostAsync<TRequest, TResult>(requestUri, data).GetAwaiter().GetResult();

        public static TResult Put<TRequest, TResult>(this IRentDynamicsApiClient apiClient, string requestUri, TRequest data)
            => apiClient.PutAsync<TRequest, TResult>(requestUri, data).GetAwaiter().GetResult();

        public static TResult Delete<TResult>(this IRentDynamicsApiClient apiClient, string requestUri)
            => apiClient.DeleteAsync<TResult>(requestUri).GetAwaiter().GetResult();

        public static void Delete(this IRentDynamicsApiClient apiClient, string requestUri)
            => apiClient.DeleteAsync(requestUri).GetAwaiter().GetResult();
    }
}