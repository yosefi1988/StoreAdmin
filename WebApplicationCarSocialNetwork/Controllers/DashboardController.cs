using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AuthFilter]  // ← فقط کاربر لاگین‌شده
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            var userId = (int)HttpContext.Items["UserId"];

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var user = db.X_Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    // کاربر حذف شده ولی توکنش مونده
                    return RedirectToAction("Index", "Login");
                }

                // ====== آمار ======
                ViewBag.TotalOrders = db.X_Orders.Count(o => o.FK_UserId == userId);
                ViewBag.TotalReviews = db.X_Reviews.Count(r => r.FK_UserId == userId);

                // آمار Resource هایی که خودم ثبت کردم
                ViewBag.MyResourcesCount = db.X_Resources
                    .Count(r => r.FK_UserId == userId);

                // سبد خرید فعال
                ViewBag.CartCount = db.X_Carts
                    .Where(c => c.FK_UserId == userId)// && c.IsActive)
                    .SelectMany(c => c.X_CartItems)
                    .Count();

                // کیف پول
                var wallet = db.X_Wallets.FirstOrDefault(w => w.FK_UserId == userId);
                ViewBag.WalletBalance = wallet != null ? wallet.Balance : 0m;

                // نام نمایشی
                if (!string.IsNullOrEmpty(user.FirstName))
                    ViewBag.DisplayName = (user.FirstName + " " + user.LastName).Trim();
                else if (!string.IsNullOrEmpty(user.UserName))
                    ViewBag.DisplayName = user.UserName;
                else
                    ViewBag.DisplayName = user.MobileNumber;

                ViewBag.Mobile = user.MobileNumber;
                ViewBag.Email = user.Email;
                ViewBag.LastLoginAt = user.LastLoginAt;

                return View(user);
            }
        }
    }
}