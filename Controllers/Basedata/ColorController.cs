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
    public class ColorController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Color
        public ActionResult Index()
        {
            return View(db.SD_Color.ToList());
        }

        // GET: Color/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Color sD_Color = db.SD_Color.Find(id);
            if (sD_Color == null)
            {
                return HttpNotFound();
            }
            return View(sD_Color);
        }

        // GET: Color/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Color/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ColorCode")] SD_Color sD_Color)
        {
            if (ModelState.IsValid)
            {
                db.SD_Color.Add(sD_Color);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sD_Color);
        }

        // GET: Color/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Color sD_Color = db.SD_Color.Find(id);
            if (sD_Color == null)
            {
                return HttpNotFound();
            }
            return View(sD_Color);
        }

        // POST: Color/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ColorCode")] SD_Color sD_Color)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Color).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sD_Color);
        }

        // GET: Color/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Color sD_Color = db.SD_Color.Find(id);
            if (sD_Color == null)
            {
                return HttpNotFound();
            }
            return View(sD_Color);
        }

        // POST: Color/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Color sD_Color = db.SD_Color.Find(id);
            db.SD_Color.Remove(sD_Color);
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
