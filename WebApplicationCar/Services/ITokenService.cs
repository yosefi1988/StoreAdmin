using System;
using System.Security.Cryptography;


#pragma warning disable CS0105 // The using directive for 'System' appeared previously in this namespace
using System;
#pragma warning restore CS0105 // The using directive for 'System' appeared previously in this namespace

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
        int? GetUserId(string token);
    }
}