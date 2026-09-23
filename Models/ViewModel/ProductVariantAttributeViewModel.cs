using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    public class ProductVariantAttributeViewModel
    {
        public int ProductVariantId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }   // ← اضافه شد
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public bool VariantIsActive { get; set; }

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
