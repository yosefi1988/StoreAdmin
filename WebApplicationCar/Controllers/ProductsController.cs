using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;

namespace WebApplicationCar.Controllers
{
    public class ProductsController : BaseController
    {
        // صفحه لیست
        public ActionResult Index(string search = null)
        {
            ViewBag.Search = search;
            return View();
        }

        // AJAX: لیست محصولات - مستقیم از DataContext
        [HttpGet]
        public JsonResult GetProducts(int page = 1, int pageSize = 20, string search = null)
        {
            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var query = from p in db.X_Products
                                join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                                where p.IsActive
                                select new
                                {
                                    p.ProductId,
                                    r.NameFa,
                                    r.NameEn,
                                    p.ProductCode,
                                    p.Barcode,
                                    p.IsActive,
                                    p.CreatedAt
                                };

                    // ← فیلتر جستجو
                    if (!string.IsNullOrEmpty(search))
                    {
                        search = search.Trim();
                        query = query.Where(x =>
                            x.NameFa.Contains(search) ||
                            x.NameEn.Contains(search) ||
                            x.ProductCode.Contains(search) ||
                            x.Barcode.Contains(search));
                    }

                    var total = query.Count();
                    var totalPages = (int)Math.Ceiling((double)total / pageSize);

                    var data = query
                        .OrderBy(x => x.ProductId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    return Json(new
                    {
                        Total = total,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages,
                        Data = data
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== صفحه اسکرول (AJAX) ====================
        public ActionResult Scroll()
        {
            return View();   // مدل نمی‌خواد، JS خودش لود می‌کنه
        }


        // ==================== AJAX: لود محصولات ====================
        public JsonResult GetProductsScroll(int skip = 0, int take = 12, int seed = 0)
        {
            if (seed == 0)
                seed = new Random().Next(1, 1000000);

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var query = db.X_Products.Where(p => p.IsActive);
                var total = query.Count();

                //var products = query
                //               .OrderBy(p => (p.ProductId * 1103515245 + seed) % 2147483647)
                //               .Skip(skip)
                //               .Take(take)
                //               .Select(p => new
                //               {
                //                   ProductId = p.ProductId,
                //                   NameFa = db.X_Resources
                //                                  .Where(r => r.ResourceId == p.FK_ResourceId)
                //                                  .Select(r => r.NameFa).FirstOrDefault(),
                //                   NameEn = db.X_Resources
                //                                  .Where(r => r.ResourceId == p.FK_ResourceId)
                //                                  .Select(r => r.NameEn).FirstOrDefault(),
                //                   ImageUrl = db.X_Resources
                //                                  .Where(r => r.ResourceId == p.FK_ResourceId)
                //                                  .Select(r => r.ImageUrl).FirstOrDefault(),
                //                   ProductCode = p.ProductCode,
                //                   Barcode = p.Barcode,
                //                   IsActive = p.IsActive
                //               })
                //               .ToList();

                var products = query
                    .Select(p => new
                    {
                        ProductId = p.ProductId,
                        NameFa = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameFa).FirstOrDefault(),
                        NameEn = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameEn).FirstOrDefault(),
                        ImageUrl = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.ImageUrl).FirstOrDefault(),
                        ProductCode = p.ProductCode,
                        Barcode = p.Barcode,
                        IsActive = p.IsActive
                    })
                    .ToList()                                  // ← انتقال به حافظه
                    .OrderBy(p => (long)(p.ProductId * 1103515245 + seed) % 2147483647)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                return Json(new
                {
                    Data = products,
                    Total = total,
                    HasMore = (skip + take) < total,
                    Seed = seed
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Details(int id)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var product = (from p in db.X_Products
                               join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                               where p.ProductId == id
                               select new ProductDetailsViewModel
                               {
                                   ProductId = p.ProductId,
                                   FK_ResourceId = p.FK_ResourceId,
                                   ProductCode = p.ProductCode,
                                   Barcode = p.Barcode,
                                   IsActive = p.IsActive,
                                   CreatedAt = p.CreatedAt,
                                   NameFa = r.NameFa,
                                   NameEn = r.NameEn,
                                   Description = r.Description,
                                   ImageUrl = r.ImageUrl
                               }).FirstOrDefault();

                if (product == null)
                    return HttpNotFound();

                product.Images = db.X_ResourceImages
                    .Where(i => i.FK_ResourceId == product.FK_ResourceId && i.IsActive)
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => new ImageDto
                    {
                        ResourceImageId = i.ResourceImageId,
                        ImageUrl = i.ImageUrl,
                        Title = i.Title,
                        SortOrder = i.SortOrder,
                        IsMain = i.IsMain
                    }).ToList();

                product.Attributes = (from ra in db.X_ResourceAttributes
                                      join a in db.X_Attributes on ra.FK_AttributeId equals a.AttributeId
                                      join av in db.X_AttributeValues on ra.FK_AttributeValueId equals av.AttributeValueId into avJoin
                                      from av in avJoin.DefaultIfEmpty()
                                      where ra.FK_ResourceId == product.FK_ResourceId
                                      select new AttributeDto
                                      {
                                          AttributeId = a.AttributeId,
                                          NameFa = a.NameFa,
                                          DataType = a.DataType,
                                          Value = av != null ? av.ValueFa :
                                                  (ra.ValueText != null ? ra.ValueText :
                                                  (ra.ValueNumber != null ? ra.ValueNumber.ToString() :
                                                  (ra.ValueBoolean != null ? (ra.ValueBoolean == true ? "بله" : "خیر") : "-")))
                                      }).ToList();

                product.Variants = db.X_ProductVariants
                    .Where(v => v.FK_ProductId == product.ProductId && v.IsActive)
                    .Select(v => new VariantDto
                    {
                        ProductVariantId = v.ProductVariantId,
                        SKU = v.SKU,
                        StockQuantity = v.StockQuantity,
                        Price = v.Price,
                        IsActive = v.IsActive
                    }).ToList();

                product.Categories = (from rc in db.X_ResourceCategories
                                      join c in db.X_Categories on rc.FK_CategoryId equals c.CategoryId
                                      where rc.FK_ResourceId == product.FK_ResourceId
                                      select new CategoryDto
                                      {
                                          CategoryId = c.CategoryId,
                                          NameFa = c.NameFa,
                                          CategoryType = c.CategoryType
                                      }).ToList();

                return View(product);
            }
        }
    }
}