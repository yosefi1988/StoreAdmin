using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AllowAnonymous]
    public class ProductsController : BaseController
    {
        // صفحه لیست
        public ActionResult Index(string search = null)
        {
            ViewBag.Search = search;
            return View();
        }

        // ==================== AJAX: لیست محصولات (Index) ====================
        [HttpGet]
        public JsonResult GetProducts(int page = 1, int pageSize = 20,
                                       string search = null,
                                       string userName = null,
                                       bool mine = false)
        {
            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var currentUserId = GetCurrentUserId();

                    var query = from p in db.X_Products
                                join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                                where p.IsActive
                                select new
                                {
                                    p.ProductId,
                                    r.NameFa,
                                    r.NameEn,
                                    r.ImageUrl,
                                    p.ProductCode,
                                    p.Barcode,
                                    p.IsActive,
                                    p.CreatedAt,
                                    UserId = r.FK_UserId,
                                    UserName = r.X_User.UserName,
                                    FirstName = r.X_User.FirstName,
                                    LastName = r.X_User.LastName,
                                    MobileNumber = r.X_User.MobileNumber
                                };

                    if (mine && currentUserId.HasValue)
                        query = query.Where(x => x.UserId == currentUserId.Value);

                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        userName = userName.Trim();
                        query = query.Where(x =>
                            x.UserName.Contains(userName) ||
                            x.FirstName.Contains(userName) ||
                            x.LastName.Contains(userName) ||
                            x.MobileNumber.Contains(userName));
                    }

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
                        .OrderByDescending(x => x.CreatedAt)
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
                return Json(new { success = false, message = ex.Message },
                            JsonRequestBehavior.AllowGet);
            }
        }


        // ==================== صفحه اسکرول ====================
        public ActionResult Scroll(string userName = null, bool mine = false, int? categoryId = null)
        {
            ViewBag.FilterUserName = userName;
            ViewBag.Mine = mine;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    ViewBag.FilterCategoryId = categoryId.Value;
                    ViewBag.FilterCategoryName = db.X_Categories
                        .Where(c => c.CategoryId == categoryId.Value)
                        .Select(c => c.NameFa)
                        .FirstOrDefault();
                }
            }
            else
            {
                ViewBag.FilterCategoryId = null;
                ViewBag.FilterCategoryName = null;
            }

            return View();
        }


        // ==================== AJAX: لود محصولات (Scroll) ====================
        public JsonResult GetProductsScroll(int skip = 0, int take = 12, int seed = 0,
                                             string userName = null, bool mine = false, int? categoryId = null)
        {
            if (seed == 0)
                seed = new Random().Next(1, 1000000);

            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var currentUserId = GetCurrentUserId();

                var query = db.X_Products.Where(p => p.IsActive);

                // فقط محصولات من
                if (mine)
                {
                    if (!currentUserId.HasValue)
                    {
                        return Json(new
                        {
                            Data = new List<object>(),
                            Total = 0,
                            HasMore = false,
                            Seed = seed,
                            Error = "کاربر لاگین نیست"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    query = query.Where(p =>
                        db.X_Resources
                          .Where(r => r.ResourceId == p.FK_ResourceId
                                   && r.FK_UserId == currentUserId.Value)
                          .Any());
                }

                // فیلتر نام کاربر
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    var u = userName.Trim();
                    query = query.Where(p =>
                        db.X_Resources
                          .Where(r => r.ResourceId == p.FK_ResourceId
                                   && r.X_User != null
                                   && (r.X_User.UserName.Contains(u)
                                    || r.X_User.FirstName.Contains(u)
                                    || r.X_User.LastName.Contains(u)
                                    || r.X_User.MobileNumber.Contains(u)))
                          .Any());
                }

                // فیلتر دسته‌بندی (با زیردسته‌ها)
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    var categoryIds = GetCategoryWithDescendants(categoryId.Value, db);
                    query = query.Where(p =>
                        db.X_ResourceCategories
                          .Where(rc => rc.FK_ResourceId == p.FK_ResourceId
                                    && categoryIds.Contains(rc.FK_CategoryId))
                          .Any());
                }

                var total = query.Count();

                var products = query
                    .Select(p => new
                    {
                        ProductId = p.ProductId,
                        NameFa = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameFa).FirstOrDefault(),
                        NameEn = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.NameEn).FirstOrDefault(),
                        ImageUrl = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.ImageUrl).FirstOrDefault(),
                        ProductCode = p.ProductCode,
                        Barcode = p.Barcode,
                        IsActive = p.IsActive,
                        UserId = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.FK_UserId).FirstOrDefault(),
                        UserName = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.X_User.UserName).FirstOrDefault(),
                        FirstName = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.X_User.FirstName).FirstOrDefault(),
                        LastName = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.X_User.LastName).FirstOrDefault(),
                        MobileNumber = db.X_Resources.Where(r => r.ResourceId == p.FK_ResourceId).Select(r => r.X_User.MobileNumber).FirstOrDefault()
                    })
                    .ToList()
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


        // تابع کمکی: گرفتن دسته و همه زیردسته‌ها به‌صورت بازگشتی
        private List<int> GetCategoryWithDescendants(int categoryId, DataClassesDatabaseDataContextDataContext db)
        {
            var result = new List<int> { categoryId };
            var children = db.X_Categories
                .Where(c => c.FK_ParentId == categoryId && c.IsActive)
                .Select(c => c.CategoryId)
                .ToList();

            foreach (var child in children)
                result.AddRange(GetCategoryWithDescendants(child, db));

            return result;
        }

        // ==================== Details ====================
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
                                   ImageUrl = r.ImageUrl,

                                   // اطلاعات ثبت‌کننده
                                   FK_UserId = r.FK_UserId,
                                   OwnerUserName = r.X_User.UserName,
                                   OwnerFirstName = r.X_User.FirstName,
                                   OwnerLastName = r.X_User.LastName,
                                   OwnerMobile = r.X_User.MobileNumber
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

                #region Resource + product Attributes
                var resourceAttributes = (
                    from ra in db.X_ResourceAttributes
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

                var variantAttributes = (
                    from pva in db.X_ProductVariantAttributes
                    join pv in db.X_ProductVariants on pva.FK_ProductVariantId equals pv.ProductVariantId
                    join a in db.X_Attributes on pva.FK_AttributeId equals a.AttributeId
                    join av in db.X_AttributeValues on pva.FK_AttributeValueId equals av.AttributeValueId into avJoin
                    from av in avJoin.DefaultIfEmpty()
                    where pv.FK_ProductId == product.ProductId
                    select new AttributeDto
                    {
                        AttributeId = a.AttributeId,
                        NameFa = a.NameFa,
                        DataType = a.DataType,
                        Value = av != null ? av.ValueFa : "-"
                    }).ToList();

                product.Attributes = resourceAttributes
                    .Concat(variantAttributes)
                    .GroupBy(x => x.AttributeId)
                    .Select(g => g.First())
                    .ToList();
                #endregion

                product.Variants = db.X_ProductVariants
                    .Where(v => v.FK_ProductId == product.ProductId && v.IsActive)
                    .Select(v => new VariantDto
                    {
                        ProductVariantId = v.ProductVariantId,
                        SKU = v.SKU,
                        StockQuantity = v.StockQuantity,
                        Price = (decimal)v.Price,
                        IsActive = v.IsActive,
                        Attributes = (from pva in db.X_ProductVariantAttributes
                                      join at in db.X_Attributes on pva.FK_AttributeId equals at.AttributeId
                                      join av in db.X_AttributeValues on pva.FK_AttributeValueId equals av.AttributeValueId
                                      where pva.FK_ProductVariantId == v.ProductVariantId
                                      select new VariantAttributeDto
                                      {
                                          ProductVariantAttributeId = pva.ProductVariantAttributeId,
                                          AttributeId = at.AttributeId,
                                          AttributeNameFa = at.NameFa,
                                          AttributeNameEn = at.NameEn,
                                          AttributeCode = at.Code,
                                          DataType = at.DataType,
                                          AttributeValueId = av.AttributeValueId,
                                          ValueFa = av.ValueFa,
                                          ValueEn = av.ValueEn,
                                          ValueCode = av.Code
                                      }).ToList()
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

        // ==================== Helper ====================

        private int? GetCurrentUserId()
        {
            if (HttpContext != null && HttpContext.Items != null && HttpContext.Items["UserId"] != null)
            {
                object item = HttpContext.Items["UserId"];
                if (item is int)
                    return (int)item;
            }

            if (Request != null && Request.Cookies != null && Request.Cookies["auth_token"] != null)
            {
                string authToken = Request.Cookies["auth_token"].Value;
                if (!string.IsNullOrEmpty(authToken))
                {
                    try
                    {
                        var tokenService = new SessionTokenService();
                        var currentUser = tokenService.ValidateToken(authToken);
                        if (currentUser != null)
                            return currentUser.UserId;
                    }
                    catch { }
                }
            }

            return null;
        }

    }
}