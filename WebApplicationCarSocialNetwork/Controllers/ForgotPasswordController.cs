using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IPasswordResetSender _resetSender;

        // مدت اعتبار لینک بازیابی
        private const int TokenLifetimeMinutes = 15;

        public ForgotPasswordController()
        {
            // 🔑 تنها جایی که پیاده‌سازی انتخاب میشه
            _resetSender = new MockPasswordResetSender();


            //_resetSender = new KavenegarPasswordResetSender(
            //    apiKey: "YOUR_API_KEY",
            //    senderLine: "10008686"
            //);

        }

        // ========== GET: فرم ورود موبایل ==========
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // ========== POST: درخواست بازیابی ==========
        [HttpPost]
        public async Task<JsonResult> RequestReset(ForgotPasswordViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Mobile))
                    return Json(new { success = false, message = "شماره موبایل را وارد کنید" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var user = db.X_Users.FirstOrDefault(u =>
                        u.MobileNumber == model.Mobile && u.IsActive);

                    // ⚠️ نکته امنیتی: همیشه پیام موفق بده
                    // (تا کسی نفهمه چه شماره‌هایی توی سیستم هستن)
                    if (user == null)
                    {
                        return Json(new
                        {
                            success = true,
                            message = "اگر این شماره در سیستم باشد، لینک بازیابی ارسال می‌شود"
                        });
                    }

                    // ساخت توکن امن
                    var token = GenerateToken();
                    var expiryUnix = (long)(DateTime.UtcNow
                        .AddMinutes(TokenLifetimeMinutes) - new DateTime(1970, 1, 1))
                        .TotalSeconds;

                    // ذخیره در x_tags
                    // فرمت کوتاه: PR:<token>:<userId>:<expUnix>
                    var tagName = $"PR:{token}:{user.UserId}:{expiryUnix}";

                    // پاک کردن توکن‌های قدیمی همین کاربر
                    var olds = db.x_tags.Where(t =>
                        t.name.StartsWith("PR:") &&
                        t.name.Contains(":" + user.UserId + ":")).ToList();
                    if (olds.Any())
                    {
                        db.x_tags.DeleteAllOnSubmit(olds);
                        db.SubmitChanges();
                    }

                    db.x_tags.InsertOnSubmit(new x_tag { name = tagName });
                    db.SubmitChanges();

                    // ====== ساخت لینک کامل ======
                    var resetLink = Url.Action(
                        "Reset",
                        "ForgotPassword",
                        new { token = token },
                        Request.Url.Scheme
                    );

                    // ارسال (Mock فعلاً)
                    await _resetSender.SendAsync(user.MobileNumber, resetLink, ResetChannel.Sms);

                    return Json(new
                    {
                        success = true,
                        message = "لینک بازیابی ارسال شد"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }

        // ========== GET: صفحه تعیین رمز جدید ==========
        [HttpGet]
        public ActionResult Reset(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Index");

            // اعتبارسنجی مقدماتی
            int userId;
            var valid = ValidateToken(token, out userId);


            if (!valid)
            {
                ViewBag.Error = "لینک بازیابی نامعتبر یا منقضی شده است";
                return View("Invalid");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        // ========== POST: ذخیره رمز جدید ==========
        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Token))
                    return Json(new { success = false, message = "توکن نامعتبر" });

                if (model.NewPassword != model.ConfirmPassword)
                    return Json(new { success = false, message = "رمزها یکسان نیستند" });

                if (model.NewPassword.Length < 6)
                    return Json(new { success = false, message = "رمز حداقل ۶ کاراکتر" });

                int userId;
                if (!ValidateToken(model.Token, out userId))
                    return Json(new { success = false, message = "لینک منقضی شده" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var user = db.X_Users.FirstOrDefault(u => u.UserId == userId);
                    if (user == null)
                        return Json(new { success = false, message = "کاربر یافت نشد" });

                    // تغییر رمز
                    user.PasswordHash = PasswordHasher.Hash(model.NewPassword);
                    user.UpdatedAt = DateTime.Now;

                    // حذف توکن‌های بازیابی این کاربر
                    var tags = db.x_tags.Where(t =>
                        t.name.StartsWith("PR:") &&
                        t.name.Contains(":" + userId + ":")).ToList();
                    if (tags.Any())
                    {
                        db.x_tags.DeleteAllOnSubmit(tags);
                    }

                    // ⭐ حذف Sessionهای فعال (کاربر از همه دستگاه‌ها خارج میشه)
                    var sessions = db.x_tags.Where(t =>
                        t.name.StartsWith("S:") &&
                        t.name.Contains(":" + userId + ":")).ToList();
                    if (sessions.Any())
                    {
                        db.x_tags.DeleteAllOnSubmit(sessions);
                    }

                    db.SubmitChanges();

                    return Json(new
                    {
                        success = true,
                        message = "رمز با موفقیت تغییر کرد"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }

        // ========== ابزارها ==========

        private string GenerateToken()
        {
            // 8 بایت = 16 کاراکتر hex
            var bytes = new byte[8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private bool ValidateToken(string token, out int userId)
        {
            userId = 0;
            if (string.IsNullOrWhiteSpace(token)) return false;

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var tag = db.x_tags.FirstOrDefault(t => t.name.StartsWith("PR:" + token + ":"));
                if (tag == null) return false;

                var parts = tag.name.Split(':');
                // PR:token:userId:expUnix
                if (parts.Length < 4) return false;

                if (!int.TryParse(parts[2], out userId)) return false;

                var expUnix = long.Parse(parts[3]);
                var expiresAt = new DateTime(1970, 1, 1).AddSeconds(expUnix);

                if (DateTime.UtcNow > expiresAt)
                {
                    db.x_tags.DeleteOnSubmit(tag);
                    db.SubmitChanges();
                    return false;
                }

                return true;
            }
        }
    }
}