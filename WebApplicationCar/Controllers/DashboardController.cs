using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AuthFilter]
    public class DashboardController : Controller
    {
        // ====== GET: /Dashboard ======
        public ActionResult Index()
        {
            var userId = (int)HttpContext.Items["UserId"];

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var user = db.X_Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return RedirectToAction("Index", "Login");

                var vm = new UserDashboardViewModel
                {
                    UserId = user.UserId,
                    Mobile = user.MobileNumber,
                    Email = user.Email,
                    LastLoginAt = user.LastLoginAt,

                    TotalOrders = db.X_Orders.Count(o => o.FK_UserId == userId),
                    TotalReviews = db.X_Reviews.Count(r => r.FK_UserId == userId),
                    MyResourcesCount = db.X_Resources.Count(r => r.FK_UserId == userId),

                    CartCount = db.X_Carts
                        .Where(c => c.FK_UserId == userId)
                        .SelectMany(c => c.X_CartItems)
                        .Count(),

                    WalletBalance = db.X_Wallets
                        .Where(w => w.FK_UserId == userId)
                        .Select(w => (decimal?)w.Balance)
                        .FirstOrDefault() ?? 0m
                };

                // نام نمایشی
                if (!string.IsNullOrWhiteSpace(user.FirstName))
                    vm.DisplayName = (user.FirstName + " " + user.LastName).Trim();
                else if (!string.IsNullOrWhiteSpace(user.UserName))
                    vm.DisplayName = user.UserName;
                else
                    vm.DisplayName = user.MobileNumber;

                // ====== نقش‌ها ======
                // لیست نقش‌های مجاز برای این کاربر (فقط نقش‌هایی که کاربر دارد و فعال هستند)
                vm.AllRoles = db.X_UserRoles
                    .Where(ur => ur.FK_UserId == userId && ur.X_Role.IsActive)
                    .Select(ur => new UserDashboardViewModel.RoleItem
                    {
                        RoleId = ur.FK_RoleId,
                        Name = ur.X_Role.Name
                    })
                    .ToList();

                // Dropdown فقط وقتی نمایش داده می‌شود که بیش از یک نقش باشد
                vm.CanChangeRole = vm.AllRoles.Count > 1;

                // ==== نقش فعال: اول از Session، بعد اولین نقش ====
                var activeFromSession = Session["ActiveRoleId"] as int?;
                if (activeFromSession.HasValue &&
                    vm.AllRoles.Any(r => r.RoleId == activeFromSession.Value))
                {
                    vm.CurrentRoleId = activeFromSession.Value;
                }
                else
                {
                    vm.CurrentRoleId = vm.AllRoles.FirstOrDefault()?.RoleId ?? 0;
                    Session["ActiveRoleId"] = vm.CurrentRoleId;
                }

                return View(vm);
            }
        }

        // ====== POST: /Dashboard/ChangeRole ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangeRole(int roleId)
        {
            try
            {
                var userId = (int)HttpContext.Items["UserId"];

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    // اعتبارسنجی: کاربر باید این نقش را داشته باشد
                    var hasRole = db.X_UserRoles
                        .Any(ur => ur.FK_UserId == userId
                                && ur.FK_RoleId == roleId
                                && ur.X_Role.IsActive);

                    if (!hasRole)
                        return Json(new { success = false, message = "این نقش برای شما مجاز نیست." });

                    // ذخیره نقش فعال در Session
                    Session["ActiveRoleId"] = roleId;

                    return Json(new { success = true, message = "نقش فعال با موفقیت تغییر کرد." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }
    }
}