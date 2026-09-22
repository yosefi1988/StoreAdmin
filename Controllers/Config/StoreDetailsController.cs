using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers
{
    public class StoreDetailsController : Controller
    {
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();
            var model = db.X_StoreSettings.ToList();
            return View(model);
        }


        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();
            var item = db.X_StoreSettings.FirstOrDefault(x => x.StoreSettingId == id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        #region Create


        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();
            ViewBag.FK_CountryId = new SelectList(db.X_Countries.Where(c => c.IsActive), "CountryId", "NameFa");
            ViewBag.FK_ProvinceId = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.FK_CityId = new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(X_StoreSetting model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();
                model.CreatedAt = DateTime.Now;
                model.IsActive = true;
                db.X_StoreSettings.InsertOnSubmit(model);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_CountryId = new SelectList(db2.X_Countries.Where(c => c.IsActive), "CountryId", "NameFa", model.FK_CountryId);
            ViewBag.FK_ProvinceId = new SelectList(db2.X_Provinces.Where(p => p.FK_CountryId == model.FK_CountryId && p.IsActive), "ProvinceId", "NameFa", model.FK_ProvinceId);
            ViewBag.FK_CityId = new SelectList(db2.X_Cities.Where(c => c.FK_ProvinceId == model.FK_ProvinceId && c.IsActive), "CityId", "NameFa", model.FK_CityId);
            return View(model);
        }

        // اکشن‌های AJAX برای آبشاری
        public JsonResult GetProvinces(int countryId)
        {
            var db = new DataClassesDatabaseDataContext();
            var provinces = db.X_Provinces
                .Where(p => p.FK_CountryId == countryId && p.IsActive)
                .Select(p => new { Id = p.ProvinceId, Name = p.NameFa })
                .ToList();
            return Json(provinces, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCities(int provinceId)
        {
            var db = new DataClassesDatabaseDataContext();
            var cities = db.X_Cities
                .Where(c => c.FK_ProvinceId == provinceId && c.IsActive)
                .Select(c => new { Id = c.CityId, Name = c.NameFa })
                .ToList();
            return Json(cities, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region delete

        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();
            var item = db.X_StoreSettings.FirstOrDefault(x => x.StoreSettingId == id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var db = new DataClassesDatabaseDataContext();
            var item = db.X_StoreSettings.FirstOrDefault(x => x.StoreSettingId == id);
            if (item == null) return HttpNotFound();

            db.X_StoreSettings.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        #endregion



        #region edit

        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();
            var item = db.X_StoreSettings.FirstOrDefault(x => x.StoreSettingId == id);
            if (item == null) return HttpNotFound();

            ViewBag.FK_CountryId = new SelectList(
                db.X_Countries.Where(c => c.IsActive).ToList(),
                "CountryId", "NameFa",
                item.FK_CountryId);

            ViewBag.FK_ProvinceId = new SelectList(
                db.X_Provinces.Where(p => p.FK_CountryId == item.FK_CountryId && p.IsActive).ToList(),
                "ProvinceId", "NameFa",
                item.FK_ProvinceId);

            ViewBag.FK_CityId = new SelectList(
                db.X_Cities.Where(c => c.FK_ProvinceId == item.FK_ProvinceId && c.IsActive).ToList(),
                "CityId", "NameFa",
                item.FK_CityId);

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(X_StoreSetting model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();
                var item = db.X_StoreSettings.FirstOrDefault(x => x.StoreSettingId == model.StoreSettingId);
                if (item == null) return HttpNotFound();

                item.StoreNameFa = model.StoreNameFa;
                item.StoreNameEn = model.StoreNameEn;
                item.LegalName = model.LegalName;
                item.RegistrationNumber = model.RegistrationNumber;
                item.NationalId = model.NationalId;
                item.EconomicCode = model.EconomicCode;
                item.PhoneNumber = model.PhoneNumber;
                item.MobileNumber = model.MobileNumber;
                item.FaxNumber = model.FaxNumber;
                item.Email = model.Email;
                item.Website = model.Website;
                item.FK_CountryId = model.FK_CountryId;
                item.FK_ProvinceId = model.FK_ProvinceId;
                item.FK_CityId = model.FK_CityId;
                item.Address = model.Address;
                item.PostalCode = model.PostalCode;
                item.Latitude = model.Latitude;
                item.Longitude = model.Longitude;
                item.LogoUrl = model.LogoUrl;
                item.FaviconUrl = model.FaviconUrl;
                item.Description = model.Description;
                item.InstagramUrl = model.InstagramUrl;
                item.TelegramUrl = model.TelegramUrl;
                item.WhatsAppNumber = model.WhatsAppNumber;
                item.LinkedInUrl = model.LinkedInUrl;
                item.WorkingHours = model.WorkingHours;
                item.SupportPhone = model.SupportPhone;
                item.SupportEmail = model.SupportEmail;
                item.IsActive = model.IsActive;
                item.UpdatedAt = DateTime.Now;

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_CountryId = new SelectList(db2.X_Countries.Where(c => c.IsActive).ToList(), "CountryId", "NameFa", model.FK_CountryId);
            ViewBag.FK_ProvinceId = new SelectList(db2.X_Provinces.Where(p => p.FK_CountryId == model.FK_CountryId && p.IsActive).ToList(), "ProvinceId", "NameFa", model.FK_ProvinceId);
            ViewBag.FK_CityId = new SelectList(db2.X_Cities.Where(c => c.FK_ProvinceId == model.FK_ProvinceId && c.IsActive).ToList(), "CityId", "NameFa", model.FK_CityId);

            return View(model);
        }
         

        #endregion


    }
}
