//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplicationStoreAdmin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SD_ProductSizes
    {
        public int ID { get; set; }
        public int SizeID { get; set; }
        public int ProductChargePropertiesID { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    
        public virtual BD_SizeTypes BD_SizeTypes { get; set; }
        public virtual SD_ProductChargesProperties SD_ProductChargesProperties { get; set; }
    }
}
