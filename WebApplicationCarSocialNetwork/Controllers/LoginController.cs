using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ITokenService _tokenService;

        public LoginController()
        {
            _tokenService = new SessionTokenService();
        }

        // ============================================================
        // GET: صفحه لاگین
        // ============================================================
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            // اگر کاربر قبلاً لاگین است، مستقیم به returnUrl یا Feed برود
            var token = Request.Cookies["auth_token"]?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var currentUser = _tokenService.ValidateToken(token);
                    if (currentUser != null)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        return RedirectToAction("Feed", "Posts");
                    }
                }
                catch { /* ignore */ }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ============================================================
        // POST: ورود
        // ============================================================
        [HttpPost]
        public JsonResult Login(LoginViewModel model, string returnUrl)
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

                    var lifetime = model.RememberMe
                        ? TimeSpan.FromDays(30)
                        : TimeSpan.FromHours(8);

                    var token = _tokenService.IssueToken(user.UserId, user.MobileNumber, lifetime);

                    user.LastLoginAt = DateTime.Now;
                    db.SubmitChanges();

                    var cookie = new System.Web.HttpCookie("auth_token", token)
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.Add(lifetime),
                        Secure = Request.IsSecureConnection,
                        //SameSite = System.Web.SameSiteMode.Lax   // ← اگر .NET 4.7.2+ هست
                    };
                    Response.Cookies.Add(cookie);

                    // 👇 تعیین مقصد
                    string redirectUrl = Url.Action("Feed", "Posts"); // پیش‌فرض
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        redirectUrl = returnUrl;

                    return Json(new
                    {
                        success = true,
                        message = "ورود موفق",
                        token = token,
                        userId = user.UserId,
                        redirectUrl = redirectUrl
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }

        // ============================================================
        // POST: خروج
        // ============================================================
        [HttpPost]
        public JsonResult Logout()
        {
            var token = Request.Cookies["auth_token"]?.Value;
            if (!string.IsNullOrEmpty(token))
                _tokenService.RevokeToken(token);

            if (Request.Cookies["auth_token"] != null)
            {
                Response.Cookies.Add(new System.Web.HttpCookie("auth_token", "")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection
                });
            }

            return Json(new { success = true });
        }
    }
}