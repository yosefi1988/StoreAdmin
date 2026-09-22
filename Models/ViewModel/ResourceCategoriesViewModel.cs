using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models.ViewModel
{
    // Models/ViewModels/ResourceCategoriesViewModel.cs
    public class ResourceCategoriesViewModel
    {
        public int ResourceId { get; set; }
        public string ResourceNameFa { get; set; }
        public string ResourceNameEn { get; set; }
        public string ResourceImageUrl { get; set; }
        public bool ResourceIsActive { get; set; }

        // لیست دسته‌بندی‌های این Resource
        public List<int> SelectedCategoryIds { get; set; }
        public List<string> SelectedCategoryNames { get; set; }

        // برای Create/Edit
        public List<CategoryItem> AllCategories { get; set; }
    }

    public class CategoryItem
    {
        public int CategoryId { get; set; }
        public string NameFa { get; set; }
        public bool Selected { get; set; }
    }
} 
