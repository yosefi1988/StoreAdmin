﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class officia1_StoreEntities : DbContext
    {
        public officia1_StoreEntities()
            : base("name=officia1_StoreEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BD_Cities> BD_Cities { get; set; }
        public virtual DbSet<BD_Country> BD_Country { get; set; }
        public virtual DbSet<BD_SendProductsPrice> BD_SendProductsPrice { get; set; }
        public virtual DbSet<BD_SizeTypes> BD_SizeTypes { get; set; }
        public virtual DbSet<BD_States> BD_States { get; set; }
        public virtual DbSet<BD_Tax> BD_Tax { get; set; }
        public virtual DbSet<SC_Admins> SC_Admins { get; set; }
        public virtual DbSet<SC_StoreDetails> SC_StoreDetails { get; set; }
        public virtual DbSet<SD_Addresses> SD_Addresses { get; set; }
        public virtual DbSet<SD_Category> SD_Category { get; set; }
        public virtual DbSet<SD_Color> SD_Color { get; set; }
        public virtual DbSet<SD_Coupons> SD_Coupons { get; set; }
        public virtual DbSet<SD_Images> SD_Images { get; set; }
        public virtual DbSet<SD_Product> SD_Product { get; set; }
        public virtual DbSet<SD_ProductCharges> SD_ProductCharges { get; set; }
        public virtual DbSet<SD_ProductChargesProperties> SD_ProductChargesProperties { get; set; }
        public virtual DbSet<SD_ProductSizes> SD_ProductSizes { get; set; }
        public virtual DbSet<SD_SendBoxs> SD_SendBoxs { get; set; }
        public virtual DbSet<SD_ShoppingBasket> SD_ShoppingBasket { get; set; }
        public virtual DbSet<SD_ShoppingBasketObjects> SD_ShoppingBasketObjects { get; set; }
        public virtual DbSet<SD_Transactions> SD_Transactions { get; set; }
        public virtual DbSet<SD_Users> SD_Users { get; set; }
        public virtual DbSet<SD_Votes> SD_Votes { get; set; }
    }
}
