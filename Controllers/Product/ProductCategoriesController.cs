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
    public class ProductCategoriesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ProductCategories
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                var sD_ProductCategories = db.SD_ProductCategories
                                            .Include(s => s.SD_Category)
                                            .Include(s => s.SD_Product);
                return View(sD_ProductCategories.ToList());
            } 

            var sD_ProductCategories_id = db.SD_ProductCategories
                                   .Include(s => s.SD_Product)
                                   .Where(s => s.ProductID == id);


            if (sD_ProductCategories_id == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductCategories_id.ToList());


        }

        // GET: ProductCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.Find(id);
            SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.FirstOrDefault(ps => ps.ID == id); 

            if (sD_ProductCategories == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductCategories);
        }

        // GET: ProductCategories/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.SD_Category, "ID", "Title");
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name");
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProductID,CategoryID,Description")] SD_ProductCategories sD_ProductCategories)
        {
            if (ModelState.IsValid)
            {
                db.SD_ProductCategories.Add(sD_ProductCategories);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.SD_Category, "ID", "Title", sD_ProductCategories.CategoryID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", sD_ProductCategories.ProductID);
            return View(sD_ProductCategories);
        }

        // GET: ProductCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.Find(id);
            SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.FirstOrDefault(ps => ps.ID == id);

            if (sD_ProductCategories == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.SD_Category, "ID", "Title", sD_ProductCategories.CategoryID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", sD_ProductCategories.ProductID);
            return View(sD_ProductCategories);
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductID,CategoryID,Description")] SD_ProductCategories sD_ProductCategories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ProductCategories).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.SD_Category, "ID", "Title", sD_ProductCategories.CategoryID);
            ViewBag.ProductID = new SelectList(db.SD_Product, "ID", "Name", sD_ProductCategories.ProductID);
            return View(sD_ProductCategories);
        }

        // GET: ProductCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.Find(id);
            SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.FirstOrDefault(ps => ps.ID == id);

            if (sD_ProductCategories == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductCategories);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.Find(id);
            SD_ProductCategories sD_ProductCategories = db.SD_ProductCategories.FirstOrDefault(ps => ps.ID == id);

            db.SD_ProductCategories.Remove(sD_ProductCategories);
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
