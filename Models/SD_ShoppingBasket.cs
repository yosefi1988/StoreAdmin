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
    
    public partial class SD_ShoppingBasket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SD_ShoppingBasket()
        {
            this.SD_ShoppingBasketObjects = new HashSet<SD_ShoppingBasketObjects>();
            this.SD_Transactions = new HashSet<SD_Transactions>();
            this.SD_SendBoxs = new HashSet<SD_SendBoxs>();
        }
    
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual SD_Users SD_Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_ShoppingBasketObjects> SD_ShoppingBasketObjects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_Transactions> SD_Transactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SD_SendBoxs> SD_SendBoxs { get; set; }
    }
}
