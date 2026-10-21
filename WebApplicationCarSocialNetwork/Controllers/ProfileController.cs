using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Filters;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [TokenAuthorize]
    public class ProfileController : Controller
    {
        // ============ نمایش پروفایل ============
        public ActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Index", "Login");

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var user = db.X_Users.FirstOrDefault(u => u.UserId == userId.Value);
                if (user == null) return HttpNotFound();

                var model = new ProfileViewModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    MobileNumber = user.MobileNumber,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImageUrl = user.ProfileImageUrl,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt
                };

                return View(model);
            }
        }

        // ============ ذخیره ویرایش ============
        [HttpPost]
        public JsonResult Update(EditProfileViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Json(new { success = false, message = "لطفاً وارد شوید" });

                if (model == null)
                    return Json(new { success = false, message = "اطلاعات ناقص" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var user = db.X_Users.FirstOrDefault(u => u.UserId == userId.Value);
                    if (user == null)
                        return Json(new { success = false, message = "کاربر یافت نشد" });

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.ProfileImageUrl = model.ProfileImageUrl;
                    user.UpdatedAt = DateTime.Now;

                    db.SubmitChanges();

                    return Json(new
                    {
                        success = true,
                        message = "اطلاعات با موفقیت ذخیره شد",
                        displayName = BuildDisplayName(user.FirstName, user.LastName, user.UserName, user.MobileNumber)
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }

        // ============ Helper ============
        private int? GetCurrentUserId()
        {
            return HttpContext.Items["UserId"] as int?;
        }

        private string BuildDisplayName(string first, string last, string userName, string mobile)
        {
            var full = ((first ?? "") + " " + (last ?? "")).Trim();
            if (!string.IsNullOrEmpty(full)) return full;
            if (!string.IsNullOrEmpty(userName)) return userName;
            return mobile;
        }
    }
}