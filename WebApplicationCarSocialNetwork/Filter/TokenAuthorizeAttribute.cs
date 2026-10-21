using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Services;

namespace WebApplicationCar.Filters
{
    public class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var token = httpContext.Request.Cookies["auth_token"]?.Value;
            if (string.IsNullOrEmpty(token)) return false;

            var svc = new SessionTokenService();
            var payload = svc.ValidateToken(token);
            if (payload == null) return false;

            httpContext.Items["UserId"] = payload.UserId;
            httpContext.Items["Mobile"] = payload.Mobile;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var req = filterContext.HttpContext.Request;

            if (req.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = "لطفاً وارد شوید", redirectToLogin = true },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return;
            }

            filterContext.Result = new RedirectResult(
                "~/Login?returnUrl=" + HttpUtility.UrlEncode(req.RawUrl));
        }
    }
}