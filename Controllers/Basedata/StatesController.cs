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
    public class StatesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: States
        public ActionResult Index()
        {
            var bD_States = db.BD_States.Include(b => b.BD_Country);
            return View(bD_States.ToList());
        }

        // GET: States/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_States bD_States = db.BD_States.Find(id);
            if (bD_States == null)
            {
                return HttpNotFound();
            }
            return View(bD_States);
        }

        // GET: States/Create
        public ActionResult Create()
        {
            ViewBag.CountryID = new SelectList(db.BD_Country, "ID", "Title");
            return View();
        }

        // POST: States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CountryID,Title")] BD_States bD_States)
        {
            if (ModelState.IsValid)
            {
                db.BD_States.Add(bD_States);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CountryID = new SelectList(db.BD_Country, "ID", "Title", bD_States.CountryID);
            return View(bD_States);
        }

        // GET: States/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_States bD_States = db.BD_States.Find(id);
            if (bD_States == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(db.BD_Country, "ID", "Title", bD_States.CountryID);
            return View(bD_States);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CountryID,Title")] BD_States bD_States)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_States).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryID = new SelectList(db.BD_Country, "ID", "Title", bD_States.CountryID);
            return View(bD_States);
        }

        // GET: States/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_States bD_States = db.BD_States.Find(id);
            if (bD_States == null)
            {
                return HttpNotFound();
            }
            return View(bD_States);
        }

        // POST: States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_States bD_States = db.BD_States.Find(id);
            db.BD_States.Remove(bD_States);
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
