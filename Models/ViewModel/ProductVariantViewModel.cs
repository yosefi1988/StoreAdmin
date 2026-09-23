using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/ProductVariantViewModel.cs
    public class ProductVariantViewModel
    {
        public int ProductVariantId { get; set; }
        public int FK_ProductId { get; set; }

        // برای نمایش
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        // فیلدهای خود Variant
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; }
    }
} 
