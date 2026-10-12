using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace WebApplicationCar.Models
{
    public class VariantDto
    {
        public int ProductVariantId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Price { get; set; }

        public bool IsActive { get; set; }
        public List<VariantAttributeDto> Attributes { get; set; } =new List<VariantAttributeDto>();
    }
     
    public class VariantAttributeDto
    {
        public int ProductVariantAttributeId { get; set; }

        public int AttributeId { get; set; }
        public string AttributeNameFa { get; set; }
        public string AttributeNameEn { get; set; }
        public string AttributeCode { get; set; }
        public string DataType { get; set; }   // string / number / bool / date ...

        public int AttributeValueId { get; set; }
        public string ValueFa { get; set; }
        public string ValueEn { get; set; }
        public string ValueCode { get; set; }
    }
}