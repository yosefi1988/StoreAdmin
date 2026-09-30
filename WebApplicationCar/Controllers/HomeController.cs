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

namespace WebApplicationCar.Controllers
{
    public class HomeController : Controller
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
                LatestCars = GetLatestCars(8)
            };

            return View(model);
        }

        private StoreSettingVM GetSettings()
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT TOP 1 StoreNameFa, Address, WorkingHours, 
                             PhoneNumber, Email, MobileNumber, LogoUrl
                      FROM X_StoreSettings 
                      WHERE IsActive = 1", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new StoreSettingVM
                        {
                            StoreNameFa = reader["StoreNameFa"]?.ToString(),
                            Address = reader["Address"]?.ToString(),
                            WorkingHours = reader["WorkingHours"]?.ToString(),
                            PhoneNumber = reader["PhoneNumber"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            MobileNumber = reader["MobileNumber"]?.ToString(),
                            LogoUrl = reader["LogoUrl"]?.ToString()
                        };
                    }
                }
            }
            return new StoreSettingVM();
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
    }
}

 