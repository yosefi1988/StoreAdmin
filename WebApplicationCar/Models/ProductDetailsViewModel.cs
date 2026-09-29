using System;
using System.Collections.Generic;

namespace WebApplicationCar.Models
{
    public class ProductDetailsViewModel
    {
        public int ProductId { get; set; }
        public int FK_ResourceId { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NameFa { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public List<ImageDto> Images { get; set; }
        public List<AttributeDto> Attributes { get; set; }
        public List<VariantDto> Variants { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }

    public class ImageDto
    {
        public int ResourceImageId { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public bool IsMain { get; set; }
    }

    public class AttributeDto
    {
        public int AttributeId { get; set; }
        public string NameFa { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
    }

    public class VariantDto
    {
        public int ProductVariantId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string NameFa { get; set; }
        public string CategoryType { get; set; }
    }
}