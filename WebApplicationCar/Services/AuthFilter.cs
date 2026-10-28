using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;

namespace WebApplicationCar.Services
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var token = httpContext.Request.Cookies["auth_token"]?.Value;
            var tokenService = new SessionTokenService();
            var payload = tokenService.ValidateToken(token);

            // ====== 1) احراز هویت ======
            if (payload == null)
            {
                if (httpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            success = false,
                            message = "لطفاً وارد شوید",
                            requireLogin = true
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Login/Index");
                }
                return;
            }

            // ذخیره در Context برای استفاده در Controller
            httpContext.Items["UserId"] = payload.UserId;
            httpContext.Items["Mobile"] = payload.Mobile;

            // ====== 2) چک فروشگاه فعال ======
            var sessionStoreId = httpContext.Session["ActiveStoreSettingId"] as int?;
            if (sessionStoreId.HasValue)
            {
                // Session معتبر است — کاری لازم نیست
                return;
            }

            // Session خالی است → از DB بازیابی کن
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var user = db.X_Users.FirstOrDefault(u => u.UserId == payload.UserId);

                if (user == null)
                {
                    // کاربر حذف شده ولی توکنش مانده
                    if (httpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult
                        {
                            Data = new { success = false, message = "کاربر یافت نشد", requireLogin = true },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/Login/Index");
                    }
                    return;
                }

                // آیا فروشگاه فعال در DB ذخیره شده؟
                if (user.FK_ActiveStoreSettingId.HasValue)
                {
                    var store = db.X_StoreSettings
                        .FirstOrDefault(s => s.StoreSettingId == user.FK_ActiveStoreSettingId.Value
                                          && s.IsActive);

                    if (store != null)
                    {
                        // بازیابی موفق → Session را پر کن
                        httpContext.Session["ActiveStoreSettingId"] = store.StoreSettingId;
                        httpContext.Session["ActiveStoreName"] = store.StoreNameFa;

                        // نقش کاربر در فروشگاه
                        var membership = db.X_UserStoreSettings
                            .FirstOrDefault(us => us.FK_UserId == user.UserId
                                                && us.FK_StoreSettingId == store.StoreSettingId
                                                && us.IsActive);
                        if (membership != null)
                        {
                            httpContext.Session["RoleInStore"] = membership.RoleInStore;
                        }

                        return; // OK
                    }
                }

                // فروشگاه فعال ندارد → دنبال فروشگاه اصلی بگرد
                var primary = db.X_UserStoreSettings
                    .Where(us => us.FK_UserId == user.UserId && us.IsActive)
                    .OrderByDescending(us => us.IsPrimary)
                    .FirstOrDefault();

                if (primary != null)
                {
                    var store = db.X_StoreSettings
                        .FirstOrDefault(s => s.StoreSettingId == primary.FK_StoreSettingId
                                          && s.IsActive);

                    if (store != null)
                    {
                        // ذخیره در DB
                        user.FK_ActiveStoreSettingId = store.StoreSettingId;
                        db.SubmitChanges();

                        // Session
                        httpContext.Session["ActiveStoreSettingId"] = store.StoreSettingId;
                        httpContext.Session["ActiveStoreName"] = store.StoreNameFa;
                        httpContext.Session["RoleInStore"] = primary.RoleInStore;

                        return; // OK
                    }
                }

                // هیچ فروشگاهی ندارد → برگرد به لاگین
                if (httpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            success = false,
                            message = "شما به هیچ فروشگاهی متصل نیستید. با مدیر تماس بگیرید.",
                            requireLogin = true
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Login/Index");
                }
            }
        }
    }
}