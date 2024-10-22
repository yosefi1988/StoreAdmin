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
    
    public partial class SD_ProductCharges
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SD_ProductCharges()
        {
            this.SD_ProductChargesProperties = new HashSet<SD_ProductChargesProperties>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> SaleTaxID { get; set; }
        public string BuyInvoiceNumber { get; set; }
        public Nullable<System.DateTime> ChargeDate { get; set; }
        public Nullable<int> BuyPrice { get; set; }
        public Nullable<int> BuyCount { get; set; }
        public Nullable<int> PercentInterest { get; set; }
        public Nullable<int> PercentWages { get; set; }
        public string IDDDL { get; set; }
    
        public virtual BD_Tax BD_Tax { get; set; }
        public virtual SD_Product SD_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_ProductChargesProperties> SD_ProductChargesProperties { get; set; }
    }
}
