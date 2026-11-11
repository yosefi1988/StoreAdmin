using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AllowAnonymous]
    public class DentalServicesController : BaseController
    {
        // کدهای خدمت دندانپزشکی — قابل تنظیم
        private static readonly string[] DentalServiceCodes = new[]
        {
            "DENTAL_EXAM", "DENTAL_CLEANING", "DENTAL_FILLING", "DENTAL_ROOT_CANAL",
            "DENTAL_EXTRACTION", "DENTAL_SURGERY", "DENTAL_IMPLANT", "DENTAL_CROWN",
            "DENTAL_VENEER", "DENTAL_WHITENING", "DENTAL_CHILD", "DENTAL_GUM"
        };

        // ==================== صفحه لیست ====================
        public ActionResult Index(string search = null)
        {
            ViewBag.Search = search;
            return View();
        }

        // ==================== AJAX: لیست خدمات ====================
        [HttpGet]
        public JsonResult GetDentalServices(int page = 1, int pageSize = 20, string search = null)
        {
            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    //var query =
                    //    from s in db.X_Services
                    //    join r in db.X_Resources on s.FK_ResourceId equals r.ResourceId
                    //    where s.IsActive && r.IsActive
                    //          //&& DentalServiceCodes.Contains(s.ServiceCode)
                    //    select new
                    //    {
                    //        s.ServiceId,
                    //        s.ServiceCode,
                    //        r.ResourceId,
                    //        r.NameFa,
                    //        r.NameEn,
                    //        r.Description,
                    //        r.ImageUrl,
                    //        Price = db.X_ServicePrices
                    //                   .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                    //                   .Select(p => (decimal?)p.Price)
                    //                   .FirstOrDefault(),
                    //        UnitName = db.X_ServicePrices
                    //                   .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                    //                   .Select(p => p.UnitName)
                    //                   .FirstOrDefault(),
                    //        ImageFromGallery = db.X_ResourceImages
                    //                   .Where(i => i.FK_ResourceId == r.ResourceId && i.IsActive && i.IsMain)
                    //                   .Select(i => i.ImageUrl)
                    //                   .FirstOrDefault(),
                    //        ScheduleCount = db.X_ServiceSchedules
                    //                   .Count(sch => sch.FK_ServiceId == s.ServiceId
                    //                              && sch.IsActive
                    //                              && sch.StartDateTime > DateTime.Now
                    //                              && sch.ReservedCount < sch.Capacity),
                    //        s.CreatedAt
                    //    };

                    var query =
                        from s in db.X_Services
                        join r in db.X_Resources on s.FK_ResourceId equals r.ResourceId
                        join rc in db.X_ResourceCategories on r.ResourceId equals rc.FK_ResourceId
                        where s.IsActive && r.IsActive
                              && rc.FK_CategoryId == 2
                        select new
                        {
                            s.ServiceId,
                            s.ServiceCode,
                            r.ResourceId,
                            r.NameFa,
                            r.NameEn,
                            r.Description,
                            r.ImageUrl,
                            Price = db.X_ServicePrices
                                       .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                                       .Select(p => (decimal?)p.Price)
                                       .FirstOrDefault(),
                            UnitName = db.X_ServicePrices
                                       .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                                       .Select(p => p.UnitName)
                                       .FirstOrDefault(),
                            ImageFromGallery = db.X_ResourceImages
                                       .Where(i => i.FK_ResourceId == r.ResourceId && i.IsActive && i.IsMain)
                                       .Select(i => i.ImageUrl)
                                       .FirstOrDefault(),
                            ScheduleCount = db.X_ServiceSchedules
                                       .Count(sch => sch.FK_ServiceId == s.ServiceId
                                                  && sch.IsActive
                                                  && sch.StartDateTime > DateTime.Now
                                                  && sch.ReservedCount < sch.Capacity),
                            s.CreatedAt

                        };

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        search = search.Trim();
                        query = query.Where(x =>
                            x.NameFa.Contains(search) ||
                            x.NameEn.Contains(search) ||
                            x.ServiceCode.Contains(search) ||
                            x.Description.Contains(search));
                    }

                    var total = query.Count();
                    var totalPages = (int)Math.Ceiling((double)total / pageSize);

                    var data = query
                        .OrderBy(x => x.ServiceId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList()
                        .Select(x => new
                        {
                            x.ServiceId,
                            x.ServiceCode,
                            x.NameFa,
                            x.NameEn,
                            x.Description,
                            ImageUrl = !string.IsNullOrEmpty(x.ImageFromGallery) ? x.ImageFromGallery : x.ImageUrl,
                            Price = x.Price ?? 0,
                            UnitName = x.UnitName ?? "جلسه",
                            AvailableSlots = x.ScheduleCount,
                            x.CreatedAt
                        })
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

        // ==================== جزئیات خدمت ====================
        public ActionResult Details(int id)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var service = (from s in db.X_Services
                               join r in db.X_Resources on s.FK_ResourceId equals r.ResourceId
                               where s.ServiceId == id && s.IsActive
                               select new DentalServiceDetailsViewModel
                               {
                                   ServiceId = s.ServiceId,
                                   ServiceCode = s.ServiceCode,
                                   ResourceId = r.ResourceId,
                                   NameFa = r.NameFa,
                                   NameEn = r.NameEn,
                                   Description = r.Description,
                                   ImageUrl = r.ImageUrl,
                                   CreatedAt = s.CreatedAt,
                                   Price = db.X_ServicePrices
                                              .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                                              .Select(p => (decimal?)p.Price).FirstOrDefault(),
                                   PriceType = db.X_ServicePrices
                                              .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                                              .Select(p => p.PriceType).FirstOrDefault(),
                                   UnitName = db.X_ServicePrices
                                              .Where(p => p.FK_ServiceId == s.ServiceId && p.IsActive)
                                              .Select(p => p.UnitName).FirstOrDefault()
                               }).FirstOrDefault();

                if (service == null)
                    return HttpNotFound();

                // گالری تصاویر
                service.Images = db.X_ResourceImages
                    .Where(i => i.FK_ResourceId == service.ResourceId && i.IsActive)
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

                // ویژگی‌ها
                service.Attributes = (from ra in db.X_ResourceAttributes
                                      join a in db.X_Attributes on ra.FK_AttributeId equals a.AttributeId
                                      join av in db.X_AttributeValues on ra.FK_AttributeValueId equals av.AttributeValueId into avJoin
                                      from av in avJoin.DefaultIfEmpty()
                                      where ra.FK_ResourceId == service.ResourceId
                                      select new AttributeDto
                                      {
                                          AttributeId = a.AttributeId,
                                          NameFa = a.NameFa,
                                          DataType = a.DataType,
                                          Value = av != null ? av.ValueFa
                                                : (ra.ValueText ?? (ra.ValueNumber.HasValue
                                                    ? ra.ValueNumber.Value.ToString()
                                                    : (ra.ValueBoolean.HasValue
                                                        ? (ra.ValueBoolean.Value ? "بله" : "خیر")
                                                        : "-")))
                                      }).ToList();

                // اسلات‌های زمانی آینده، گروه‌بندی بر اساس ارائه‌دهنده
                var futureSchedules = db.X_ServiceSchedules
                    .Where(sch =>  sch.FK_ServiceId == service.ServiceId  && 
                                sch.IsActive)
                              // && sch.StartDateTime > DateTime.Now)
                    .OrderBy(sch => sch.StartDateTime)
                    .Select(sch => new ServiceScheduleDto
                    {
                        ScheduleId = sch.ScheduleId,
                        StartDateTime = sch.StartDateTime,
                        EndDateTime = sch.EndDateTime,
                        Capacity = sch.Capacity,
                        ReservedCount = sch.ReservedCount,
                        RemainingCapacity = sch.Capacity - sch.ReservedCount,
                        ProviderId = (int) sch.FK_ProviderId,
                        ProviderName = db.X_Users
                            .Where(u => u.UserId == sch.FK_ProviderId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault()
                    }).ToList();

                service.ProviderGroups = futureSchedules
                    .GroupBy(s => new { s.ProviderId, s.ProviderName })
                    .Select(g => new ProviderGroupDto
                    {
                        ProviderId = g.Key.ProviderId,
                        ProviderName = g.Key.ProviderName,
                        Schedules = g.ToList()
                    }).ToList();

                return View(service);
            }
        }
    }
}