using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/ProductVariantAttributeViewModel.cs
    public class ProductVariantAttributeViewModel
    {
        // اطلاعات خود X_ProductVariants
        public int ProductVariantId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public bool VariantIsActive { get; set; }

        // لیست Attributeهای این Variant
        public List<VariantAttributeRow> Attributes { get; set; }
    }

    public class VariantAttributeRow
    {
        public int ProductVariantAttributeId { get; set; }
        public int FK_AttributeId { get; set; }
        public string AttributeName { get; set; }
        public int FK_AttributeValueId { get; set; }
        public string AttributeValueName { get; set; }
    }
} 
