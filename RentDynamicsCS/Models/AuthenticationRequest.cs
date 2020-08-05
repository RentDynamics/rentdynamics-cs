using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace RentDynamicsCS.Models
{
    public class AuthenticationRequest
    {
        public string Username { get; }
        
        [JsonProperty("password")]
        public string PasswordHash { get; }

        public AuthenticationRequest(string username, string password)
        {
            Username = username;
            PasswordHash = GetPasswordHash(password);
        }

        private static string GetPasswordHash(string password)
        {
            using var sha1Managed = new SHA1Managed();
            byte[] passwordHash = sha1Managed.ComputeHash(Encoding.UTF8.GetBytes(password));
            return string.Join("", passwordHash.Select(b => b.ToString("X2"))).ToLower();
        }
    }
}