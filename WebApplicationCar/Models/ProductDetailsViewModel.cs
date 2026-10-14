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

        // 👇 اطلاعات ثبت‌کننده
        public int? FK_UserId { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerMobile { get; set; }

        public string OwnerDisplayName
        {
            get
            {
                var full = ((OwnerFirstName ?? "") + " " + (OwnerLastName ?? "")).Trim();
                if (!string.IsNullOrEmpty(full)) return full;
                if (!string.IsNullOrEmpty(OwnerUserName)) return OwnerUserName;
                return OwnerMobile;
            }
        }

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

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string NameFa { get; set; }
        public string CategoryType { get; set; }
    }
}