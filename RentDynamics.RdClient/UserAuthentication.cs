namespace RentDynamics.RdClient
{
    public class UserAuthentication
    {
        public int? UserId { get; private set; }
        public string? AuthenticationToken { get; private set; }
        
        public bool IsAuthenticated { get; private set; }

        public void SetAuthentication(int userId, string authenticationToken)
        {
            IsAuthenticated = true;
            UserId = userId;
            AuthenticationToken = authenticationToken;
        }

        public void RemoveAuthentication()
        {
            IsAuthenticated = false;
            UserId = null;
            AuthenticationToken = null;
        }
    }
}