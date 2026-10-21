using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationCar.Models.ViewModels.Wizard
{
    // Models/ViewModels/Wizard/ProductWizardViewModel.cs
    public class ProductWizardViewModel
    {
        // ========== قدم ۱ ==========
        public Step1ResourceProductVariant Step1 { get; set; }

        // ========== قدم ۲ ==========
        public Step2CategoriesAttributes Step2 { get; set; }

        // ========== قدم ۳ ==========
        public Step3ResourceAttributesImages Step3 { get; set; }
    }

    // ========== قدم ۱ ==========
    public class Step1ResourceProductVariant
    {
        // X_Resources
        public string NameFa { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool ResourceIsActive { get; set; } = true;

        // X_Products
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public bool ProductIsActive { get; set; } = true;

        // X_ProductVariants (چند تا)
        public List<VariantItem> Variants { get; set; } = new List<VariantItem>();
    }

    public class VariantItem
    {
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ========== قدم ۲ ==========
    public class Step2CategoriesAttributes
    {
        // X_ResourceCategories (چند تا)
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        // X_ProductVariantAttributes (چند تا، برای هر Variant)
        public List<VariantAttributesMap> VariantAttributes { get; set; } = new List<VariantAttributesMap>();


        public int? CountryId { get; set; }
        public int? ProvinceId { get; set; }
        public int? CityId { get; set; }

    }

    public class VariantAttributesMap
    {
        public int VariantIndex { get; set; }        // شماره‌ی Variant توی قدم ۱
        public List<AttributeValuePair> Attributes { get; set; } = new List<AttributeValuePair>();
    }

    public class AttributeValuePair
    {
        public int FK_AttributeId { get; set; }
        public int FK_AttributeValueId { get; set; }
    }

    // ========== قدم ۳ ==========
    public class Step3ResourceAttributesImages
    {
        // X_ResourceAttributes (چند تا)
        public List<ResourceAttributeItem> ResourceAttributes { get; set; } = new List<ResourceAttributeItem>();

        // X_ResourceImages (چند تا)
        public List<ResourceImageItem> Images { get; set; } = new List<ResourceImageItem>();
    }

    public class ResourceAttributeItem
    {
        public int FK_AttributeId { get; set; }
        public int? FK_AttributeValueId { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBoolean { get; set; }
        public string ValueText { get; set; }
    }

    public class ResourceImageItem
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; } = true;
    }
}