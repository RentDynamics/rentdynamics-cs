namespace RentDynamics.RdClient
{
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

        public static void Delete(this IRentDynamicsApiClient apiClient, string url)
            => apiClient.DeleteAsync(url).GetAwaiter().GetResult();
    }
}