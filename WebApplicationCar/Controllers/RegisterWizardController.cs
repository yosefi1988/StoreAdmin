using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels.Wizard;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    public class RegisterWizardController : Controller
    {
        private readonly IOtpSender _otpSender;
        private readonly OtpService _otpService;

        public RegisterWizardController()
        {
            // 🔑 تنها جایی که پیاده‌سازی انتخاب میشه
            _otpSender = new MockOtpSender();
            //_otpSender = new KavenegarSmsSender();

            _otpService = new OtpService();
        }

        public ActionResult Index() => View();

        // ========== قدم ۲: ارسال OTP ==========
        [HttpPost]
        public async Task<JsonResult> SendOtp(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < 10)
                return Json(new { success = false, message = "شماره موبایل نامعتبر" });

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                if (db.X_Users.Any(u => u.MobileNumber == mobile && u.IsActive))
                    return Json(new { success = false, message = "این شماره قبلاً ثبت‌نام کرده" });
            }

            var code = _otpService.GenerateCode();
            _otpService.Save(mobile, code);

            await _otpSender.SendAsync(mobile, code, OtpChannel.Sms);

            return Json(new { success = true, message = "کد ارسال شد" });
        }

        // ========== قدم ۲: تأیید OTP ==========
        [HttpPost]
        public JsonResult VerifyOtp(string mobile, string code)
        {
            var result = _otpService.Verify(mobile, code);

            switch (result)
            {
                case OtpVerifyResult.Success:
                    return Json(new { success = true });
                case OtpVerifyResult.Expired:
                    return Json(new { success = false, message = "کد منقضی شده" });
                case OtpVerifyResult.TooManyAttempts:
                    return Json(new { success = false, message = "تعداد تلاش بیش از حد" });
                default:
                    return Json(new { success = false, message = "کد نامعتبر" });
            }
        }

        // ========== قدم ۴: ثبت نهایی ==========
        [HttpPost]
        public JsonResult Save(RegisterWizardViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.MobileNumber))
                    return Json(new { success = false, message = "اطلاعات ناقص" });

                if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
                    return Json(new { success = false, message = "رمز حداقل ۶ کاراکتر" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    if (db.X_Users.Any(u => u.MobileNumber == model.MobileNumber && u.IsActive))
                        return Json(new { success = false, message = "قبلاً ثبت‌نام کرده" });

                    var user = new X_User
                    {
                        UserName = model.UserName ?? model.MobileNumber,
                        MobileNumber = model.MobileNumber,
                        Email = model.Email,
                        //PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        PasswordHash = PasswordHasher.Hash(model.Password), // ← اینجا
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Users.InsertOnSubmit(user);
                    db.SubmitChanges();

                    // نقش پیش‌فرض = User
                    db.X_UserRoles.InsertOnSubmit(new X_UserRole
                    {
                        FK_UserId = user.UserId,
                        FK_RoleId = 2
                    });
                    db.SubmitChanges();

                    return Json(new { success = true, userId = user.UserId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }
    }
}