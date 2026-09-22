using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ProductController : Controller
    {
        // ============ INDEX ============
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from r in db.X_Resources
                         join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into gj
                         from p in gj.DefaultIfEmpty()
                         select new ProductResourceViewModel
                         {
                             ResourceId = r.ResourceId,
                             NameFa = r.NameFa,
                             NameEn = r.NameEn,
                             Description = r.Description,
                             ImageUrl = r.ImageUrl,
                             ResourceIsActive = r.IsActive,
                             ResourceCreatedAt = r.CreatedAt,

                             ProductId = p != null ? (int?)p.ProductId : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             Barcode = p != null ? p.Barcode : null,
                             ProductIsActive = p != null ? p.IsActive : false,
                             ProductCreatedAt = p != null ? (DateTime?)p.CreatedAt : null,
                             ProductUpdatedAt = p != null ? p.UpdatedAt : null
                         }).ToList();

            return View(model);
        }

        // ============ DETAILS ============
        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from r in db.X_Resources
                         join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into gj
                         from p in gj.DefaultIfEmpty()
                         where r.ResourceId == id
                         select new ProductResourceViewModel
                         {
                             ResourceId = r.ResourceId,
                             NameFa = r.NameFa,
                             NameEn = r.NameEn,
                             Description = r.Description,
                             ImageUrl = r.ImageUrl,
                             ResourceIsActive = r.IsActive,
                             ResourceCreatedAt = r.CreatedAt,

                             ProductId = p != null ? (int?)p.ProductId : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             Barcode = p != null ? p.Barcode : null,
                             ProductIsActive = p != null ? p.IsActive : false,
                             ProductCreatedAt = p != null ? (DateTime?)p.CreatedAt : null,
                             ProductUpdatedAt = p != null ? p.UpdatedAt : null
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            return View();
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductResourceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                // ۱. درج در X_Resources
                var resource = new X_Resource
                {
                    NameFa = model.NameFa,
                    NameEn = model.NameEn,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    IsActive = model.ResourceIsActive,
                    CreatedAt = DateTime.Now
                };
                db.X_Resources.InsertOnSubmit(resource);
                db.SubmitChanges(); // برای گرفتن ResourceId

                // ۲. درج در X_Products
                var product = new X_Product
                {
                    FK_ResourceId = resource.ResourceId,
                    ProductCode = model.ProductCode,
                    Barcode = model.Barcode,
                    IsActive = model.ProductIsActive,
                    CreatedAt = DateTime.Now
                };
                db.X_Products.InsertOnSubmit(product);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from r in db.X_Resources
                         join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into gj
                         from p in gj.DefaultIfEmpty()
                         where r.ResourceId == id
                         select new ProductResourceViewModel
                         {
                             ResourceId = r.ResourceId,
                             NameFa = r.NameFa,
                             NameEn = r.NameEn,
                             Description = r.Description,
                             ImageUrl = r.ImageUrl,
                             ResourceIsActive = r.IsActive,
                             ResourceCreatedAt = r.CreatedAt,

                             ProductId = p != null ? (int?)p.ProductId : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             Barcode = p != null ? p.Barcode : null,
                             ProductIsActive = p != null ? p.IsActive : false,
                             ProductCreatedAt = p != null ? (DateTime?)p.CreatedAt : null,
                             ProductUpdatedAt = p != null ? p.UpdatedAt : null
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductResourceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                // ۱. آپدیت X_Resources
                var resource = db.X_Resources.FirstOrDefault(r => r.ResourceId == model.ResourceId);
                if (resource == null) return HttpNotFound();

                resource.NameFa = model.NameFa;
                resource.NameEn = model.NameEn;
                resource.Description = model.Description;
                resource.ImageUrl = model.ImageUrl;
                resource.IsActive = model.ResourceIsActive;

                // ۲. آپدیت یا درج X_Products
                var product = db.X_Products.FirstOrDefault(p => p.FK_ResourceId == model.ResourceId);
                if (product == null)
                {
                    // اگه Product نداشت، بساز
                    product = new X_Product
                    {
                        FK_ResourceId = model.ResourceId,
                        ProductCode = model.ProductCode,
                        Barcode = model.Barcode,
                        IsActive = model.ProductIsActive,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Products.InsertOnSubmit(product);
                }
                else
                {
                    product.ProductCode = model.ProductCode;
                    product.Barcode = model.Barcode;
                    product.IsActive = model.ProductIsActive;
                    product.UpdatedAt = DateTime.Now;
                }

                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from r in db.X_Resources
                         join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into gj
                         from p in gj.DefaultIfEmpty()
                         where r.ResourceId == id
                         select new ProductResourceViewModel
                         {
                             ResourceId = r.ResourceId,
                             NameFa = r.NameFa,
                             NameEn = r.NameEn,
                             Description = r.Description,
                             ImageUrl = r.ImageUrl,
                             ResourceIsActive = r.IsActive,
                             ResourceCreatedAt = r.CreatedAt,

                             ProductId = p != null ? (int?)p.ProductId : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             Barcode = p != null ? p.Barcode : null,
                             ProductIsActive = p != null ? p.IsActive : false,
                             ProductCreatedAt = p != null ? (DateTime?)p.CreatedAt : null,
                             ProductUpdatedAt = p != null ? p.UpdatedAt : null
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

            // اول Product رو حذف کن (چون FK داره)
            var product = db.X_Products.FirstOrDefault(p => p.FK_ResourceId == id);
            if (product != null)
            {
                db.X_Products.DeleteOnSubmit(product);
                db.SubmitChanges();
            }

            // بعد Resource رو حذف کن
            var resource = db.X_Resources.FirstOrDefault(r => r.ResourceId == id);
            if (resource != null)
            {
                db.X_Resources.DeleteOnSubmit(resource);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }

}