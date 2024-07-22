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
    public class CitiesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Cities
        public ActionResult Index()
        {
            var bD_Cities = db.BD_Cities.Include(b => b.BD_States);
            return View(bD_Cities.ToList());
        }

        // GET: Cities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Cities bD_Cities = db.BD_Cities.Find(id);
            if (bD_Cities == null)
            {
                return HttpNotFound();
            }
            return View(bD_Cities);
        }

        // GET: Cities/Create
        public ActionResult Create()
        {
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title");
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StateID,Title")] BD_Cities bD_Cities)
        {
            if (ModelState.IsValid)
            {
                db.BD_Cities.Add(bD_Cities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_Cities.StateID);
            return View(bD_Cities);
        }

        // GET: Cities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Cities bD_Cities = db.BD_Cities.Find(id);
            if (bD_Cities == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_Cities.StateID);
            return View(bD_Cities);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StateID,Title")] BD_Cities bD_Cities)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_Cities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_Cities.StateID);
            return View(bD_Cities);
        }

        // GET: Cities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_Cities bD_Cities = db.BD_Cities.Find(id);
            if (bD_Cities == null)
            {
                return HttpNotFound();
            }
            return View(bD_Cities);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_Cities bD_Cities = db.BD_Cities.Find(id);
            db.BD_Cities.Remove(bD_Cities);
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
