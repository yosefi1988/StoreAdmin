using System.Linq;
using System.Web.Mvc;

namespace WebApplicationCar.Services
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var token = filterContext.HttpContext.Request.Cookies["auth_token"]?.Value;
            var tokenService = new SessionTokenService();
            var payload = tokenService.ValidateToken(token);

            if (payload == null)
            {
                // ====== تشخیص AJAX ======
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // کلاینت منتظر JSON هست، نه HTML صفحه لاگین
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
                    // درخواست عادی → ریدایرکت
                    filterContext.Result = new RedirectResult("/Login/Index");
                }
                return;
            }

            // ذخیره در Context برای استفاده در Controller
            filterContext.HttpContext.Items["UserId"] = payload.UserId;
            filterContext.HttpContext.Items["Mobile"] = payload.Mobile;
        }
    }
}