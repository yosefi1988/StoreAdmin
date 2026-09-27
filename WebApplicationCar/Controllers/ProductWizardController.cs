using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Models.ViewModels.Wizard;

namespace WebApplicationCar.Controllers
{
      
    public class ProductWizardController : Controller
    {
        
        // ========== نمایش Wizard ==========
        public ActionResult Create()
        {
            return View();
        }

        // ========== AJAX: بارگذاری Dropdownهای هر قدم ==========

        // دسته‌بندی‌ها
        [HttpGet]
        public JsonResult GetCategories()
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var categories = db.X_Categories
                    .Where(c => c.IsActive)
                    .Select(c => new { c.CategoryId, c.NameFa, c.CategoryType })
                    .ToList();
                return Json(categories, JsonRequestBehavior.AllowGet);
            }
        }

        // صفت‌ها
        [HttpGet]
        public JsonResult GetAttributes()
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var attributes = db.X_Attributes
                    .Where(a => a.IsActive)
                    .Select(a => new { a.AttributeId, a.NameFa, a.DataType })
                    .ToList();
                return Json(attributes, JsonRequestBehavior.AllowGet);
            }
        }

        // مقادیر یه صفت
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
        //[ValidateAntiForgeryToken]
        public JsonResult Save(ProductWizardViewModel model)
        {
            try
            {
                if (model == null || model.Step1 == null)
                    return Json(new { success = false, message = "اطلاعات ناقص است" });

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    // ============ ۱. X_Resources ============
                    var resource = new X_Resource
                    {
                        NameFa = model.Step1.NameFa,
                        NameEn = model.Step1.NameEn,
                        Description = model.Step1.Description,
                        ImageUrl = model.Step1.ImageUrl,
                        IsActive = model.Step1.ResourceIsActive,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Resources.InsertOnSubmit(resource);
                    db.SubmitChanges(); // برای گرفتن ResourceId

                    // ============ ۲. X_Products ============
                    var product = new X_Product
                    {
                        FK_ResourceId = resource.ResourceId,
                        ProductCode = model.Step1.ProductCode,
                        Barcode = model.Step1.Barcode,
                        IsActive = model.Step1.ProductIsActive,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Products.InsertOnSubmit(product);
                    db.SubmitChanges(); // برای گرفتن ProductId

                    // ============ ۳. X_ProductVariants ============
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
                            db.SubmitChanges(); // برای گرفتن ProductVariantId
                            variantIds.Add(variant.ProductVariantId);
                        }
                    }

                    // ============ ۴. X_ResourceCategories ============
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

                    // ============ ۵. X_ProductVariantAttributes ============
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

                    // ============ ۶. X_ResourceAttributes ============
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

                    // ============ ۷. X_ResourceImages ============
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

                    // ذخیره همه
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
    }
}

 