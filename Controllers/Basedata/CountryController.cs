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
    public class CountryController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Country
        public ActionResult Index()
        {
            return View(db.BD_Country.ToList());
        }

        // GET: Country/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Country bD_Country = db.BD_Country.Find(id);
            if (bD_Country == null)
            {
                return HttpNotFound();
            }
            return View(bD_Country);
        }

        // GET: Country/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Logo")] BD_Country bD_Country)
        {
            if (ModelState.IsValid)
            {
                db.BD_Country.Add(bD_Country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bD_Country);
        }

        // GET: Country/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Country bD_Country = db.BD_Country.Find(id);
            if (bD_Country == null)
            {
                return HttpNotFound();
            }
            return View(bD_Country);
        }

        // POST: Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Logo")] BD_Country bD_Country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_Country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bD_Country);
        }

        // GET: Country/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Country bD_Country = db.BD_Country.Find(id);
            if (bD_Country == null)
            {
                return HttpNotFound();
            }
            return View(bD_Country);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_Country bD_Country = db.BD_Country.Find(id);
            db.BD_Country.Remove(bD_Country);
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
