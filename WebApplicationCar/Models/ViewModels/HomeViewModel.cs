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
#pragma warning disable CS0105 // The using directive for 'WebApplicationCar.Models' appeared previously in this namespace
using WebApplicationCar.Models;
#pragma warning restore CS0105 // The using directive for 'WebApplicationCar.Models' appeared previously in this namespace

namespace WebApplicationCar.Models
{
    public class HomeViewModel
    {
        public StoreSettingVM Settings { get; set; }
        public List<CarItemVM> LatestCars { get; set; }
        public List<List<X_ContactMessage>> CommentsGrouped { get; set; }

    }

    public class StoreSettingVM
    {
        public string StoreNameFa { get; set; }
        public string StoreNameEn { get; set; }
        public string LogoUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string WorkingHours { get; set; }
        public string Description { get; set; }     // ← اضافه شد
        public string AbouteUs { get; set; }         // ← اضافه شد
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedInUrl { get; set; }
    }

    public class CarItemVM
    {
        public int ProductId { get; set; }
        public string NameFa { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}