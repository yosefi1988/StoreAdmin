using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ProductVariantAttributesController : Controller
    {
        // ============ INDEX ============
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var variants = (from v in db.X_ProductVariants
                            join p in db.X_Products on v.FK_ProductId equals p.ProductId into pj
                            from p in pj.DefaultIfEmpty()
                            join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                            from r in rj.DefaultIfEmpty()
                            orderby r.NameFa, v.SKU
                            select new
                            {
                                v.ProductVariantId,
                                v.SKU,
                                ProductName = r != null ? r.NameFa : null,
                                ProductCode = p != null ? p.ProductCode : null,
                                v.Price,
                                v.StockQuantity,
                                v.IsActive
                            }).ToList();

            var variantIds = variants.Select(v => v.ProductVariantId).ToList();
            var allAttrs = (from pva in db.X_ProductVariantAttributes
                            join a in db.X_Attributes on pva.FK_AttributeId equals a.AttributeId
                            join av in db.X_AttributeValues on pva.FK_AttributeValueId equals av.AttributeValueId
                            where variantIds.Contains(pva.FK_ProductVariantId)
                            select new
                            {
                                pva.ProductVariantAttributeId,
                                pva.FK_ProductVariantId,
                                pva.FK_AttributeId,
                                AttributeName = a.NameFa,
                                pva.FK_AttributeValueId,
                                AttributeValueName = av.ValueFa
                            }).ToList();

            var model = variants.Select(v => new ProductVariantAttributeViewModel
            {
                ProductVariantId = v.ProductVariantId,
                SKU = v.SKU,
                ProductName = v.ProductName,
                ProductCode = v.ProductCode,
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                VariantIsActive = v.IsActive,
                Attributes = allAttrs
                    .Where(a => a.FK_ProductVariantId == v.ProductVariantId)
                    .Select(a => new VariantAttributeRow
                    {
                        ProductVariantAttributeId = a.ProductVariantAttributeId,
                        FK_AttributeId = a.FK_AttributeId,
                        AttributeName = a.AttributeName,
                        FK_AttributeValueId = a.FK_AttributeValueId,
                        AttributeValueName = a.AttributeValueName
                    }).ToList()
            }).ToList();

            return View(model);
        }

        // ============ DETAILS ============
        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var v = (from vv in db.X_ProductVariants
                     join p in db.X_Products on vv.FK_ProductId equals p.ProductId into pj
                     from p in pj.DefaultIfEmpty()
                     join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId into rj
                     from r in rj.DefaultIfEmpty()
                     where vv.ProductVariantId == id
                     select new
                     {
                         vv.ProductVariantId,
                         vv.SKU,
                         ProductName = r != null ? r.NameFa : null,
                         vv.Price,
                         vv.StockQuantity,
                         vv.IsActive
                     }).FirstOrDefault();

            if (v == null) return HttpNotFound();

            var model = new ProductVariantAttributeViewModel
            {
                ProductVariantId = v.ProductVariantId,
                SKU = v.SKU,
                ProductName = v.ProductName,
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                VariantIsActive = v.IsActive,
                Attributes = (from pva in db.X_ProductVariantAttributes
                              join a in db.X_Attributes on pva.FK_AttributeId equals a.AttributeId
                              join av in db.X_AttributeValues on pva.FK_AttributeValueId equals av.AttributeValueId
                              where pva.FK_ProductVariantId == id
                              select new VariantAttributeRow
                              {
                                  ProductVariantAttributeId = pva.ProductVariantAttributeId,
                                  FK_AttributeId = pva.FK_AttributeId,
                                  AttributeName = a.NameFa,
                                  FK_AttributeValueId = pva.FK_AttributeValueId,
                                  AttributeValueName = av.ValueFa
                              }).ToList()
            };

            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();

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
                        Group = group
                    });
                }
            }

            ViewBag.ProductVariantId = items;

            ViewBag.FK_AttributeId = new SelectList(
                db.X_Attributes.Where(a => a.IsActive).ToList(),
                "AttributeId", "NameFa");

            return View();
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int ProductVariantId, int[] FK_AttributeId, int[] FK_AttributeValueId)
        {
            if (FK_AttributeId == null) FK_AttributeId = new int[0];
            if (FK_AttributeValueId == null) FK_AttributeValueId = new int[0];

            var db = new DataClassesDatabaseDataContext();

            // ═══ اعتبارسنجی ═══
            var validRows = new List<Tuple<int, int>>();
            for (int i = 0; i < FK_AttributeId.Length; i++)
            {
                var attrId = FK_AttributeId[i];
                var valId = (i < FK_AttributeValueId.Length) ? FK_AttributeValueId[i] : 0;

                if (attrId <= 0 || valId <= 0) continue;

                validRows.Add(Tuple.Create(attrId, valId));
            }

            if (!validRows.Any())
            {
                ModelState.AddModelError("", "حداقل یک صفت معتبر انتخاب کنید.");
            }

            var attrIds = validRows.Select(r => r.Item1).Distinct().ToList();
            var existingAttrs = db.X_Attributes
                .Where(a => attrIds.Contains(a.AttributeId))
                .Select(a => a.AttributeId).ToList();
            var missingAttrs = attrIds.Except(existingAttrs).ToList();
            if (missingAttrs.Any())
                ModelState.AddModelError("", "صفت‌های نامعتبر: " + string.Join(", ", missingAttrs));

            var valIds = validRows.Select(r => r.Item2).Distinct().ToList();
            var valueMap = db.X_AttributeValues
                .Where(v => valIds.Contains(v.AttributeValueId))
                .Select(v => new { v.AttributeValueId, v.FK_AttributeId })
                .ToDictionary(v => v.AttributeValueId, v => v.FK_AttributeId);

            foreach (var row in validRows)
            {
                if (!valueMap.ContainsKey(row.Item2))
                    ModelState.AddModelError("", $"مقدار نامعتبر: {row.Item2}");
                else if (valueMap[row.Item2] != row.Item1)
                    ModelState.AddModelError("", $"مقدار {row.Item2} به صفت {row.Item1} تعلق ندارد.");
            }

            var dup = validRows.GroupBy(r => r.Item1).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dup.Any())
                ModelState.AddModelError("", "صفت‌های تکراری: " + string.Join(", ", dup));

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var row in validRows)
                    {
                        bool exists = db.X_ProductVariantAttributes.Any(x =>
                            x.FK_ProductVariantId == ProductVariantId &&
                            x.FK_AttributeId == row.Item1);

                        if (!exists)
                        {
                            db.X_ProductVariantAttributes.InsertOnSubmit(new X_ProductVariantAttribute
                            {
                                FK_ProductVariantId = ProductVariantId,
                                FK_AttributeId = row.Item1,
                                FK_AttributeValueId = row.Item2
                            });
                        }
                    }

                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "خطا در ذخیره: " + ex.Message);
                }
            }

            ViewBag.ProductVariantId = new SelectList(
                db.X_ProductVariants.Select(v => new { v.ProductVariantId, Name = v.SKU }).ToList(),
                "ProductVariantId", "Name", ProductVariantId);
            ViewBag.FK_AttributeId = new SelectList(
                db.X_Attributes.Where(a => a.IsActive).ToList(),
                "AttributeId", "NameFa");

            return View();
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var variant = db.X_ProductVariants.FirstOrDefault(v => v.ProductVariantId == id);
            if (variant == null) return HttpNotFound();

            var rows = (from pva in db.X_ProductVariantAttributes
                        join a in db.X_Attributes on pva.FK_AttributeId equals a.AttributeId
                        join av in db.X_AttributeValues on pva.FK_AttributeValueId equals av.AttributeValueId
                        where pva.FK_ProductVariantId == id
                        select new VariantAttributeRow
                        {
                            ProductVariantAttributeId = pva.ProductVariantAttributeId,
                            FK_AttributeId = pva.FK_AttributeId,
                            AttributeName = a.NameFa,
                            FK_AttributeValueId = pva.FK_AttributeValueId,
                            AttributeValueName = av.ValueFa
                        }).ToList();

            var model = new ProductVariantAttributeViewModel
            {
                ProductVariantId = variant.ProductVariantId,
                SKU = variant.SKU,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                VariantIsActive = variant.IsActive,
                Attributes = rows
            };

            ViewBag.FK_AttributeId = new SelectList(
                db.X_Attributes.Where(a => a.IsActive).ToList(),
                "AttributeId", "NameFa");

            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int ProductVariantId, int[] FK_AttributeId, int[] FK_AttributeValueId)
        {
            if (FK_AttributeId == null) FK_AttributeId = new int[0];
            if (FK_AttributeValueId == null) FK_AttributeValueId = new int[0];

            var db = new DataClassesDatabaseDataContext();

            // ═══ گام ۱: ساخت لیست ردیف‌های معتبر ═══
            var validRows = new List<Tuple<int, int>>();
            for (int i = 0; i < FK_AttributeId.Length; i++)
            {
                var attrId = FK_AttributeId[i];
                var valId = (i < FK_AttributeValueId.Length) ? FK_AttributeValueId[i] : 0;

                if (attrId <= 0 || valId <= 0) continue;

                validRows.Add(Tuple.Create(attrId, valId));
            }

            // ═══ گام ۲: اعتبارسنجی ═══
            var errors = new List<string>();

            var attrIds = validRows.Select(r => r.Item1).Distinct().ToList();
            var existingAttrs = db.X_Attributes
                .Where(a => attrIds.Contains(a.AttributeId))
                .Select(a => a.AttributeId).ToList();
            var missingAttrs = attrIds.Except(existingAttrs).ToList();
            if (missingAttrs.Any())
                errors.Add("صفت‌های نامعتبر: " + string.Join(", ", missingAttrs));

            var valIds = validRows.Select(r => r.Item2).Distinct().ToList();
            var valueMap = db.X_AttributeValues
                .Where(v => valIds.Contains(v.AttributeValueId))
                .Select(v => new { v.AttributeValueId, v.FK_AttributeId })
                .ToDictionary(v => v.AttributeValueId, v => v.FK_AttributeId);

            foreach (var row in validRows)
            {
                if (!valueMap.ContainsKey(row.Item2))
                    errors.Add($"مقدار نامعتبر: {row.Item2}");
                else if (valueMap[row.Item2] != row.Item1)
                    errors.Add($"مقدار {row.Item2} به صفت {row.Item1} تعلق ندارد.");
            }

            var dup = validRows.GroupBy(r => r.Item1).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dup.Any())
                errors.Add("صفت‌های تکراری: " + string.Join(", ", dup));

            // ═══ گام ۳: اگه خطا بود، View رو برگردون ═══
            if (errors.Any())
            {
                foreach (var err in errors)
                    ModelState.AddModelError("", err);

                var variant = db.X_ProductVariants.FirstOrDefault(v => v.ProductVariantId == ProductVariantId);
                var model = new ProductVariantAttributeViewModel
                {
                    ProductVariantId = ProductVariantId,
                    SKU = variant?.SKU,
                    Price = variant?.Price,
                    StockQuantity = (int)variant?.StockQuantity,
                    VariantIsActive = variant?.IsActive ?? false,
                    Attributes = validRows.Select(r => new VariantAttributeRow
                    {
                        FK_AttributeId = r.Item1,
                        FK_AttributeValueId = r.Item2
                    }).ToList()
                };

                ViewBag.FK_AttributeId = new SelectList(
                    db.X_Attributes.Where(a => a.IsActive).ToList(),
                    "AttributeId", "NameFa");

                return View(model);
            }

            // ═══ گام ۴: حذف + درج (LINQ to SQL خودش Transaction می‌سازه) ═══
            try
            {
                var oldRows = db.X_ProductVariantAttributes
                    .Where(x => x.FK_ProductVariantId == ProductVariantId)
                    .ToList();

                if (oldRows.Any())
                    db.X_ProductVariantAttributes.DeleteAllOnSubmit(oldRows);

                foreach (var row in validRows)
                {
                    db.X_ProductVariantAttributes.InsertOnSubmit(new X_ProductVariantAttribute
                    {
                        FK_ProductVariantId = ProductVariantId,
                        FK_AttributeId = row.Item1,
                        FK_AttributeValueId = row.Item2
                    });
                }

                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "خطا در ذخیره: " + ex.Message);

                var variant = db.X_ProductVariants.FirstOrDefault(v => v.ProductVariantId == ProductVariantId);
                var model = new ProductVariantAttributeViewModel
                {
                    ProductVariantId = ProductVariantId,
                    SKU = variant?.SKU,
                    Price = variant?.Price,
                    StockQuantity = (int)variant?.StockQuantity,
                    VariantIsActive = variant?.IsActive ?? false,
                    Attributes = validRows.Select(r => new VariantAttributeRow
                    {
                        FK_AttributeId = r.Item1,
                        FK_AttributeValueId = r.Item2
                    }).ToList()
                };
                ViewBag.FK_AttributeId = new SelectList(
                    db.X_Attributes.Where(a => a.IsActive).ToList(),
                    "AttributeId", "NameFa");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var variant = db.X_ProductVariants.FirstOrDefault(v => v.ProductVariantId == id);
            if (variant == null) return HttpNotFound();

            var model = new ProductVariantAttributeViewModel
            {
                ProductVariantId = variant.ProductVariantId,
                SKU = variant.SKU,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                VariantIsActive = variant.IsActive,
                Attributes = (from pva in db.X_ProductVariantAttributes
                              join a in db.X_Attributes on pva.FK_AttributeId equals a.AttributeId
                              join av in db.X_AttributeValues on pva.FK_AttributeValueId equals av.AttributeValueId
                              where pva.FK_ProductVariantId == id
                              select new VariantAttributeRow
                              {
                                  ProductVariantAttributeId = pva.ProductVariantAttributeId,
                                  FK_AttributeId = pva.FK_AttributeId,
                                  AttributeName = a.NameFa,
                                  FK_AttributeValueId = pva.FK_AttributeValueId,
                                  AttributeValueName = av.ValueFa
                              }).ToList()
            };

            return View(model);
        }

        // ============ DELETE (POST) ============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var rows = db.X_ProductVariantAttributes.Where(x => x.FK_ProductVariantId == id).ToList();
            if (rows.Any())
            {
                db.X_ProductVariantAttributes.DeleteAllOnSubmit(rows);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        // ============ AJAX: گرفتن مقادیر یه Attribute ============
        public JsonResult GetAttributeValues(int attributeId)
        {
            var db = new DataClassesDatabaseDataContext();
            var values = db.X_AttributeValues
                .Where(v => v.FK_AttributeId == attributeId && v.IsActive)
                .OrderBy(v => v.SortOrder)
                .Select(v => new { Id = v.AttributeValueId, Name = v.ValueFa })
                .ToList();
            return Json(values, JsonRequestBehavior.AllowGet);
        }
    }
}