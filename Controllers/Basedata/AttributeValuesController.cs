using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;
using WebApplicationStoreAdmin.Models.ViewModel;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class AttributeValuesController : Controller
    {
        // ============ INDEX ============
 
        public ActionResult Index()
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from av in db.X_AttributeValues
                         join a in db.X_Attributes on av.FK_AttributeId equals a.AttributeId into aj
                         from a in aj.DefaultIfEmpty()
                         orderby a.NameFa, av.SortOrder
                         select new AttributeValueViewModel
                         {
                             AttributeValueId = av.AttributeValueId,
                             FK_AttributeId = av.FK_AttributeId,
                             AttributeName = a != null ? a.NameFa : null,
                             ValueFa = av.ValueFa,
                             ValueEn = av.ValueEn,
                             Code = av.Code,
                             SortOrder = av.SortOrder,
                             IsActive = av.IsActive,
                             CreatedAt = av.CreatedAt
                         }).ToList();

            return View(model);
        }


        // ============ DETAILS ============
        public ActionResult Details(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from av in db.X_AttributeValues
                         join a in db.X_Attributes on av.FK_AttributeId equals a.AttributeId into aj
                         from a in aj.DefaultIfEmpty()
                         where av.AttributeValueId == id
                         select new AttributeValueViewModel
                         {
                             AttributeValueId = av.AttributeValueId,
                             FK_AttributeId = av.FK_AttributeId,
                             AttributeName = a != null ? a.NameFa : null,
                             ValueFa = av.ValueFa,
                             ValueEn = av.ValueEn,
                             Code = av.Code,
                             SortOrder = av.SortOrder,
                             IsActive = av.IsActive,
                             CreatedAt = av.CreatedAt
                         }).FirstOrDefault();

            if (model == null) return HttpNotFound();
            return View(model);
        }

        // ============ CREATE (GET) ============
        public ActionResult Create()
        {
            var db = new DataClassesDatabaseDataContext();
            ViewBag.FK_AttributeId = new SelectList(
                db.X_Attributes.Where(a => a.IsActive).ToList(),
                "AttributeId", "NameFa");
            return View();
        }

        // ============ CREATE (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AttributeValueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var entity = new X_AttributeValue
                {
                    FK_AttributeId = model.FK_AttributeId,
                    ValueFa = model.ValueFa,
                    ValueEn = model.ValueEn,
                    Code = model.Code,
                    SortOrder = model.SortOrder,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.Now
                };

                db.X_AttributeValues.InsertOnSubmit(entity);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_AttributeId = new SelectList(db2.X_Attributes.Where(a => a.IsActive).ToList(), "AttributeId", "NameFa", model.FK_AttributeId);
            return View(model);
        }

        // ============ EDIT (GET) ============
        public ActionResult Edit(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var entity = db.X_AttributeValues.FirstOrDefault(x => x.AttributeValueId == id);
            if (entity == null) return HttpNotFound();

            var model = new AttributeValueViewModel
            {
                AttributeValueId = entity.AttributeValueId,
                FK_AttributeId = entity.FK_AttributeId,
                ValueFa = entity.ValueFa,
                ValueEn = entity.ValueEn,
                Code = entity.Code,
                SortOrder = entity.SortOrder,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };

            ViewBag.FK_AttributeId = new SelectList(db.X_Attributes.Where(a => a.IsActive).ToList(), "AttributeId", "NameFa", entity.FK_AttributeId);
            return View(model);
        }

        // ============ EDIT (POST) ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AttributeValueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DataClassesDatabaseDataContext();

                var entity = db.X_AttributeValues.FirstOrDefault(x => x.AttributeValueId == model.AttributeValueId);
                if (entity == null) return HttpNotFound();

                entity.FK_AttributeId = model.FK_AttributeId;
                entity.ValueFa = model.ValueFa;
                entity.ValueEn = model.ValueEn;
                entity.Code = model.Code;
                entity.SortOrder = model.SortOrder;
                entity.IsActive = model.IsActive;

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            var db2 = new DataClassesDatabaseDataContext();
            ViewBag.FK_AttributeId = new SelectList(db2.X_Attributes.Where(a => a.IsActive).ToList(), "AttributeId", "NameFa", model.FK_AttributeId);
            return View(model);
        }

        // ============ DELETE (GET) ============
        public ActionResult Delete(int id)
        {
            var db = new DataClassesDatabaseDataContext();

            var model = (from av in db.X_AttributeValues
                         join a in db.X_Attributes on av.FK_AttributeId equals a.AttributeId into aj
                         from a in aj.DefaultIfEmpty()
                         where av.AttributeValueId == id
                         select new AttributeValueViewModel
                         {
                             AttributeValueId = av.AttributeValueId,
                             FK_AttributeId = av.FK_AttributeId,
                             AttributeName = a != null ? a.NameFa : null,
                             ValueFa = av.ValueFa,
                             ValueEn = av.ValueEn,
                             Code = av.Code,
                             SortOrder = av.SortOrder,
                             IsActive = av.IsActive,
                             CreatedAt = av.CreatedAt
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

            var entity = db.X_AttributeValues.FirstOrDefault(x => x.AttributeValueId == id);
            if (entity != null)
            {
                db.X_AttributeValues.DeleteOnSubmit(entity);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }

}
