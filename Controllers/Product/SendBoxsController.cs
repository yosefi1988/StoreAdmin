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
    public class SendBoxsController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: SendBoxs
        public ActionResult Index()
        {
            var sD_SendBoxs = db.SD_SendBoxs.Include(s => s.BD_Cities).Include(s => s.BD_SendStatusTypes).Include(s => s.BD_SendBoxPrices).Include(s => s.SD_ShoppingBasket).Include(s => s.SD_Transactions);
            return View(sD_SendBoxs.ToList());
        }

        // GET: SendBoxs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_SendBoxs sD_SendBoxs = db.SD_SendBoxs.Find(id);
            if (sD_SendBoxs == null)
            {
                return HttpNotFound();
            }
            return View(sD_SendBoxs);
        }

        // GET: SendBoxs/Create
        public ActionResult Create()
        {
            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title");
            ViewBag.SendStatusID = new SelectList(db.BD_SendStatusTypes, "ID", "Status");
            ViewBag.SendPriceID = new SelectList(db.BD_SendBoxPrices, "ID", "Title");
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID");
            ViewBag.TransactionID = new SelectList(db.SD_Transactions, "ID", "PaymentTrackingNo");
            return View();
        }

        // POST: SendBoxs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ShoppingBasketID,TransactionID,SendPriceID,SendTypeTitle,CityID,Address,FullName,MobileNo,SendDate,SendStatusID,SendTrackingNo")] SD_SendBoxs sD_SendBoxs)
        {
            if (ModelState.IsValid)
            {
                db.SD_SendBoxs.Add(sD_SendBoxs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title", sD_SendBoxs.CityID);
            ViewBag.SendStatusID = new SelectList(db.BD_SendStatusTypes, "ID", "Status", sD_SendBoxs.SendStatusID);
            ViewBag.SendPriceID = new SelectList(db.BD_SendBoxPrices, "ID", "Title", sD_SendBoxs.SendPriceID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_SendBoxs.ShoppingBasketID);
            ViewBag.TransactionID = new SelectList(db.SD_Transactions, "ID", "PaymentTrackingNo", sD_SendBoxs.TransactionID);
            return View(sD_SendBoxs);
        }

        // GET: SendBoxs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_SendBoxs sD_SendBoxs = db.SD_SendBoxs.Find(id);
            if (sD_SendBoxs == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title", sD_SendBoxs.CityID);
            ViewBag.SendStatusID = new SelectList(db.BD_SendStatusTypes, "ID", "Status", sD_SendBoxs.SendStatusID);
            ViewBag.SendPriceID = new SelectList(db.BD_SendBoxPrices, "ID", "Title", sD_SendBoxs.SendPriceID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_SendBoxs.ShoppingBasketID);
            ViewBag.TransactionID = new SelectList(db.SD_Transactions, "ID", "PaymentTrackingNo", sD_SendBoxs.TransactionID);
            return View(sD_SendBoxs);
        }

        // POST: SendBoxs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShoppingBasketID,TransactionID,SendPriceID,SendTypeTitle,CityID,Address,FullName,MobileNo,SendDate,SendStatusID,SendTrackingNo")] SD_SendBoxs sD_SendBoxs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_SendBoxs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title", sD_SendBoxs.CityID);
            ViewBag.SendStatusID = new SelectList(db.BD_SendStatusTypes, "ID", "Status", sD_SendBoxs.SendStatusID);
            ViewBag.SendPriceID = new SelectList(db.BD_SendBoxPrices, "ID", "Title", sD_SendBoxs.SendPriceID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_SendBoxs.ShoppingBasketID);
            ViewBag.TransactionID = new SelectList(db.SD_Transactions, "ID", "PaymentTrackingNo", sD_SendBoxs.TransactionID);
            return View(sD_SendBoxs);
        }

        // GET: SendBoxs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_SendBoxs sD_SendBoxs = db.SD_SendBoxs.Find(id);
            if (sD_SendBoxs == null)
            {
                return HttpNotFound();
            }
            return View(sD_SendBoxs);
        }

        // POST: SendBoxs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_SendBoxs sD_SendBoxs = db.SD_SendBoxs.Find(id);
            db.SD_SendBoxs.Remove(sD_SendBoxs);
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
