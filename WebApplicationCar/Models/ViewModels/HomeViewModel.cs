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

namespace WebApplicationCar.Models
{
    public class HomeViewModel
    {
        public StoreSettingVM Settings { get; set; }
        public List<CarItemVM> LatestCars { get; set; }
    }

    public class StoreSettingVM
    {
        public string StoreNameFa { get; set; }
        public string Address { get; set; }
        public string WorkingHours { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string LogoUrl { get; set; }
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