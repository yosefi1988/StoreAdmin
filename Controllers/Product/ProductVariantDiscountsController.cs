using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ProductVariantDiscountsController : Controller
    {
        // ============ INDEX ============
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into dj
                         from d in dj.DefaultIfEmpty()
                         orderby r.NameFa, v.SKU
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
                             FK_ProductVariantId = d != null ? (int?)d.FK_ProductVariantId : null,
                             DiscountType = d != null ? d.DiscountType : null,
                             DiscountValue = d != null ? (decimal?)d.DiscountValue : null,
                             StartDate = d != null ? d.StartDate : null,
                             EndDate = d != null ? d.EndDate : null,
                             DiscountIsActive = d != null ? d.IsActive : false,
                             DiscountCreatedAt = d != null ? (DateTime?)d.CreatedAt : null
                         }).ToList();

            return View(model);
        }

        // ============ DETAILS ============
        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into dj
                         from d in dj.DefaultIfEmpty()
                         where v.ProductVariantId == id
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
                             FK_ProductVariantId = d != null ? (int?)d.FK_ProductVariantId : null,
                             DiscountType = d != null ? d.DiscountType : null,
                             DiscountValue = d != null ? (decimal?)d.DiscountValue : null,
                             StartDate = d != null ? d.StartDate : null,
                             EndDate = d != null ? d.EndDate : null,
                             DiscountIsActive = d != null ? d.IsActive : false,
                             DiscountCreatedAt = d != null ? (DateTime?)d.CreatedAt : null
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductVariantId = BuildVariantSelectList(db);
            return View();
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductVariantDiscountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                // چک کن این Variant از قبل Discount نداره
                bool exists = db.X_ProductVariantDiscounts.Any(d => d.FK_ProductVariantId == model.FK_ProductVariantId);
                if (exists)
                {
                    ModelState.AddModelError("FK_ProductVariantId", "این Variant از قبل تخفیف دارد. از صفحه ویرایش استفاده کنید.");
                    ViewBag.FK_ProductVariantId = BuildVariantSelectList(db, model.FK_ProductVariantId);
                    return View(model);
                }

                var entity = new X_ProductVariantDiscount
                {
                    FK_ProductVariantId = (int) model.FK_ProductVariantId,
                    DiscountType = model.DiscountType,
                    DiscountValue = model.DiscountValue ?? 0,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsActive = model.DiscountIsActive,
                    CreatedAt = DateTime.Now
                };

                db.X_ProductVariantDiscounts.InsertOnSubmit(entity);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductVariantId = BuildVariantSelectList(db2, model.FK_ProductVariantId);
            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            // id اینجا ProductVariantId هست (نه DiscountId)
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into dj
                         from d in dj.DefaultIfEmpty()
                         where v.ProductVariantId == id
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
                             FK_ProductVariantId = d != null ? (int?)d.FK_ProductVariantId : null,
                             DiscountType = d != null ? d.DiscountType : null,
                             DiscountValue = d != null ? (decimal?)d.DiscountValue : null,
                             StartDate = d != null ? d.StartDate : null,
                             EndDate = d != null ? d.EndDate : null,
                             DiscountIsActive = d != null ? d.IsActive : false,
                             DiscountCreatedAt = d != null ? (DateTime?)d.CreatedAt : null
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();

            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductVariantDiscountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var discount = db.X_ProductVariantDiscounts.FirstOrDefault(d => d.FK_ProductVariantId == model.ProductVariantId);

                if (discount == null)
                {
                    // ساخت Discount جدید
                    discount = new X_ProductVariantDiscount
                    {
                        FK_ProductVariantId = model.ProductVariantId,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue ?? 0,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        IsActive = model.DiscountIsActive,
                        CreatedAt = DateTime.Now
                    };
                    db.X_ProductVariantDiscounts.InsertOnSubmit(discount);
                }
                else
                {
                    // آپدیت Discount موجود
                    discount.DiscountType = model.DiscountType;
                    discount.DiscountValue = model.DiscountValue ?? 0;
                    discount.StartDate = model.StartDate;
                    discount.EndDate = model.EndDate;
                    discount.IsActive = model.DiscountIsActive;
                }

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            // id = ProductVariantDiscountId
            var db = new DataClassesDatabaseDataContext();

            var model = (from d in db.X_ProductVariantDiscounts
                         join v in db.X_ProductVariants on d.FK_ProductVariantId equals v.ProductVariantId into vj
                         from v in vj.DefaultIfEmpty()
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         where d.ProductVariantDiscountId == id
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantDiscountId = d.ProductVariantDiscountId,
                             FK_ProductVariantId = d.FK_ProductVariantId,
                             ProductVariantId = v != null ? v.ProductVariantId : 0,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v != null ? v.SKU : null,
                             DiscountType = d.DiscountType,
                             DiscountValue = d.DiscountValue,
                             StartDate = d.StartDate,
                             EndDate = d.EndDate,
                             DiscountIsActive = d.IsActive,
                             DiscountCreatedAt = d.CreatedAt
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ DELETE (POST) ============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var entity = db.X_ProductVariantDiscounts.FirstOrDefault(x => x.ProductVariantDiscountId == id);
            if (entity != null)
            {
                db.X_ProductVariantDiscounts.DeleteOnSubmit(entity);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        // ============ Helper ============
        private List<SelectListItem> BuildVariantSelectList(DataClassesDatabaseDataContext db, int? selectedId = null)
        {
            var variants = (from v in db.X_ProductVariants
                            join p in db.X_Products on v.FK_ProductId equals p.ProductId
                            join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                            where r.IsActive
                            select new
                            {
                                v.ProductVariantId,
                                v.SKU,
                                ProductName = r.NameFa,
                                ProductCode = p.ProductCode,
                                CategoryName = db.X_ResourceCategories
                                    .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                    .Join(db.X_Categories,
                                          rc => rc.FK_CategoryId,
                                          c => c.CategoryId,
                                          (rc, c) => c.NameFa)
                                    .FirstOrDefault() ?? "بدون دسته‌بندی"
                            }).ToList();

            var items = new List<SelectListItem>();
            foreach (var grp in variants.GroupBy(x => x.CategoryName).OrderBy(g => g.Key))
            {
                var group = new SelectListGroup { Name = grp.Key };
                foreach (var v in grp.OrderBy(x => x.ProductName).ThenBy(x => x.SKU))
                {
                    var text = v.ProductName;
                    if (!string.IsNullOrEmpty(v.ProductCode))
                        text += $" ({v.ProductCode})";
                    text += $" — SKU: {v.SKU}";

                    items.Add(new SelectListItem
                    {
                        Value = v.ProductVariantId.ToString(),
                        Text = text,
                        Group = group,
                        Selected = selectedId.HasValue && selectedId.Value == v.ProductVariantId
                    });
                }
            }

            return items;
        }
    }
}