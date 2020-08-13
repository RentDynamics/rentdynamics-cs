using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace RentDynamics.RdClient.Models
{
    public class LoginRequest
    {
        public string Username { get; }
        
        [JsonProperty("password")]
        public string PasswordHash { get; }

        public LoginRequest(string username, string password)
        {
            Username = username;
            PasswordHash = GetPasswordHash(password);
        }

        private static string GetPasswordHash(string password)
        {
            using var sha1Managed = new SHA1Managed();
            byte[] passwordHash = sha1Managed.ComputeHash(Encoding.UTF8.GetBytes(password));
            return passwordHash.ToHexString().ToLower();
        }
    }
}