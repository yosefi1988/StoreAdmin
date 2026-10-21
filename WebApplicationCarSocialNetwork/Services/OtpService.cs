using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

using System;
using System.Linq;
using WebApplicationCar.Models;

namespace WebApplicationCar.Services
{
    public class OtpService
    {
        private const int CodeLength = 6;
        private const int ExpireMinutes = 2;
        private const int MaxAttempts = 5;

        public string GenerateCode()
        {
            var rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }

        public void Save(string mobile, string code)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                // پاک کردن OTPهای قبلی همین موبایل
                var olds = db.x_tags.Where(t => t.name.StartsWith("OTP:" + mobile + ":")).ToList();
                if (olds.Any())
                {
                    db.x_tags.DeleteAllOnSubmit(olds);
                    db.SubmitChanges();
                }

                var expiry = DateTime.Now.AddMinutes(ExpireMinutes).Ticks;
                db.x_tags.InsertOnSubmit(new x_tag
                {
                    name = $"OTP:{mobile}:{code}:exp={expiry}:try=0"
                });
                db.SubmitChanges();
            }
        }

        public OtpVerifyResult Verify(string mobile, string code)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var tag = db.x_tags.FirstOrDefault(t => t.name.StartsWith($"OTP:{mobile}:{code}:"));
                if (tag == null) return OtpVerifyResult.Invalid;

                var parts = tag.name.Split(':');
                var expTicks = long.Parse(parts[3].Replace("exp=", ""));
                var tries = int.Parse(parts[4].Replace("try=", ""));

                if (DateTime.Now.Ticks > expTicks)
                {
                    db.x_tags.DeleteOnSubmit(tag);
                    db.SubmitChanges();
                    return OtpVerifyResult.Expired;
                }

                if (tries >= MaxAttempts)
                    return OtpVerifyResult.TooManyAttempts;

                // موفق → حذف
                db.x_tags.DeleteOnSubmit(tag);
                db.SubmitChanges();
                return OtpVerifyResult.Success;
            }
        }
    }

    public enum OtpVerifyResult
    {
        Success, Invalid, Expired, TooManyAttempts
    }
}