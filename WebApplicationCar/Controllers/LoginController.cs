using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    public class LoginController : Controller
    {
        private readonly ITokenService _tokenService;

        public LoginController()
        {
            // 🔑 تنها جایی که پیاده‌سازی انتخاب میشه
            _tokenService = new SessionTokenService();
            //_tokenService = new SessionTokenService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Mobile)
                    || string.IsNullOrWhiteSpace(model.Password))
                    return Json(new { success = false, message = "اطلاعات ناقص" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var user = db.X_Users.FirstOrDefault(u => u.MobileNumber == model.Mobile);

                    if (user == null)
                        return Json(new { success = false, message = "کاربر یافت نشد" });

                    if (!user.IsActive)
                        return Json(new { success = false, message = "حساب غیرفعال است" });

                    if (string.IsNullOrEmpty(user.PasswordHash)
                        || !PasswordHasher.Verify(model.Password, user.PasswordHash))
                        return Json(new { success = false, message = "رمز عبور اشتباه" });

                    // مدت اعتبار
                    var lifetime = model.RememberMe
                        ? TimeSpan.FromDays(30)
                        : TimeSpan.FromHours(8);

                    var token = _tokenService.IssueToken(user.UserId, user.MobileNumber, lifetime);

                    // بروزرسانی LastLoginAt
                    user.LastLoginAt = DateTime.Now;
                    db.SubmitChanges();

                    // ست کردن کوکی (اختیاری)
                    Response.Cookies.Add(new System.Web.HttpCookie("auth_token", token)
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.Add(lifetime),
                        Secure = Request.IsSecureConnection
                    });

                    return Json(new
                    {
                        success = true,
                        message = "ورود موفق",
                        token = token,
                        userId = user.UserId
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Logout()
        {
            var token = Request.Cookies["auth_token"]?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _tokenService.RevokeToken(token);
            }

            if (Request.Cookies["auth_token"] != null)
            {
                var cookie = new System.Web.HttpCookie("auth_token", "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            return Json(new { success = true });
        }
    }
}