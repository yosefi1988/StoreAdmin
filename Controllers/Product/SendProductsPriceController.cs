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
    public class SendProductsPriceController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: SendProductsPrice
        public ActionResult Index()
        {
            var bD_SendProductsPrice = db.BD_SendProductsPrice.Include(b => b.BD_States).Include(b => b.SD_Product);
            return View(bD_SendProductsPrice.ToList());
        }

        // GET: SendProductsPrice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendProductsPrice bD_SendProductsPrice = db.BD_SendProductsPrice.Find(id);
            if (bD_SendProductsPrice == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendProductsPrice);
        }

        // GET: SendProductsPrice/Create
        public ActionResult Create()
        {
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title");
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name");
            return View();
        }

        // POST: SendProductsPrice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Price,StateID,ProductID,Title")] BD_SendProductsPrice bD_SendProductsPrice)
        {
            if (ModelState.IsValid)
            {
                db.BD_SendProductsPrice.Add(bD_SendProductsPrice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_SendProductsPrice.StateID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", bD_SendProductsPrice.ProductID);
            return View(bD_SendProductsPrice);
        }

        // GET: SendProductsPrice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendProductsPrice bD_SendProductsPrice = db.BD_SendProductsPrice.Find(id);
            if (bD_SendProductsPrice == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_SendProductsPrice.StateID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", bD_SendProductsPrice.ProductID);
            return View(bD_SendProductsPrice);
        }

        // POST: SendProductsPrice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Price,StateID,ProductID,Title")] BD_SendProductsPrice bD_SendProductsPrice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_SendProductsPrice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_SendProductsPrice.StateID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", bD_SendProductsPrice.ProductID);
            return View(bD_SendProductsPrice);
        }

        // GET: SendProductsPrice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendProductsPrice bD_SendProductsPrice = db.BD_SendProductsPrice.Find(id);
            if (bD_SendProductsPrice == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendProductsPrice);
        }

        // POST: SendProductsPrice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_SendProductsPrice bD_SendProductsPrice = db.BD_SendProductsPrice.Find(id);
            db.BD_SendProductsPrice.Remove(bD_SendProductsPrice);
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
