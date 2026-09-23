using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers
{
    public class Helper
    {
        //private List<CategorySelectItem> BuildCategoryTree(List<X_Category> all, int? parentId = null, string prefix = "")
        //{
        //    var result = new List<CategorySelectItem>();

        //    var children = all.Where(c => c.FK_ParentId == parentId).OrderBy(c => c.NameFa).ToList();

        //    foreach (var c in children)
        //    {
        //        result.Add(new CategorySelectItem
        //        {
        //            CategoryId = c.CategoryId,
        //            DisplayName = prefix + c.NameFa
        //        });

        //        // بازگشتی برای زیردسته‌ها
        //        result.AddRange(BuildCategoryTree(all, c.CategoryId, prefix + "── "));
        //    }

        //    return result;
        //}

        internal static List<CategorySelectItem> BuildCategoryTree(List<X_Category> all, int? parentId = null, string prefix = "")
        {
            var result = new List<CategorySelectItem>();

            var children = all.Where(c => c.FK_ParentId == parentId).OrderBy(c => c.NameFa).ToList();

            foreach (var c in children)
            {
                result.Add(new CategorySelectItem
                {
                    CategoryId = c.CategoryId,
                    DisplayName = prefix + c.NameFa
                });

                // بازگشتی برای زیردسته‌ها
                result.AddRange(BuildCategoryTree(all, c.CategoryId, prefix + "── "));
            }

            return result;
        }
    }
}
