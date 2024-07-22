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
    public class ShoppingBasketController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ShoppingBasket
        public ActionResult Index()
        {
            var sD_ShoppingBasket = db.SD_ShoppingBasket.Include(s => s.SD_Users);
            return View(sD_ShoppingBasket.ToList());
        }

        // GET: ShoppingBasket/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ShoppingBasket sD_ShoppingBasket = db.SD_ShoppingBasket.Find(id);
            if (sD_ShoppingBasket == null)
            {
                return HttpNotFound();
            }
            return View(sD_ShoppingBasket);
        }

        // GET: ShoppingBasket/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name");
            return View();
        }

        // POST: ShoppingBasket/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,Status,CreatedOn")] SD_ShoppingBasket sD_ShoppingBasket)
        {
            if (ModelState.IsValid)
            {
                db.SD_ShoppingBasket.Add(sD_ShoppingBasket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_ShoppingBasket.UserID);
            return View(sD_ShoppingBasket);
        }

        // GET: ShoppingBasket/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ShoppingBasket sD_ShoppingBasket = db.SD_ShoppingBasket.Find(id);
            if (sD_ShoppingBasket == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_ShoppingBasket.UserID);
            return View(sD_ShoppingBasket);
        }

        // POST: ShoppingBasket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,Status,CreatedOn")] SD_ShoppingBasket sD_ShoppingBasket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ShoppingBasket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_ShoppingBasket.UserID);
            return View(sD_ShoppingBasket);
        }

        // GET: ShoppingBasket/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ShoppingBasket sD_ShoppingBasket = db.SD_ShoppingBasket.Find(id);
            if (sD_ShoppingBasket == null)
            {
                return HttpNotFound();
            }
            return View(sD_ShoppingBasket);
        }

        // POST: ShoppingBasket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_ShoppingBasket sD_ShoppingBasket = db.SD_ShoppingBasket.Find(id);
            db.SD_ShoppingBasket.Remove(sD_ShoppingBasket);
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
