using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace RentDynamics.RdClient.Resources.Authentication
{
    [PublicAPI]
    public class LoginRequestVM
    {
        public string Username { get; }
        
        [JsonProperty("password")]
        public string PasswordHash { get; }

        public LoginRequestVM(string username, string password)
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