using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/CategoryViewModel.cs
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string NameFa { get; set; }
        public string NameEn { get; set; }
        public string ImageUrl { get; set; }
        public int? FK_ParentId { get; set; }
        public string ParentNameFa { get; set; }  // برای نمایش نام والد
        public bool IsActive { get; set; }
        public string CategoryType { get; set; }

        // برای Dropdown والد (با تورفتگی)
        public List<CategorySelectItem> AllCategories { get; set; }

        // تعداد زیردسته‌ها
        public int ChildCount { get; set; }
    }

    public class CategorySelectItem
    {
        public int CategoryId { get; set; }
        public string DisplayName { get; set; }  // با تورفتگی
    }
} 
