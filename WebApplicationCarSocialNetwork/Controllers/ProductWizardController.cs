using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels.Wizard;
using WebApplicationCar.Filters;

namespace WebApplicationCar.Controllers
{
    [TokenAuthorize]
    public class ProductWizardController : Controller
    {
        // ========== نمایش Wizard ==========
        public ActionResult Create()
        {
            return View();
        }

        // ========== دسته‌بندی‌ها (درختی) ==========
        [HttpGet]
        public JsonResult GetCategories()
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                // مرحله ۱: داده خام از دیتابیس (فقط IsActive)
                var raw = db.X_Categories
                    .Where(c => c.IsActive)
                    .Select(c => new
                    {
                        c.CategoryId,
                        c.NameFa,
                        c.CategoryType,
                        c.FK_ParentId
                    })
                    .ToList();

                // مرحله ۲: map به CategoryNodeVM تو حافظه
                var all = raw.Select(x => new CategoryNodeVM
                {
                    CategoryId = x.CategoryId,
                    NameFa = x.NameFa,
                    CategoryType = x.CategoryType,
                    FK_ParentId = x.FK_ParentId,
                    Children = new List<CategoryNodeVM>()
                }).ToList();

                // مرحله ۳: ریشه‌ها = دسته‌هایی که FK_ParentId == 1
                var tree = all
                    .Where(x => x.FK_ParentId == 1)
                    .ToList();

                // مرحله ۴: فرزندان بازگشتی برای هر ریشه
                foreach (var node in tree)
                {
                    node.Children = BuildChildren(all, node.CategoryId);
                }

