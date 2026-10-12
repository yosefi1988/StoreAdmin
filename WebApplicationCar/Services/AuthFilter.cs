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
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            // ذخیره در Context برای استفاده در Controller
            filterContext.HttpContext.Items["UserId"] = payload.UserId;
            filterContext.HttpContext.Items["Mobile"] = payload.Mobile;
        }
    }
}