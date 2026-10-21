using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AuthFilter]  // ← فقط کاربر لاگین‌شده
    public class ChangePasswordController : Controller
    {
        // ========== GET: فرم تغییر رمز ==========
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // ========== POST: ذخیره رمز جدید ==========
        [HttpPost]
        public JsonResult Save(ChangePasswordViewModel model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "اطلاعات ناقص" });

                if (model.NewPassword != model.ConfirmPassword)
                    return Json(new { success = false, message = "رمزها یکسان نیستند" });

                if (model.NewPassword.Length < 6)
                    return Json(new { success = false, message = "رمز حداقل ۶ کاراکتر" });

                var userId = (int)HttpContext.Items["UserId"];

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var user = db.X_Users.FirstOrDefault(u => u.UserId == userId);
                    if (user == null)
                        return Json(new { success = false, message = "کاربر یافت نشد" });

                    // بررسی رمز فعلی
                    if (string.IsNullOrEmpty(user.PasswordHash)
                        || !PasswordHasher.Verify(model.CurrentPassword, user.PasswordHash))
                        return Json(new { success = false, message = "رمز فعلی اشتباه است" });

                    // بررسی: رمز جدید با فعلی یکسان نباشه
                    if (PasswordHasher.Verify(model.NewPassword, user.PasswordHash))
                        return Json(new { success = false, message = "رمز جدید نباید با رمز فعلی یکسان باشد" });

                    // تغییر رمز
                    user.PasswordHash = PasswordHasher.Hash(model.NewPassword);
                    user.UpdatedAt = DateTime.Now;

                    // ⭐ حذف همه Sessionها (خروج از همه دستگاه‌ها)
                    var sessions = db.x_tags.Where(t =>
                        t.name.StartsWith("S:") &&
                        t.name.Contains(":" + userId + ":")).ToList();
                    if (sessions.Any())
                    {
                        db.x_tags.DeleteAllOnSubmit(sessions);
                    }

                    db.SubmitChanges();

                    return Json(new
                    {
                        success = true,
                        message = "رمز با موفقیت تغییر کرد. لطفاً دوباره وارد شوید."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }
    }
}