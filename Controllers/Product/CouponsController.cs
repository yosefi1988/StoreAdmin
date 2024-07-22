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
    public class CouponsController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Coupons
        public ActionResult Index()
        {
            return View(db.SD_Coupons.ToList());
        }

        // GET: Coupons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Coupons sD_Coupons = db.SD_Coupons.Find(id);
            if (sD_Coupons == null)
            {
                return HttpNotFound();
            }
            return View(sD_Coupons);
        }

        // GET: Coupons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,CouponPercent,MaxRialValue,ExpireDate")] SD_Coupons sD_Coupons)
        {
            if (ModelState.IsValid)
            {
                db.SD_Coupons.Add(sD_Coupons);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sD_Coupons);
        }

        // GET: Coupons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Coupons sD_Coupons = db.SD_Coupons.Find(id);
            if (sD_Coupons == null)
            {
                return HttpNotFound();
            }
            return View(sD_Coupons);
        }

        // POST: Coupons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,CouponPercent,MaxRialValue,ExpireDate")] SD_Coupons sD_Coupons)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Coupons).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sD_Coupons);
        }

        // GET: Coupons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Coupons sD_Coupons = db.SD_Coupons.Find(id);
            if (sD_Coupons == null)
            {
                return HttpNotFound();
            }
            return View(sD_Coupons);
        }

        // POST: Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Coupons sD_Coupons = db.SD_Coupons.Find(id);
            db.SD_Coupons.Remove(sD_Coupons);
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
