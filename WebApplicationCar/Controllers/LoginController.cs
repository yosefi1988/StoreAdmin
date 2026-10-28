using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels;
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
            var token = Request.Cookies["auth_token"]?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var currentUser = _tokenService.ValidateToken(token);
                    if (currentUser != null)
                    {
                        // اگر کاربر لاگین است ولی فروشگاه فعال در Session نیست، از DB بازیابی کن
                        if (Session["ActiveStoreSettingId"] == null)
                        {
                            using (var db = new DataClassesDatabaseDataContextDataContext())
                            {
                                var user = db.X_Users.FirstOrDefault(u => u.UserId == currentUser.UserId);
                                if (user != null && user.FK_ActiveStoreSettingId.HasValue)
                                {
                                    var store = db.X_StoreSettings
                                        .FirstOrDefault(s => s.StoreSettingId == user.FK_ActiveStoreSettingId.Value
                                                          && s.IsActive);

                                    if (store != null)
                                    {
                                        Session["ActiveStoreSettingId"] = store.StoreSettingId;
                                        Session["ActiveStoreName"] = store.StoreNameFa;
                                    }
                                }
                            }
                        }

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
        // POST: ورود (سه فیلدی: موبایل + رمز + کد فروشگاه)
        // ============================================================
        [HttpPost]
        public JsonResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                // ===== اعتبارسنجی ورودی =====
                if (model == null
                    || string.IsNullOrWhiteSpace(model.Mobile)
                    || string.IsNullOrWhiteSpace(model.Password)
                    || string.IsNullOrWhiteSpace(model.StoreCode))
                    return Json(new { success = false, message = "همه فیلدها را پر کنید." });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    // ===== 1) پیدا کردن کاربر =====
                    var user = db.X_Users.FirstOrDefault(u => u.MobileNumber == model.Mobile);
                    if (user == null)
                        return Json(new { success = false, message = "کاربر یافت نشد." });

                    if (!user.IsActive)
                        return Json(new { success = false, message = "حساب غیرفعال است." });

                    if (string.IsNullOrEmpty(user.PasswordHash)
                        || !PasswordHasher.Verify(model.Password, user.PasswordHash))
                        return Json(new { success = false, message = "رمز عبور اشتباه است." });

                    // ===== 2) پیدا کردن فروشگاه با Code یا NameFa =====
                    var storeCode = model.StoreCode.Trim();
                    var store = db.X_StoreSettings.FirstOrDefault(s =>
                        (s.Code == storeCode || s.StoreNameFa == storeCode)
                        && s.IsActive);

                    if (store == null)
                        return Json(new { success = false, message = "فروشگاهی با این کد یافت نشد." });

                    // ===== 3) چک عضویت کاربر در فروشگاه =====
                    var membership = db.X_UserStoreSettings.FirstOrDefault(us =>
                        us.FK_UserId == user.UserId
                        && us.FK_StoreSettingId == store.StoreSettingId
                        && us.IsActive);

                    if (membership == null)
                        return Json(new { success = false, message = "شما عضو این فروشگاه نیستید." });

                    // ===== 4) صدور توکن =====
                    var lifetime = model.RememberMe
                        ? TimeSpan.FromDays(30)
                        : TimeSpan.FromHours(8);

                    var token = _tokenService.IssueToken(user.UserId, user.MobileNumber, lifetime);

                    // ===== 5) ذخیره در دیتابیس =====
                    user.LastLoginAt = DateTime.Now;
                    user.FK_ActiveStoreSettingId = store.StoreSettingId;  // ← فروشگاه فعال
                    db.SubmitChanges();

                    // ===== 6) Session =====
                    Session["UserId"] = user.UserId;
                    Session["ActiveStoreSettingId"] = store.StoreSettingId;
                    Session["ActiveStoreName"] = store.StoreNameFa;
                    Session["RoleInStore"] = membership.RoleInStore;

                    // ===== 7) Cookie =====
                    var cookie = new System.Web.HttpCookie("auth_token", token)
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.Add(lifetime),
                        Secure = Request.IsSecureConnection,
                        //SameSite = System.Web.SameSiteMode.Lax   // اگر .NET 4.7.2+
                    };
                    Response.Cookies.Add(cookie);

                    // ===== 8) مقصد =====
                    string redirectUrl = Url.Action("Feed", "Posts");
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        redirectUrl = returnUrl;

                    return Json(new
                    {
                        success = true,
                        message = "ورود موفق",
                        token = token,
                        userId = user.UserId,
                        storeSettingId = store.StoreSettingId,
                        storeName = store.StoreNameFa,
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
        // POST: تغییر فروشگاه فعال (اختیاری — از داشبورد)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangeStore(int storeSettingId)
        {
            try
            {
                var userId = (int)HttpContext.Items["UserId"];

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var membership = db.X_UserStoreSettings
                        .FirstOrDefault(us => us.FK_UserId == userId
                                            && us.FK_StoreSettingId == storeSettingId
                                            && us.IsActive);

                    if (membership == null)
                        return Json(new { success = false, message = "شما عضو این فروشگاه نیستید." });

                    var store = db.X_StoreSettings
                        .FirstOrDefault(s => s.StoreSettingId == storeSettingId && s.IsActive);

                    if (store == null)
                        return Json(new { success = false, message = "فروشگاه یافت نشد." });

                    // ذخیره در DB
                    var user = db.X_Users.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        user.FK_ActiveStoreSettingId = storeSettingId;
                        db.SubmitChanges();
                    }

                    // ذخیره در Session
                    Session["ActiveStoreSettingId"] = store.StoreSettingId;
                    Session["ActiveStoreName"] = store.StoreNameFa;
                    Session["RoleInStore"] = membership.RoleInStore;

                    return Json(new
                    {
                        success = true,
                        message = "فروشگاه فعال تغییر کرد.",
                        storeName = store.StoreNameFa
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
        [ValidateAntiForgeryToken]
        public JsonResult Logout()
        {
            // 1) پاک کردن Cookie
            if (Request.Cookies["auth_token"] != null)
            {
                Response.Cookies.Add(new System.Web.HttpCookie("auth_token", "")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    //SameSite = System.Web.SameSiteMode.Lax
                });
            }

            // 2) Revoke توکن
            try
            {
                var token = Request.Cookies["auth_token"]?.Value;
                if (!string.IsNullOrEmpty(token))
                    _tokenService.RevokeToken(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RevokeToken failed: " + ex.Message);
            }

            // 3) پاک کردن Session (UserId، ActiveStoreSettingId، ...)
            Session.Clear();

            return Json(new { success = true });
        }
    }
}