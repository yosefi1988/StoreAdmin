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
    
    public partial class BD_States
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BD_States()
        {
            this.BD_Cities = new HashSet<BD_Cities>();
            this.BD_SendProductsPrice = new HashSet<BD_SendProductsPrice>();
            this.BD_SendBoxPrices = new HashSet<BD_SendBoxPrices>();
        }
    
        public int ID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string Title { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BD_Cities> BD_Cities { get; set; }
        public virtual BD_Country BD_Country { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BD_SendProductsPrice> BD_SendProductsPrice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BD_SendBoxPrices> BD_SendBoxPrices { get; set; }
    }
}
