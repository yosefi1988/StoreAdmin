using System;
using System.Security.Cryptography;


using System;

namespace WebApplicationCar.Services
{
    public class TokenPayload
    {
        public int UserId { get; set; }
        public string Mobile { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public interface ITokenService
    {
        string IssueToken(int userId, string mobile, TimeSpan lifetime);
        TokenPayload ValidateToken(string token);
        void RevokeToken(string token);
    }
}