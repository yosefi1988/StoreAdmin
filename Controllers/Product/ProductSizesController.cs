using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers.Product
{
    public class ProductSizesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ProductSizes
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                var sD_ProductSizes = db.SD_ProductSizes
                                            .Include(s => s.BD_SizeTypes)
                                            .Include(s => s.SD_ProductChargesProperties);
                return View(sD_ProductSizes.ToList());
            }
            var SD_ProductSizes_id = db.SD_ProductSizes
                                   .Include(s => s.SD_ProductChargesProperties)
                                   .Where(s => s.ProductChargePropertiesID == id);

            if (SD_ProductSizes_id == null)
            {
                return HttpNotFound();
            }
            return View(SD_ProductSizes_id.ToList());
        }

        // GET: ProductSizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.Find(id);
            SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.FirstOrDefault(ps => ps.ID == id);

            if (sD_ProductSizes == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductSizes);
        }

        // GET: ProductSizes/Create
        public ActionResult Create()
        {
            ViewBag.SizeID = new SelectList(db.BD_SizeTypes, "ID", "Title");
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID");
            return View();
        }

        // POST: ProductSizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SizeID,ProductChargePropertiesID,Value,Description")] SD_ProductSizes sD_ProductSizes)
        {
            if (ModelState.IsValid)
            {
                db.SD_ProductSizes.Add(sD_ProductSizes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SizeID = new SelectList(db.BD_SizeTypes, "ID", "Title", sD_ProductSizes.SizeID);
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_ProductSizes.ProductChargePropertiesID);
            return View(sD_ProductSizes);
        }

        // GET: ProductSizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.Find(id);
            SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.FirstOrDefault(ps => ps.ID == id);

            if (sD_ProductSizes == null)
            {
                return HttpNotFound();
            }
            ViewBag.SizeID = new SelectList(db.BD_SizeTypes, "ID", "Title", sD_ProductSizes.SizeID);
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_ProductSizes.ProductChargePropertiesID);
            return View(sD_ProductSizes);
        }

        // POST: ProductSizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SizeID,ProductChargePropertiesID,Value,Description")] SD_ProductSizes sD_ProductSizes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ProductSizes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SizeID = new SelectList(db.BD_SizeTypes, "ID", "Title", sD_ProductSizes.SizeID);
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_ProductSizes.ProductChargePropertiesID);
            return View(sD_ProductSizes);
        }

        // GET: ProductSizes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.Find(id);
            SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.FirstOrDefault(ps => ps.ID == id);

            if (sD_ProductSizes == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductSizes);
        }

        // POST: ProductSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.Find(id);
            SD_ProductSizes sD_ProductSizes = db.SD_ProductSizes.FirstOrDefault(ps => ps.ID == id);

            db.SD_ProductSizes.Remove(sD_ProductSizes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
