using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels.Wizard;
using System;
using System.Collections.Generic;



namespace WebApplicationCar.Models.ViewModels
{
        public class UserDashboardViewModel
        {
            // اطلاعات کاربر
            public int UserId { get; set; }
            public string DisplayName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public DateTime? LastLoginAt { get; set; }

            // آمار
            public int TotalOrders { get; set; }
            public int TotalReviews { get; set; }
            public int MyResourcesCount { get; set; }
            public int CartCount { get; set; }
            public decimal WalletBalance { get; set; }

            // نقش‌ها
            public List<RoleItem> AllRoles { get; set; } = new List<RoleItem>();
            public int CurrentRoleId { get; set; }
            public bool CanChangeRole { get; set; } // فقط اگر بیش از یک نقش مجاز باشد

            public class RoleItem
            {
                public int RoleId { get; set; }
                public string Name { get; set; }
            }
        }
    }
 