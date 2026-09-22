using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/AttributeValueViewModel.cs
    public class AttributeValueViewModel
    {
        public int AttributeValueId { get; set; }
        public int FK_AttributeId { get; set; }
        public string AttributeName { get; set; }  // برای نمایش نام صفت
        public string ValueFa { get; set; }
        public string ValueEn { get; set; }
        public string Code { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 
