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
    
    public partial class BD_SendProductsPrice
    {
        public int ID { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string Title { get; set; }
    
        public virtual BD_States BD_States { get; set; }
        public virtual SD_Product SD_Product { get; set; }
    }
}
