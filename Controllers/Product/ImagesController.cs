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
    public class ImagesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Images or Images Of id
        public ActionResult Index(int? id)
        { 
            if (id == null)
            {
                var sD_Images_all = db.SD_Images.Include(s => s.SD_ProductChargesProperties);
                return View(sD_Images_all.ToList());
            }
             
            var sD_Images_id = db.SD_Images
                       .Include(s => s.SD_ProductChargesProperties)
                       .Where(s => s.ProductChargePropertiesID == id);

            if (sD_Images_id == null)
            {
                return HttpNotFound();
            }
            return View(sD_Images_id.ToList());
        }


        // GET: Images/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Images sD_Images = db.SD_Images.Find(id);
            if (sD_Images == null)
            {
                return HttpNotFound();
            }
            return View(sD_Images);
        }

        // GET: Images/Create
        public ActionResult Create()
        {
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID");
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProductChargePropertiesID,ImageUrl,CreatedOn")] SD_Images sD_Images)
        {
            if (ModelState.IsValid)
            {
                db.SD_Images.Add(sD_Images);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_Images.ProductChargePropertiesID);
            return View(sD_Images);
        }

        // GET: Images/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Images sD_Images = db.SD_Images.Find(id);
            if (sD_Images == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_Images.ProductChargePropertiesID);
            return View(sD_Images);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductChargePropertiesID,ImageUrl,CreatedOn")] SD_Images sD_Images)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Images).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_Images.ProductChargePropertiesID);
            return View(sD_Images);
        }

        // GET: Images/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Images sD_Images = db.SD_Images.Find(id);
            if (sD_Images == null)
            {
                return HttpNotFound();
            }
            return View(sD_Images);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Images sD_Images = db.SD_Images.Find(id);
            db.SD_Images.Remove(sD_Images);
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
