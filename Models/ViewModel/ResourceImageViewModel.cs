using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    public class ResourceImageViewModel
    {
        public int ResourceImageId { get; set; }
        public int FK_ResourceId { get; set; }
        public string ResourceNameFa { get; set; }
        public string ProductCode { get; set; }   // ← اضافه شد
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 
