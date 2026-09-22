using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/ProductResourceViewModel.cs
    public class ProductResourceViewModel
    {
        // از X_Resources
        public int ResourceId { get; set; }
        public string NameFa { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool ResourceIsActive { get; set; }
        public DateTime ResourceCreatedAt { get; set; }

        // از X_Products
        public int? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public bool ProductIsActive { get; set; }
        public DateTime? ProductCreatedAt { get; set; }
        public DateTime? ProductUpdatedAt { get; set; }
    }
} 
