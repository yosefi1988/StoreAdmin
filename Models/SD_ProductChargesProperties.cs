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
    
    public partial class SD_ProductChargesProperties
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SD_ProductChargesProperties()
        {
            this.SD_Images = new HashSet<SD_Images>();
            this.SD_ProductSizes = new HashSet<SD_ProductSizes>();
            this.SD_ShoppingBasketObjects = new HashSet<SD_ShoppingBasketObjects>();
            this.SD_Votes = new HashSet<SD_Votes>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ProductChargeID { get; set; }
        public Nullable<int> ColorID { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> RemainingCount { get; set; }
    
        public virtual SD_Color SD_Color { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_Images> SD_Images { get; set; }
        public virtual SD_ProductCharges SD_ProductCharges { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_ProductSizes> SD_ProductSizes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_ShoppingBasketObjects> SD_ShoppingBasketObjects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_Votes> SD_Votes { get; set; }
    }
}
