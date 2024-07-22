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
    public class ProductController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Product
        public ActionResult Index()
        {
            return View(db.SD_Product.ToList());
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Product sD_Product = db.SD_Product.Find(id);
            if (sD_Product == null)
            {
                return HttpNotFound();
            }
            return View(sD_Product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Code")] SD_Product sD_Product)
        {
            if (ModelState.IsValid)
            {
                db.SD_Product.Add(sD_Product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sD_Product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Product sD_Product = db.SD_Product.Find(id);
            if (sD_Product == null)
            {
                return HttpNotFound();
            }
            return View(sD_Product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Code")] SD_Product sD_Product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sD_Product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Product sD_Product = db.SD_Product.Find(id);
            if (sD_Product == null)
            {
                return HttpNotFound();
            }
            return View(sD_Product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Product sD_Product = db.SD_Product.Find(id);
            db.SD_Product.Remove(sD_Product);
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
