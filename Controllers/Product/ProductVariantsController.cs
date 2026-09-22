using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ProductVariantsController : Controller
    {
        // ============ INDEX ============
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into gj
                         from d in gj.DefaultIfEmpty()
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
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
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into gj
                         from d in gj.DefaultIfEmpty()
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         where v.ProductVariantId == id
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
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
            ViewBag.FK_ProductId = new SelectList(
                db.X_Products.Select(p => new {
                    p.ProductId,
                    Name = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameFa).FirstOrDefault()
                }).ToList(),
                "ProductId", "Name");

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

                // ۱. درج در X_ProductVariants
                var variant = new X_ProductVariant
                {
                    FK_ProductId = model.FK_ProductId,
                    SKU = model.SKU,
                    StockQuantity = model.StockQuantity,
                    Price = model.Price,
                    IsActive = model.VariantIsActive
                };
                db.X_ProductVariants.InsertOnSubmit(variant);
                db.SubmitChanges();

                // ۲. درج در X_ProductVariantDiscounts (اگه اطلاعات تخفیف داده شده)
                if (!string.IsNullOrWhiteSpace(model.DiscountType) && model.DiscountValue.HasValue)
                {
                    var discount = new X_ProductVariantDiscount
                    {
                        FK_ProductVariantId = variant.ProductVariantId,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue.Value,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        IsActive = model.DiscountIsActive,
                        CreatedAt = DateTime.Now
                    };
                    db.X_ProductVariantDiscounts.InsertOnSubmit(discount);
                    db.SubmitChanges();
                }

                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductId = new SelectList(
                db2.X_Products.Select(p => new {
                    p.ProductId,
                    Name = db2.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameFa).FirstOrDefault()
                }).ToList(),
                "ProductId", "Name", model.FK_ProductId);

            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into gj
                         from d in gj.DefaultIfEmpty()
                         where v.ProductVariantId == id
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
                             DiscountType = d != null ? d.DiscountType : null,
                             DiscountValue = d != null ? (decimal?)d.DiscountValue : null,
                             StartDate = d != null ? d.StartDate : null,
                             EndDate = d != null ? d.EndDate : null,
                             DiscountIsActive = d != null ? d.IsActive : false,
                             DiscountCreatedAt = d != null ? (DateTime?)d.CreatedAt : null
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();

            ViewBag.FK_ProductId = new SelectList(
                db.X_Products.Select(p => new {
                    p.ProductId,
                    Name = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameFa).FirstOrDefault()
                }).ToList(),
                "ProductId", "Name", model.FK_ProductId);

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

                // ۱. آپدیت X_ProductVariants
                var variant = db.X_ProductVariants.FirstOrDefault(v => v.ProductVariantId == model.ProductVariantId);
                if (variant == null) return HttpNotFound();

                variant.FK_ProductId = model.FK_ProductId;
                variant.SKU = model.SKU;
                variant.StockQuantity = model.StockQuantity;
                variant.Price = model.Price;
                variant.IsActive = model.VariantIsActive;

                // ۲. آپدیت یا درج X_ProductVariantDiscounts
                var discount = db.X_ProductVariantDiscounts.FirstOrDefault(d => d.FK_ProductVariantId == model.ProductVariantId);

                if (!string.IsNullOrWhiteSpace(model.DiscountType) && model.DiscountValue.HasValue)
                {
                    if (discount == null)
                    {
                        discount = new X_ProductVariantDiscount
                        {
                            FK_ProductVariantId = model.ProductVariantId,
                            DiscountType = model.DiscountType,
                            DiscountValue = model.DiscountValue.Value,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            IsActive = model.DiscountIsActive,
                            CreatedAt = DateTime.Now
                        };
                        db.X_ProductVariantDiscounts.InsertOnSubmit(discount);
                    }
                    else
                    {
                        discount.DiscountType = model.DiscountType;
                        discount.DiscountValue = model.DiscountValue.Value;
                        discount.StartDate = model.StartDate;
                        discount.EndDate = model.EndDate;
                        discount.IsActive = model.DiscountIsActive;
                    }
                }
                else if (discount != null)
                {
                    // اگه کاربر تخفیف رو پاک کرد
                    db.X_ProductVariantDiscounts.DeleteOnSubmit(discount);
                }

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductId = new SelectList(
                db2.X_Products.Select(p => new {
                    p.ProductId,
                    Name = db2.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameFa).FirstOrDefault()
                }).ToList(),
                "ProductId", "Name", model.FK_ProductId);

            return View(model);
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join d in db.X_ProductVariantDiscounts on v.ProductVariantId equals d.FK_ProductVariantId into gj
                         from d in gj.DefaultIfEmpty()
                         where v.ProductVariantId == id
                         select new ProductVariantDiscountViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             VariantIsActive = v.IsActive,

                             ProductVariantDiscountId = d != null ? (int?)d.ProductVariantDiscountId : null,
                             DiscountType = d != null ? d.DiscountType : null,
                             DiscountValue = d != null ? (decimal?)d.DiscountValue : null,
                             StartDate = d != null ? d.StartDate : null,
                             EndDate = d != null ? d.EndDate : null,
                             DiscountIsActive = d != null ? d.IsActive : false
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

            // اول Discount رو حذف کن (چون FK داره)
            var discount = db.X_ProductVariantDiscounts.FirstOrDefault(d => d.FK_ProductVariantId == id);
            if (discount != null)
            {
                db.X_ProductVariantDiscounts.DeleteOnSubmit(discount);
                db.SubmitChanges();
            }

            // بعد Variant رو حذف کن
            var variant = db.X_ProductVariants.FirstOrDefault(v => v.ProductVariantId == id);
            if (variant != null)
            {
                db.X_ProductVariants.DeleteOnSubmit(variant);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}