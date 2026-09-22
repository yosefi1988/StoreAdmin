using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/ProductResourceViewModel.cs
    public class ProductVariantDiscountViewModel
    {
        // از X_ProductVariants
        public int ProductVariantId { get; set; }
        public int FK_ProductId { get; set; }
        public string ProductName { get; set; } // برای نمایش نام محصول
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Price { get; set; }
        public bool VariantIsActive { get; set; }

        // از X_ProductVariantDiscounts
        public int? ProductVariantDiscountId { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool DiscountIsActive { get; set; }
        public DateTime? DiscountCreatedAt { get; set; }
    }
} 
