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

            var sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects
                .Include(s => s.SD_ProductChargesProperties)
                .Include(s => s.SD_ShoppingBasket);


            var sD_ShoppingBaskets = db.SD_ShoppingBasket
                .Include(s => s.BD_ShoppingBasketTypes);
             

            var shoppingBaskets = sD_ShoppingBaskets.ToList(); 

            return View(sD_ShoppingBasketObjects.ToList());
        }

        // GET: ShoppingBasketObjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.FirstOrDefault(ps => ps.ID == id);


            if (sD_ShoppingBasketObjects == null)
            {
                return HttpNotFound();
            }
            return View(sD_ShoppingBasketObjects);
        }

        // GET: ShoppingBasketObjects/Create
        public ActionResult Create()
        {
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID");
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID");


            var productChargePropertiesForDDL = db.View_Admin_ProductChargePropertiesID.Select(p => new
                                                                    {
                                                                        ProductChargePropertiesID = p.ProductChargePropertiesID,
                                                                        CombinedText = p.ProductName + " (code:" + p.ProductCode + ") Invoice Number:" + p.BuyInvoiceNumber + " Buy Price:" + p.BuyPrice + " - color:" + p.Color
                                                                    }).OrderByDescending(x => x.ProductChargePropertiesID)
                                                                    .ToList();
            ViewBag.ProductChargePropertiesDDL = new SelectList(productChargePropertiesForDDL, "ProductChargePropertiesID", "CombinedText");


            var shoppingBasketForDDL = db.View_Admin_ShoppingBasketID.Select(p => new
                                                                    {
                                                                        ProductChargePropertiesID = p.ShoppingBasketID,
                                                                        CombinedText = p.Name + " " + p.Family + "  (type:" + p.BasketType + ") mobile:" + p.Mobile + " Email:" + p.Email 
                                                                    }).OrderByDescending(x => x.ProductChargePropertiesID)
                                                                    .ToList();
            ViewBag.ShoppingBasketForDDL = new SelectList(shoppingBasketForDDL, "ProductChargePropertiesID", "CombinedText");


            return View();
        }

        // POST: ShoppingBasketObjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ShoppingBasketID,ProductChargePropertiesID,Price,OldPrice,Count,AddDate")] SD_ShoppingBasketObjects sD_ShoppingBasketObjects)
        {
            if (ModelState.IsValid)
            {
                db.SD_ShoppingBasketObjects.Add(sD_ShoppingBasketObjects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_ShoppingBasketObjects.ProductChargePropertiesID);
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
            //SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.FirstOrDefault(ps => ps.ID == id);

            if (sD_ShoppingBasketObjects == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_ShoppingBasketObjects.ProductChargePropertiesID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_ShoppingBasketObjects.ShoppingBasketID);
            return View(sD_ShoppingBasketObjects);
        }

        // POST: ShoppingBasketObjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShoppingBasketID,ProductChargePropertiesID,Price,OldPrice,Count,AddDate")] SD_ShoppingBasketObjects sD_ShoppingBasketObjects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ShoppingBasketObjects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_ShoppingBasketObjects.ProductChargePropertiesID);
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
            //SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.FirstOrDefault(ps => ps.ID == id);

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
            //SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.Find(id);
            SD_ShoppingBasketObjects sD_ShoppingBasketObjects = db.SD_ShoppingBasketObjects.FirstOrDefault(ps => ps.ID == id);


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
