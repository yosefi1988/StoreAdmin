using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Models;

namespace WebApplicationCar.Controllers
{
    public class BaseController : Controller
    { 
        private readonly string _conn = ConfigurationManager.ConnectionStrings["balabar1_balabarkaranConnectionString"].ConnectionString;

        public StoreSettingVM GetSettings()
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT TOP 1 StoreNameFa, Address, WorkingHours, 
                        PhoneNumber, Email, MobileNumber, LogoUrl , FaxNumber,Description,
                        AbouteUs,
                        InstagramUrl,TwitterUrl,FacebookUrl,LinkedInUrl

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
                            LogoUrl = reader["LogoUrl"]?.ToString(),
                             
                            FaxNumber = reader["FaxNumber"]?.ToString(),
                            Description = reader["Description"]?.ToString(),

                            AbouteUs = reader["AbouteUs"]?.ToString(),
                            InstagramUrl = reader["InstagramUrl"]?.ToString(),
                            TwitterUrl = reader["TwitterUrl"]?.ToString(),
                            FacebookUrl = reader["FacebookUrl"]?.ToString(),
                            LinkedInUrl = reader["LinkedInUrl"]?.ToString(),

                        };
                    }
                }
            }
            return new StoreSettingVM();
        }

        // این متد قبل از اجرای هر Action اجرا میشه
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.StoreSettings = GetSettings();
            base.OnActionExecuting(filterContext);
        }
    }
}