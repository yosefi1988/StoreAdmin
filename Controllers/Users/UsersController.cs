using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers.Users
{
    public class UsersController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.SD_Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Users sD_Users = db.SD_Users.Find(id);
            if (sD_Users == null)
            {
                return HttpNotFound();
            }
            return View(sD_Users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Family,Mobile,Password,Email,Description")] SD_Users sD_Users)
        {
            if (ModelState.IsValid)
            {
                db.SD_Users.Add(sD_Users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sD_Users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Users sD_Users = db.SD_Users.Find(id);
            if (sD_Users == null)
            {
                return HttpNotFound();
            }
            return View(sD_Users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Family,Mobile,Password,Email,Description")] SD_Users sD_Users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sD_Users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Users sD_Users = db.SD_Users.Find(id);
            if (sD_Users == null)
            {
                return HttpNotFound();
            }
            return View(sD_Users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Users sD_Users = db.SD_Users.Find(id);
            db.SD_Users.Remove(sD_Users);
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
