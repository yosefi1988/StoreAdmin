using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers.Basedata
{
    public class TaxController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Tax
        public ActionResult Index()
        {
            return View(db.BD_Tax.ToList());
        }

        // GET: Tax/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Tax bD_Tax = db.BD_Tax.Find(id);
            if (bD_Tax == null)
            {
                return HttpNotFound();
            }
            return View(bD_Tax);
        }

        // GET: Tax/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tax/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,TaxPercentage")] BD_Tax bD_Tax)
        {
            if (ModelState.IsValid)
            {
                db.BD_Tax.Add(bD_Tax);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bD_Tax);
        }

        // GET: Tax/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Tax bD_Tax = db.BD_Tax.Find(id);
            if (bD_Tax == null)
            {
                return HttpNotFound();
            }
            return View(bD_Tax);
        }

        // POST: Tax/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,TaxPercentage")] BD_Tax bD_Tax)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_Tax).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bD_Tax);
        }

        // GET: Tax/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Tax bD_Tax = db.BD_Tax.Find(id);
            if (bD_Tax == null)
            {
                return HttpNotFound();
            }
            return View(bD_Tax);
        }

        // POST: Tax/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_Tax bD_Tax = db.BD_Tax.Find(id);
            db.BD_Tax.Remove(bD_Tax);
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
