using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/ProductResourceViewModel.cs
    public class ProductVariantDiscountViewModel
    {
        // Variant
        public int ProductVariantId { get; set; }
        public int FK_ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Price { get; set; }
        public bool VariantIsActive { get; set; }

        // Discount (اختیاری)
        public int? ProductVariantDiscountId { get; set; }
        public int? FK_ProductVariantId { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool DiscountIsActive { get; set; }
        public DateTime? DiscountCreatedAt { get; set; }

 
 

        // اطلاعات Discount
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
} 
