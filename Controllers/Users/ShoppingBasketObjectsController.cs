using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers.Users
{
    public class ShoppingBasketObjectsController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ShoppingBasketObjects
        public ActionResult Index()
        {
            var sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Include(s => s.SD_ProductCharges).Include(s => s.SD_ShoppingBasket);
            return View(sD_ShoppingBasketObjects.ToList());
        }

        // GET: ShoppingBasketObjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            if (sD_ShoppingBasketObjects == null)
            {
                return HttpNotFound();
            }
            return View(sD_ShoppingBasketObjects);
        }

        // GET: ShoppingBasketObjects/Create
        public ActionResult Create()
        {
            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber");
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID");
            return View();
        }

        // POST: ShoppingBasketObjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ShoppingBasketID,ProductChargeID,Price,OldPrice,Count,AddDate")] SD_ShoppingBasketObjects sD_ShoppingBasketObjects)
        {
            if (ModelState.IsValid)
            {
                db.SD_ShoppingBasketObjects.Add(sD_ShoppingBasketObjects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber", sD_ShoppingBasketObjects.ProductChargeID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_ShoppingBasketObjects.ShoppingBasketID);
            return View(sD_ShoppingBasketObjects);
        }

        // GET: ShoppingBasketObjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            if (sD_ShoppingBasketObjects == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber", sD_ShoppingBasketObjects.ProductChargeID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_ShoppingBasketObjects.ShoppingBasketID);
            return View(sD_ShoppingBasketObjects);
        }

        // POST: ShoppingBasketObjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShoppingBasketID,ProductChargeID,Price,OldPrice,Count,AddDate")] SD_ShoppingBasketObjects sD_ShoppingBasketObjects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ShoppingBasketObjects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber", sD_ShoppingBasketObjects.ProductChargeID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_ShoppingBasketObjects.ShoppingBasketID);
            return View(sD_ShoppingBasketObjects);
        }

        // GET: ShoppingBasketObjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            if (sD_ShoppingBasketObjects == null)
            {
                return HttpNotFound();
            }
            return View(sD_ShoppingBasketObjects);
        }

        // POST: ShoppingBasketObjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            db.SD_ShoppingBasketObjects.Remove(sD_ShoppingBasketObjects);
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
