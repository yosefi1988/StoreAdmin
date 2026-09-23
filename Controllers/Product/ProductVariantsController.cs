using System;
using System.Collections.Generic;
using System.Linq;
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
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         orderby r.NameFa, v.SKU
                         select new ProductVariantViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             IsActive = v.IsActive
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
                         where v.ProductVariantId == id
                         select new ProductVariantViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             IsActive = v.IsActive
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductId = BuildProductSelectList(db);
            return View();
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductVariantViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var entity = new X_ProductVariant
                {
                    FK_ProductId = model.FK_ProductId,
                    SKU = model.SKU,
                    StockQuantity = model.StockQuantity,
                    Price = model.Price,
                    IsActive = model.IsActive
                };

                db.X_ProductVariants.InsertOnSubmit(entity);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductId = BuildProductSelectList(db2, model.FK_ProductId);
            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var entity = db.X_ProductVariants.FirstOrDefault(x => x.ProductVariantId == id);
            if (entity == null) return HttpNotFound();

            var model = new ProductVariantViewModel
            {
                ProductVariantId = entity.ProductVariantId,
                FK_ProductId = entity.FK_ProductId,
                SKU = entity.SKU,
                StockQuantity = entity.StockQuantity,
                Price = entity.Price,
                IsActive = entity.IsActive
            };

            ViewBag.FK_ProductId = BuildProductSelectList(db, entity.FK_ProductId);
            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductVariantViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var entity = db.X_ProductVariants.FirstOrDefault(x => x.ProductVariantId == model.ProductVariantId);
                if (entity == null) return HttpNotFound();

                entity.FK_ProductId = model.FK_ProductId;
                entity.SKU = model.SKU;
                entity.StockQuantity = model.StockQuantity;
                entity.Price = model.Price;
                entity.IsActive = model.IsActive;

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ProductId = BuildProductSelectList(db2, model.FK_ProductId);
            return View(model);
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from v in db.X_ProductVariants
                         join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                         from p in pj.DefaultIfEmpty()
                         join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         where v.ProductVariantId == id
                         select new ProductVariantViewModel
                         {
                             ProductVariantId = v.ProductVariantId,
                             FK_ProductId = v.FK_ProductId,
                             ProductName = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             SKU = v.SKU,
                             StockQuantity = v.StockQuantity,
                             Price = v.Price,
                             IsActive = v.IsActive
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

            var entity = db.X_ProductVariants.FirstOrDefault(x => x.ProductVariantId == id);
            if (entity != null)
            {
                db.X_ProductVariants.DeleteOnSubmit(entity);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        // ============ Helper ============
        private List<SelectListItem> BuildProductSelectList(DataClassesDatabaseDataContext db, int? selectedId = null)
        {
            var products = (from p in db.X_Products
                            join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                            where r.IsActive
                            select new
                            {
                                p.ProductId,
                                ProductName = r.NameFa,
                                p.ProductCode,
                                CategoryName = db.X_ResourceCategories
                                    .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                    .Join(db.X_Categories,
                                          rc => rc.FK_CategoryId,
                                          c => c.CategoryId,
                                          (rc, c) => c.NameFa)
                                    .FirstOrDefault() ?? "بدون دسته‌بندی"
                            }).ToList();

            var items = new List<SelectListItem>();
            foreach (var grp in products.GroupBy(x => x.CategoryName).OrderBy(g => g.Key))
            {
                var group = new SelectListGroup { Name = grp.Key };
                foreach (var p in grp.OrderBy(x => x.ProductName))
                {
                    var text = p.ProductName;
                    if (!string.IsNullOrEmpty(p.ProductCode))
                        text += $" ({p.ProductCode})";

                    items.Add(new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = text,
                        Group = group,
                        Selected = selectedId.HasValue && selectedId.Value == p.ProductId
                    });
                }
            }

            return items;
        }
    }
}