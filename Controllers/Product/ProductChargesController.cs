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
    public class ProductChargesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ProductCharges
        public ActionResult Index()
        {
            var sD_ProductCharges = db.SD_ProductCharges.Include(s => s.BD_Tax).Include(s => s.SD_Product);
            return View(sD_ProductCharges.ToList());
        }

        // GET: ProductCharges/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ProductCharges sD_ProductCharges = db.SD_ProductCharges.Find(id);
            if (sD_ProductCharges == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductCharges);
        }

        // GET: ProductCharges/Create
        public ActionResult Create()
        {
            ViewBag.SaleTaxID = new SelectList(db.BD_Tax, "ID", "Title");
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name");
            return View();
        }

        // POST: ProductCharges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProductID,SaleTaxID,BuyInvoiceNumber,Charge,BuyPrice,BuyCount,PercentInterest,PercentWages")] SD_ProductCharges sD_ProductCharges)
        {
            if (ModelState.IsValid)
            {
                db.SD_ProductCharges.Add(sD_ProductCharges);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SaleTaxID = new SelectList(db.BD_Tax, "ID", "Title", sD_ProductCharges.SaleTaxID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", sD_ProductCharges.ProductID);
            return View(sD_ProductCharges);
        }

        // GET: ProductCharges/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ProductCharges sD_ProductCharges = db.SD_ProductCharges.Find(id);
            if (sD_ProductCharges == null)
            {
                return HttpNotFound();
            }
            ViewBag.SaleTaxID = new SelectList(db.BD_Tax, "ID", "Title", sD_ProductCharges.SaleTaxID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", sD_ProductCharges.ProductID);
            return View(sD_ProductCharges);
        }

        // POST: ProductCharges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductID,SaleTaxID,BuyInvoiceNumber,Charge,BuyPrice,BuyCount,PercentInterest,PercentWages")] SD_ProductCharges sD_ProductCharges)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ProductCharges).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SaleTaxID = new SelectList(db.BD_Tax, "ID", "Title", sD_ProductCharges.SaleTaxID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", sD_ProductCharges.ProductID);
            return View(sD_ProductCharges);
        }

        // GET: ProductCharges/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ProductCharges sD_ProductCharges = db.SD_ProductCharges.Find(id);
            if (sD_ProductCharges == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductCharges);
        }

        // POST: ProductCharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_ProductCharges sD_ProductCharges = db.SD_ProductCharges.Find(id);
            db.SD_ProductCharges.Remove(sD_ProductCharges);
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
