using System;
using System.Collections.Generic;
#pragma warning disable CS0105 // The using directive for 'System' appeared previously in this namespace
using System;
#pragma warning restore CS0105 // The using directive for 'System' appeared previously in this namespace
#pragma warning disable CS0105 // The using directive for 'System.Collections.Generic' appeared previously in this namespace
using System.Collections.Generic;
#pragma warning restore CS0105 // The using directive for 'System.Collections.Generic' appeared previously in this namespace
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels.Wizard;
#pragma warning disable CS0105 // The using directive for 'System' appeared previously in this namespace
using System;
#pragma warning restore CS0105 // The using directive for 'System' appeared previously in this namespace
#pragma warning disable CS0105 // The using directive for 'System.Collections.Generic' appeared previously in this namespace
using System.Collections.Generic;
#pragma warning restore CS0105 // The using directive for 'System.Collections.Generic' appeared previously in this namespace



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
 