                return Json(tree, JsonRequestBehavior.AllowGet);
            }
        }

        private List<CategoryNodeVM> BuildChildren(List<CategoryNodeVM> all, int parentId)
        {
            var children = all.Where(x => x.FK_ParentId == parentId).ToList();
            foreach (var c in children)
            {
                c.Children = BuildChildren(all, c.CategoryId);
            }
            return children;
        }

        // ========== صفت‌ها ==========
        [HttpGet]
        public JsonResult GetAttributes()
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var attributes = db.X_Attributes
                    .Where(a => a.IsActive && (a.FK_CategoryId == 7 || a.FK_CategoryId == 6))
                    .Select(a => new { a.AttributeId, a.NameFa, a.DataType })
                    .ToList();
                return Json(attributes, JsonRequestBehavior.AllowGet);
            }
        }

        // ========== مقادیر یه صفت ==========
        [HttpGet]
        public JsonResult GetAttributeValues(int attributeId)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var values = db.X_AttributeValues
                    .Where(v => v.FK_AttributeId == attributeId && v.IsActive)
                    .Select(v => new { v.AttributeValueId, v.ValueFa })
                    .ToList();
                return Json(values, JsonRequestBehavior.AllowGet);
            }
        }

        // ========== ذخیره نهایی ==========
        [HttpPost]
        public JsonResult Save(ProductWizardViewModel model)
        {
            try
            {
                if (model == null || model.Step1 == null)
                    return Json(new { success = false, message = "اطلاعات ناقص است" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var userId = HttpContext.Items["UserId"] as int?;

                    // ۱. X_Resources
                    var resource = new X_Resource
                    {
                        NameFa = model.Step1.NameFa,
                        NameEn = model.Step1.NameEn,
                        Description = model.Step1.Description,
                        ImageUrl = model.Step1.ImageUrl,
                        IsActive = model.Step1.ResourceIsActive,
                        CreatedAt = DateTime.Now,
                        FK_UserId = userId
                    };
                    db.X_Resources.InsertOnSubmit(resource);
                    db.SubmitChanges();

                    // ۲. X_Products
                    var product = new X_Product
                    {
                        FK_ResourceId = resource.ResourceId,
                        ProductCode = model.Step1.ProductCode,
                        Barcode = model.Step1.Barcode,
                        IsActive = model.Step1.ProductIsActive,
                        CreatedAt = DateTime.Now,
                    };
                    db.X_Products.InsertOnSubmit(product);
                    db.SubmitChanges();

                    // ۳. X_ProductVariants
                    var variantIds = new List<int>();
                    if (model.Step1.Variants != null && model.Step1.Variants.Any())
                    {
                        foreach (var v in model.Step1.Variants)
                        {
                            var variant = new X_ProductVariant
                            {
                                FK_ProductId = product.ProductId,
                                SKU = v.SKU,
                                StockQuantity = v.StockQuantity,
                                Price = v.Price,
                                IsActive = v.IsActive
                            };
                            db.X_ProductVariants.InsertOnSubmit(variant);
                            db.SubmitChanges();
                            variantIds.Add(variant.ProductVariantId);
                        }
                    }

                    // ۴. X_ResourceCategories
                    if (model.Step2 != null && model.Step2.SelectedCategoryIds != null)
                    {
                        foreach (var catId in model.Step2.SelectedCategoryIds)
                        {
                            db.X_ResourceCategories.InsertOnSubmit(new X_ResourceCategory
                            {
                                FK_ResourceId = resource.ResourceId,
                                FK_CategoryId = catId
                            });
                        }
                    }

                    // ۵. X_ProductVariantAttributes
                    if (model.Step2 != null && model.Step2.VariantAttributes != null)
                    {
                        foreach (var va in model.Step2.VariantAttributes)
                        {
                            if (va.VariantIndex < 0 || va.VariantIndex >= variantIds.Count) continue;
                            var variantId = variantIds[va.VariantIndex];

                            foreach (var attr in va.Attributes)
                            {
                                db.X_ProductVariantAttributes.InsertOnSubmit(new X_ProductVariantAttribute
                                {
                                    FK_ProductVariantId = variantId,
                                    FK_AttributeId = attr.FK_AttributeId,
                                    FK_AttributeValueId = attr.FK_AttributeValueId
                                });
                            }
                        }
                    }

                    // ۶. X_Properties — محل تولید / انبار (کشور، استان، شهر)
                    // این بلوک باید قبل از SubmitChanges نهایی باشه (نیازی به SubmitChanges جدا نداره)
                    if (model.Step2 != null &&
                        (model.Step2.CountryId.HasValue || model.Step2.ProvinceId.HasValue || model.Step2.CityId.HasValue))
                    {
                        db.X_Properties.InsertOnSubmit(new X_Property
                        {
                            FK_ProductId = product.ProductId,
                            FK_CountryId = model.Step2.CountryId,
                            FK_ProvinceId = model.Step2.ProvinceId,
                            FK_CityId = model.Step2.CityId,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        });
                    }

                    // ۷. X_ResourceAttributes
                    if (model.Step3 != null && model.Step3.ResourceAttributes != null)
                    {
                        foreach (var ra in model.Step3.ResourceAttributes)
                        {
                            db.X_ResourceAttributes.InsertOnSubmit(new X_ResourceAttribute
                            {
                                FK_ResourceId = resource.ResourceId,
                                FK_AttributeId = ra.FK_AttributeId,
                                FK_AttributeValueId = ra.FK_AttributeValueId,
                                ValueNumber = ra.ValueNumber,
                                ValueDate = ra.ValueDate,
                                ValueBoolean = ra.ValueBoolean,
                                ValueText = ra.ValueText
                            });
                        }
                    }

                    // ۸. X_ResourceImages
                    if (model.Step3 != null && model.Step3.Images != null)
                    {
                        foreach (var img in model.Step3.Images)
                        {
                            db.X_ResourceImages.InsertOnSubmit(new X_ResourceImage
                            {
                                FK_ResourceId = resource.ResourceId,
                                ImageUrl = img.ImageUrl,
                                Title = img.Title,
                                SortOrder = img.SortOrder,
                                IsMain = img.IsMain,
                                IsActive = img.IsActive,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }

                    db.SubmitChanges();

                    return Json(new
                    {
                        success = true,
                        message = "محصول با موفقیت ثبت شد",
                        resourceId = resource.ResourceId,
                        productId = product.ProductId
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "خطا در ذخیره: " + ex.Message,
                    details = ex.ToString()
                });
            }
        }

        // ========== آپلود تصویر ==========
        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "فایلی انتخاب نشده" });

                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!allowed.Contains(ext))
                    return Json(new { success = false, message = "فرمت مجاز نیست" });

                if (file.ContentLength > 5 * 1024 * 1024)
                    return Json(new { success = false, message = "حجم بیش از ۵ مگابایت" });

                var folder = Server.MapPath("~/Uploads/Products/");
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString("N") + ext;
                var fullPath = System.IO.Path.Combine(folder, fileName);
                file.SaveAs(fullPath);

                var url = Url.Content("~/Uploads/Products/" + fileName);
                return Json(new { success = true, url = url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public JsonResult GetCountries()
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var list = db.X_Countries
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.NameFa)
                    .Select(c => new { id = c.CountryId, text = c.NameFa })
                    .ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProvinces(int countryId)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var list = db.X_Provinces
                    .Where(p => p.FK_CountryId == countryId && p.IsActive)
                    .OrderBy(p => p.NameFa)
                    .Select(p => new { id = p.ProvinceId, text = p.NameFa })
                    .ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCities(int provinceId)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var list = db.X_Cities
                    .Where(c => c.FK_ProvinceId == provinceId && c.IsActive)
                    .OrderBy(c => c.NameFa)
                    .Select(c => new { id = c.CityId, text = c.NameFa })
                    .ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }


    }
}