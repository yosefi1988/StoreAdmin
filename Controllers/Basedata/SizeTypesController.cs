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
    public class SizeTypesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: SizeTypes
        public ActionResult Index()
        {
            return View(db.BD_SizeTypes.ToList());
        }

        // GET: SizeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SizeTypes bD_SizeTypes = db.BD_SizeTypes.Find(id);
            if (bD_SizeTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_SizeTypes);
        }

        // GET: SizeTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SizeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description")] BD_SizeTypes bD_SizeTypes)
        {
            if (ModelState.IsValid)
            {
                db.BD_SizeTypes.Add(bD_SizeTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bD_SizeTypes);
        }

        // GET: SizeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SizeTypes bD_SizeTypes = db.BD_SizeTypes.Find(id);
            if (bD_SizeTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_SizeTypes);
        }

        // POST: SizeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description")] BD_SizeTypes bD_SizeTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_SizeTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bD_SizeTypes);
        }

        // GET: SizeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SizeTypes bD_SizeTypes = db.BD_SizeTypes.Find(id);
            if (bD_SizeTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_SizeTypes);
        }

        // POST: SizeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_SizeTypes bD_SizeTypes = db.BD_SizeTypes.Find(id);
            db.BD_SizeTypes.Remove(bD_SizeTypes);
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
