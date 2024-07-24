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
    
    public partial class SD_SendBoxs
    {
        public int ID { get; set; }
        public Nullable<int> ShoppingBasketID { get; set; }
        public Nullable<int> TransactionID { get; set; }
        public Nullable<int> SendTypeID { get; set; }
        public string SendTypeTitle { get; set; }
        public Nullable<int> CityID { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<int> SendStatus { get; set; }
        public string SendTrackingNo { get; set; }
    
        public virtual BD_Cities BD_Cities { get; set; }
        public virtual BD_SendProductsPrice BD_SendProductsPrice { get; set; }
        public virtual SD_ShoppingBasket SD_ShoppingBasket { get; set; }
        public virtual SD_Transactions SD_Transactions { get; set; }
    }
}
