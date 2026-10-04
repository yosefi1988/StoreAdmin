using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Web.Mvc;      // به جای Microsoft.AspNetCore.Mvc
using System.Data.Entity;  // به جای Microsoft.EntityFrameworkCore
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;


namespace WebApplicationCar.Controllers
{
    public class HomeController : BaseController
    {
        private readonly string _conn =
            ConfigurationManager.ConnectionStrings["balabar1_balabarkaranConnectionString"].ConnectionString;

 

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Settings = GetSettings(),
                //LatestCars = GetLatestCars(8),
                LatestCars = GetRandomCars(8),
                CommentsGrouped = GetActiveCommentsGrouped(2)   // ۲ تایی گروه‌بندی
            };

            return View(model);
        }

        private List<List<X_ContactMessage>> GetActiveCommentsGrouped(int perSlide)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var messages = db.X_ContactMessages
                                 .Where(x => x.IsActive)
                                 .OrderByDescending(x => x.CreatedAt)
                                 .ToList();

                var grouped = messages
                              .Select((m, i) => new { m, i })
                              .GroupBy(x => x.i / perSlide)
                              .Select(g => g.Select(x => x.m).ToList())
                              .ToList();

                return grouped;
            }
        }



        private StoreSettingVM GetSettings()
        {
            return base.GetSettings();
        }

        private List<CarItemVM> GetLatestCars(int count)
        {
            var list = new List<CarItemVM>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT TOP (@Count) 
                             p.ProductId, r.NameFa, r.ImageUrl, p.CreatedAt,
                             (SELECT TOP 1 v.Price 
                              FROM X_ProductVariants v 
                              WHERE v.FK_ProductId = p.ProductId AND v.IsActive = 1) AS Price
                      FROM X_Products p
                      INNER JOIN X_Resources r ON p.FK_ResourceId = r.ResourceId
                      WHERE p.IsActive = 1
                      ORDER BY p.CreatedAt DESC", conn);

                cmd.Parameters.AddWithValue("@Count", count);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CarItemVM
                        {
                            ProductId = (int)reader["ProductId"],
                            NameFa = reader["NameFa"]?.ToString(),
                            ImageUrl = reader["ImageUrl"]?.ToString(),
                            Price = reader["Price"] == DBNull.Value ? (decimal?)null : (decimal)reader["Price"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        });
                    }
                }
            }

            return list;
        }

        private List<CarItemVM> GetRandomCars(int count)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var all = db.X_Resources
                            .Where(x => x.IsActive)
                            .Select(x => new CarItemVM
                            {
                                ProductId = x.ResourceId,
                                NameFa = x.NameFa,
                                ImageUrl = x.ImageUrl,
                                Price = null,        // ← از X_Resources نمیاد
                        CreatedAt = x.CreatedAt
                            })
                            .ToList();

                return all.OrderBy(x => Guid.NewGuid())
                          .Take(count)
                          .ToList();
            }
        }

        [HttpPost]
        public ActionResult SendContactMessage(string Name, string Email, string PhoneNumber, string Massage)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return Json(new { success = false, message = "نام الزامی است" });

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    //var msg = new X_ContactMessage
                    //{
                    //    FullName = Name.Trim(),
                    //    Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                    //    PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim(),
                    //    Message = string.IsNullOrWhiteSpace(Massage) ? null : Massage.Trim(),
                    //    IsRead = false,
                    //    IsActive = true,
                    //    CreatedAt = DateTime.Now
                    //};

                    //db.X_ContactMessages.InsertOnSubmit(msg);  // ← برای LINQ to SQL درست‌تره
                    //db.SubmitChanges();                         // ← برای LINQ to SQL

                    var msg = new X_ContactMessage
                    {
                        FullName = Name.Trim(),
                        Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                        PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim(),
                        Message = string.IsNullOrWhiteSpace(Massage) ? null : Massage.Trim(),

                        // ستون‌های جدید
                        MessageType = "Contact",   // چون فرم تماس با ماست
                        Subject = null,
                        ImageUrl1 = null,
                        ImageUrl2 = null,

                        IsRead = false,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    db.X_ContactMessages.InsertOnSubmit(msg);
                    db.SubmitChanges();
                    return Json(new { success = true, message = "پیام شما با موفقیت ثبت شد" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در ثبت پیام: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SubscribeNewsletter(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return Json(new { success = false, message = "ایمیل معتبر وارد کنید" });

            email = email.Trim().ToLower();

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    // چک تکراری بودن
                    var exists = db.X_NewsletterSubscriptions
                                   .Any(x => x.Email == email && x.IsActive);

                    if (exists)
                        return Json(new { success = false, message = "این ایمیل قبلاً ثبت شده است" });

                    var sub = new X_NewsletterSubscription
                    {
                        Email = email,
                        IsConfirmed = false,
                        ConfirmedAt = null,
                        Token = Guid.NewGuid().ToString("N"),
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    db.X_NewsletterSubscriptions.InsertOnSubmit(sub);
                    db.SubmitChanges();

                    return Json(new { success = true, message = "عضویت شما با موفقیت ثبت شد" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در ثبت عضویت: " + ex.Message });
            }
        }

    }
}

 