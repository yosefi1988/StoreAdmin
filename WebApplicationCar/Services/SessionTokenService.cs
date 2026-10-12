using System;
using System.Linq;
using System.Security.Cryptography;
using WebApplicationCar.Models;

namespace WebApplicationCar.Services
{
    /// <summary>
    /// مدیریت توکن بر پایه x_tags (بدون JWT)
    /// فرمت: SESSION:<token>:<userId>:<mobile>:<expUnix>
    /// طول کل زیر 50 کاراکتر نگه داشته میشه
    /// </summary>
    public class SessionTokenService : ITokenService
    {
        public string IssueToken(int userId, string mobile, TimeSpan lifetime)
        {
            var token = GenerateToken();
            var expUnix = (long)(DateTime.UtcNow.Add(lifetime) - new DateTime(1970, 1, 1)).TotalSeconds;

            // کوتاه نگه دار: موبایل 11 + توکن 16 + userId ~5 + exp 10 ≈ 50
            // پس موبایل رو کوتاه میکنیم (بدون صفر اول)
            var shortMobile = mobile != null && mobile.Length > 10
                ? mobile.Substring(1)
                : mobile;

            var tagName = $"S:{token}:{userId}:{shortMobile}:{expUnix}";

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                db.x_tags.InsertOnSubmit(new x_tag { name = tagName });
                db.SubmitChanges();
            }

            return token;
        }

        public TokenPayload ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var tag = db.x_tags.FirstOrDefault(t => t.name.StartsWith("S:" + token + ":"));
                if (tag == null) return null;

                var parts = tag.name.Split(':');
                // parts: [0]=S, [1]=token, [2]=userId, [3]=mobile, [4]=expUnix
                if (parts.Length < 5) return null;

                var expUnix = long.Parse(parts[4]);
                var expiresAt = new DateTime(1970, 1, 1).AddSeconds(expUnix);

                if (DateTime.UtcNow > expiresAt)
                {
                    // منقضی → حذف
                    db.x_tags.DeleteOnSubmit(tag);
                    db.SubmitChanges();
                    return null;
                }

                return new TokenPayload
                {
                    UserId = int.Parse(parts[2]),
                    Mobile = parts[3],
                    ExpiresAt = expiresAt
                };
            }
        }

        public void RevokeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var tag = db.x_tags.FirstOrDefault(t => t.name.StartsWith("S:" + token + ":"));
                if (tag != null)
                {
                    db.x_tags.DeleteOnSubmit(tag);
                    db.SubmitChanges();
                }
            }
        }

        private string GenerateToken()
        {
            // 16 کاراکتر hex = 8 بایت
            var bytes = new byte[8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}