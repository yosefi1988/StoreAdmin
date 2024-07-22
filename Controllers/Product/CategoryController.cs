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
    public class CategoryController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Category
        public ActionResult Index()
        {
            return View(db.SD_Category.ToList());
        }

        // GET: Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Category sD_Category = db.SD_Category.Find(id);
            if (sD_Category == null)
            {
                return HttpNotFound();
            }
            return View(sD_Category);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Code")] SD_Category sD_Category)
        {
            if (ModelState.IsValid)
            {
                db.SD_Category.Add(sD_Category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sD_Category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Category sD_Category = db.SD_Category.Find(id);
            if (sD_Category == null)
            {
                return HttpNotFound();
            }
            return View(sD_Category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Code")] SD_Category sD_Category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sD_Category);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Category sD_Category = db.SD_Category.Find(id);
            if (sD_Category == null)
            {
                return HttpNotFound();
            }
            return View(sD_Category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Category sD_Category = db.SD_Category.Find(id);
            db.SD_Category.Remove(sD_Category);
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
