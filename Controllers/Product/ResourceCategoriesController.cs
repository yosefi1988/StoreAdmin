using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ResourceCategoriesController : Controller
    {
        // ============ INDEX ============
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from r in db.X_Resources
                         select new ResourceCategoriesViewModel
                         {
                             ResourceId = r.ResourceId,
                             ResourceNameFa = r.NameFa,
                             ResourceNameEn = r.NameEn,
                             ResourceImageUrl = r.ImageUrl,
                             ResourceIsActive = r.IsActive,
                             SelectedCategoryIds = db.X_ResourceCategories
                                 .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                 .Select(rc => rc.FK_CategoryId)
                                 .ToList(),
                             SelectedCategoryNames = db.X_ResourceCategories
                                 .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                 .Join(db.X_Categories,
                                       rc => rc.FK_CategoryId,
                                       c => c.CategoryId,
                                       (rc, c) => c.NameFa)
                                 .ToList()
                         }).ToList();

            return View(model);
        }

        // ============ DETAILS ============
        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from r in db.X_Resources
                         where r.ResourceId == id
                         select new ResourceCategoriesViewModel
                         {
                             ResourceId = r.ResourceId,
                             ResourceNameFa = r.NameFa,
                             ResourceNameEn = r.NameEn,
                             ResourceImageUrl = r.ImageUrl,
                             ResourceIsActive = r.IsActive,
                             SelectedCategoryIds = db.X_ResourceCategories
                                 .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                 .Select(rc => rc.FK_CategoryId)
                                 .ToList(),
                             SelectedCategoryNames = db.X_ResourceCategories
                                 .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                 .Join(db.X_Categories,
                                       rc => rc.FK_CategoryId,
                                       c => c.CategoryId,
                                       (rc, c) => c.NameFa)
                                 .ToList()
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();

            // همه Resourceهای فعال با نام و کد محصول و دسته‌بندی
            var resources = (from r in db.X_Resources
                             where r.IsActive
                             join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into pj
                             from p in pj.DefaultIfEmpty()
                             select new
                             {
                                 r.ResourceId,
                                 r.NameFa,
                                 ProductCode = p != null ? p.ProductCode : null,
                                 CategoryName = db.X_ResourceCategories
                                     .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                     .Join(db.X_Categories,
                                           rc => rc.FK_CategoryId,
                                           c => c.CategoryId,
                                           (rc, c) => c.NameFa)
                                     .FirstOrDefault() ?? "بدون دسته‌بندی"
                             }).ToList();

            // گروه‌بندی بر اساس دسته‌بندی
            var items = new List<SelectListItem>();
            foreach (var grp in resources.GroupBy(x => x.CategoryName).OrderBy(g => g.Key))
            {
                var group = new SelectListGroup { Name = grp.Key };
                foreach (var r in grp.OrderBy(x => x.NameFa))
                {
                    var text = r.NameFa;
                    if (!string.IsNullOrEmpty(r.ProductCode))
                        text += $" ({r.ProductCode})";

                    items.Add(new SelectListItem
                    {
                        Value = r.ResourceId.ToString(),
                        Text = text,
                        Group = group
                    });
                }
            }

            ViewBag.ResourceId = items;

            // برای چک‌باکس‌های دسته‌بندی
            var model = new ResourceCategoriesViewModel
            {
                AllCategories = db.X_Categories
                    .Where(c => c.IsActive)
                    .Select(c => new CategoryItem
                    {
                        CategoryId = c.CategoryId,
                        NameFa = c.NameFa,
                        Selected = false
                    }).ToList()
            };

            return View(model);
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int ResourceId, List<int> SelectedCategoryIds)
        {
            if (SelectedCategoryIds == null || !SelectedCategoryIds.Any())
            {
                ModelState.AddModelError("", "حداقل یک دسته‌بندی انتخاب کنید.");
            }

            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                foreach (var catId in SelectedCategoryIds)
                {
                    // جلوگیری از تکرار
                    bool exists = db.X_ResourceCategories.Any(rc =>
                        rc.FK_ResourceId == ResourceId && rc.FK_CategoryId == catId);

                    if (!exists)
                    {
                        var rc = new X_ResourceCategory
                        {
                            FK_ResourceId = ResourceId,
                            FK_CategoryId = catId
                        };
                        db.X_ResourceCategories.InsertOnSubmit(rc);
                    }
                }

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            var model = new ResourceCategoriesViewModel
            {
                AllCategories = db2.X_Categories.Where(c => c.IsActive)
                    .Select(c => new CategoryItem
                    {
                        CategoryId = c.CategoryId,
                        NameFa = c.NameFa,
                        Selected = SelectedCategoryIds != null && SelectedCategoryIds.Contains(c.CategoryId)
                    }).ToList()
            };

            ViewBag.ResourceId = new SelectList(db2.X_Resources.Where(r => r.IsActive), "ResourceId", "NameFa", ResourceId);
            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var resource = db.X_Resources.FirstOrDefault(r => r.ResourceId == id);
            if (resource == null) return HttpNotFound();

            var selectedIds = db.X_ResourceCategories
                .Where(rc => rc.FK_ResourceId == id)
                .Select(rc => rc.FK_CategoryId)
                .ToList();

            var model = new ResourceCategoriesViewModel
            {
                ResourceId = resource.ResourceId,
                ResourceNameFa = resource.NameFa,
                ResourceNameEn = resource.NameEn,
                SelectedCategoryIds = selectedIds,
                AllCategories = db.X_Categories.Where(c => c.IsActive)
                    .Select(c => new CategoryItem
                    {
                        CategoryId = c.CategoryId,
                        NameFa = c.NameFa,
                        Selected = selectedIds.Contains(c.CategoryId)
                    }).ToList()
            };

            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int ResourceId, List<int> SelectedCategoryIds)
        {
            if (SelectedCategoryIds == null) SelectedCategoryIds = new List<int>();

            var db = new DataClassesDatabaseDataContext();

            // ۱. حذف دسته‌بندی‌هایی که دیگه انتخاب نشدن
            var current = db.X_ResourceCategories.Where(rc => rc.FK_ResourceId == ResourceId).ToList();
            var toDelete = current.Where(rc => !SelectedCategoryIds.Contains(rc.FK_CategoryId)).ToList();
            if (toDelete.Any())
            {
                db.X_ResourceCategories.DeleteAllOnSubmit(toDelete);
            }

            // ۲. اضافه کردن دسته‌بندی‌های جدید
            var currentIds = current.Select(rc => rc.FK_CategoryId).ToList();
            var toAdd = SelectedCategoryIds.Where(id => !currentIds.Contains(id)).ToList();

            foreach (var catId in toAdd)
            {
                db.X_ResourceCategories.InsertOnSubmit(new X_ResourceCategory
                {
                    FK_ResourceId = ResourceId,
                    FK_CategoryId = catId
                });
            }

            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var resource = db.X_Resources.FirstOrDefault(r => r.ResourceId == id);
            if (resource == null) return HttpNotFound();

            var model = new ResourceCategoriesViewModel
            {
                ResourceId = resource.ResourceId,
                ResourceNameFa = resource.NameFa,
                ResourceNameEn = resource.NameEn,
                ResourceImageUrl = resource.ImageUrl,
                ResourceIsActive = resource.IsActive,
                SelectedCategoryNames = db.X_ResourceCategories
                    .Where(rc => rc.FK_ResourceId == id)
                    .Join(db.X_Categories,
                          rc => rc.FK_CategoryId,
                          c => c.CategoryId,
                          (rc, c) => c.NameFa)
                    .ToList()
            };

            return View(model);
        }

        // ============ DELETE (POST) ============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            // حذف همه دسته‌بندی‌های این Resource
            var items = db.X_ResourceCategories.Where(rc => rc.FK_ResourceId == id).ToList();
            if (items.Any())
            {
                db.X_ResourceCategories.DeleteAllOnSubmit(items);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}