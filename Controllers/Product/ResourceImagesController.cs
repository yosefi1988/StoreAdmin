using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ResourceImagesController : Controller
    {
        // ============ INDEX ============
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from ri in db.X_ResourceImages
                         join r in db.X_Resources on ri.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into pj
                         from p in pj.DefaultIfEmpty()
                         orderby ri.FK_ResourceId, ri.SortOrder
                         select new ResourceImageViewModel
                         {
                             ResourceImageId = ri.ResourceImageId,
                             FK_ResourceId = ri.FK_ResourceId,
                             ResourceNameFa = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,   // ← کد محصول
                             ImageUrl = ri.ImageUrl,
                             Title = ri.Title,
                             SortOrder = ri.SortOrder,
                             IsMain = ri.IsMain,
                             IsActive = ri.IsActive,
                             CreatedAt = ri.CreatedAt
                         }).ToList();

            return View(model);
        }

        // ============ DETAILS ============
        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from ri in db.X_ResourceImages
                         join r in db.X_Resources on ri.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         join p in db.X_Products on r.ResourceId equals p.FK_ResourceId into pj
                         from p in pj.DefaultIfEmpty()
                         where ri.ResourceImageId == id
                         select new ResourceImageViewModel
                         {
                             ResourceImageId = ri.ResourceImageId,
                             FK_ResourceId = ri.FK_ResourceId,
                             ResourceNameFa = r != null ? r.NameFa : null,
                             ProductCode = p != null ? p.ProductCode : null,
                             ImageUrl = ri.ImageUrl,
                             Title = ri.Title,
                             SortOrder = ri.SortOrder,
                             IsMain = ri.IsMain,
                             IsActive = ri.IsActive,
                             CreatedAt = ri.CreatedAt
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();

            // لیست سایر تصاویر همین محصول (برای نمایش توی گالری)
            ViewBag.OtherImages = db.X_ResourceImages
                .Where(x => x.FK_ResourceId == model.FK_ResourceId && x.ResourceImageId != model.ResourceImageId)
                .OrderBy(x => x.SortOrder)
                .Select(x => new ResourceImageViewModel
                {
                    ResourceImageId = x.ResourceImageId,
                    FK_ResourceId = x.FK_ResourceId,
                    ResourceNameFa = model.ResourceNameFa,
                    ProductCode = model.ProductCode,
                    ImageUrl = x.ImageUrl,
                    Title = x.Title,
                    SortOrder = x.SortOrder,
                    IsMain = x.IsMain,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                }).ToList();

            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();

            // همه محصولات با نام Resource و دسته‌بندی اولشون
            var products = (from p in db.X_Products
                            join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                            where r.IsActive
                            select new
                            {
                                p.ProductId,
                                r.ResourceId,
                                r.NameFa,
                                p.ProductCode,
                                CategoryName = db.X_ResourceCategories
                                    .Where(rc => rc.FK_ResourceId == r.ResourceId)
                                    .Join(db.X_Categories,
                                          rc => rc.FK_CategoryId,
                                          c => c.CategoryId,
                                          (rc, c) => c.NameFa)
                                    .FirstOrDefault() ?? "بدون دسته‌بندی"
                            }).ToList();

            // گروه‌بندی بر اساس دسته‌بندی
            var grouped = products
                .GroupBy(p => p.CategoryName)
                .OrderBy(g => g.Key)
                .Select(g => new SelectListGroup { Name = g.Key })
                .ToList();

            // ساخت SelectListItem با Group
            var items = new List<SelectListItem>();
            foreach (var grp in products.GroupBy(p => p.CategoryName).OrderBy(g => g.Key))
            {
                var group = new SelectListGroup { Name = grp.Key };
                foreach (var p in grp.OrderBy(x => x.NameFa))
                {
                    items.Add(new SelectListItem
                    {
                        Value = p.ResourceId.ToString(),
                        Text = p.NameFa + (string.IsNullOrEmpty(p.ProductCode) ? "" : $" ({p.ProductCode})"),
                        Group = group
                    });
                }
            }

            ViewBag.FK_ResourceId = items;

            return View();
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var entity = new X_ResourceImage
                {
                    FK_ResourceId = model.FK_ResourceId,
                    ImageUrl = model.ImageUrl,
                    Title = model.Title,
                    SortOrder = model.SortOrder,
                    IsMain = model.IsMain,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.Now
                };

                // اگه IsMain=true، بقیه تصاویر همون Resource رو IsMain=false کن
                if (model.IsMain)
                {
                    var others = db.X_ResourceImages.Where(x => x.FK_ResourceId == model.FK_ResourceId && x.IsMain).ToList();
                    foreach (var o in others) o.IsMain = false;
                }

                db.X_ResourceImages.InsertOnSubmit(entity);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ResourceId = new SelectList(db2.X_Resources.Where(r => r.IsActive).ToList(), "ResourceId", "NameFa", model.FK_ResourceId);
            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var entity = db.X_ResourceImages.FirstOrDefault(x => x.ResourceImageId == id);
            if (entity == null) return HttpNotFound();

            var model = new ResourceImageViewModel
            {
                ResourceImageId = entity.ResourceImageId,
                FK_ResourceId = entity.FK_ResourceId,
                ImageUrl = entity.ImageUrl,
                Title = entity.Title,
                SortOrder = entity.SortOrder,
                IsMain = entity.IsMain,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };

            ViewBag.FK_ResourceId = new SelectList(db.X_Resources.Where(r => r.IsActive).ToList(), "ResourceId", "NameFa", entity.FK_ResourceId);
            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ResourceImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var entity = db.X_ResourceImages.FirstOrDefault(x => x.ResourceImageId == model.ResourceImageId);
                if (entity == null) return HttpNotFound();

                entity.FK_ResourceId = model.FK_ResourceId;
                entity.ImageUrl = model.ImageUrl;
                entity.Title = model.Title;
                entity.SortOrder = model.SortOrder;
                entity.IsActive = model.IsActive;

                // اگه IsMain=true، بقیه رو false کن
                if (model.IsMain && !entity.IsMain)
                {
                    var others = db.X_ResourceImages.Where(x => x.FK_ResourceId == model.FK_ResourceId && x.IsMain && x.ResourceImageId != model.ResourceImageId).ToList();
                    foreach (var o in others) o.IsMain = false;
                }
                entity.IsMain = model.IsMain;

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_ResourceId = new SelectList(db2.X_Resources.Where(r => r.IsActive).ToList(), "ResourceId", "NameFa", model.FK_ResourceId);
            return View(model);
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from ri in db.X_ResourceImages
                         join r in db.X_Resources on ri.FK_ResourceId equals r.ResourceId into rj
                         from r in rj.DefaultIfEmpty()
                         where ri.ResourceImageId == id
                         select new ResourceImageViewModel
                         {
                             ResourceImageId = ri.ResourceImageId,
                             FK_ResourceId = ri.FK_ResourceId,
                             ResourceNameFa = r != null ? r.NameFa : null,
                             ImageUrl = ri.ImageUrl,
                             Title = ri.Title,
                             SortOrder = ri.SortOrder,
                             IsMain = ri.IsMain,
                             IsActive = ri.IsActive,
                             CreatedAt = ri.CreatedAt
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

            var entity = db.X_ResourceImages.FirstOrDefault(x => x.ResourceImageId == id);
            if (entity != null)
            {
                db.X_ResourceImages.DeleteOnSubmit(entity);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}