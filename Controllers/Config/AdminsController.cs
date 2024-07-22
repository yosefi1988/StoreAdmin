using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers.Config
{
    public class AdminsController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Admins
        public ActionResult Index()
        {
            var sC_Admins = db.SC_Admins.Include(s => s.SD_Users);
            return View(sC_Admins.ToList());
        }

        // GET: Admins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SC_Admins sC_Admins = db.SC_Admins.Find(id);
            if (sC_Admins == null)
            {
                return HttpNotFound();
            }
            return View(sC_Admins);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name");
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,IsLocked,Description")] SC_Admins sC_Admins)
        {
            if (ModelState.IsValid)
            {
                db.SC_Admins.Add(sC_Admins);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sC_Admins.UserID);
            return View(sC_Admins);
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SC_Admins sC_Admins = db.SC_Admins.Find(id);
            if (sC_Admins == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sC_Admins.UserID);
            return View(sC_Admins);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,IsLocked,Description")] SC_Admins sC_Admins)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sC_Admins).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sC_Admins.UserID);
            return View(sC_Admins);
        }

        // GET: Admins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SC_Admins sC_Admins = db.SC_Admins.Find(id);
            if (sC_Admins == null)
            {
                return HttpNotFound();
            }
            return View(sC_Admins);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SC_Admins sC_Admins = db.SC_Admins.Find(id);
            db.SC_Admins.Remove(sC_Admins);
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
