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
    
    public partial class View_User_BasketsObjects
    {
        public int ID { get; set; }
        public int ShoppingBasketID { get; set; }
        public int ShoppingBasketObjectsID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProductCode { get; set; }
        public string Tax { get; set; }
        public Nullable<int> TaxPercentage { get; set; }
        public Nullable<int> PercentInterest { get; set; }
        public Nullable<int> PercentWages { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> RemainingCount { get; set; }
        public int ProductChargePropertiesID { get; set; }
        public Nullable<int> ProductChargeID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> OldPrice { get; set; }
        public Nullable<int> Count { get; set; }
        public Nullable<System.DateTime> AddInToBasketDate { get; set; }
        public Nullable<int> BasketStatusID { get; set; }
        public string BasketStatus { get; set; }
        public Nullable<int> TotalPriceWithTax { get; set; }
    }
}
