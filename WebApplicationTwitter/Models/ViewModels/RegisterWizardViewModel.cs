using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTwitter.Models;
using WebApplicationTwitter.Models.ViewModels.Wizard;
using System;
using System.Collections.Generic;

namespace WebApplicationTwitter.Models
{
    public class RegisterWizardViewModel
    {
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int StoreSettingId { get; set; }
    }
}