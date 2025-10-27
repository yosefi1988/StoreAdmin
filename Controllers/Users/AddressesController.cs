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
    public class AddressesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Addresses
        public ActionResult Index()
        {
            var sD_Addresses = db.SD_Addresses.Include(s => s.BD_Cities).Include(s => s.SD_Users);
            return View(sD_Addresses.ToList());
        }

        // GET: Addresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Addresses sD_Addresses = db.SD_Addresses.Find(id);
            if (sD_Addresses == null)
            {
                return HttpNotFound();
            }
            return View(sD_Addresses);
        }

        // GET: Addresses/Create
        public ActionResult Create()
        {
            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title");
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name");
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,CityID,Address,IsDefault,FullName,MobileNo")] SD_Addresses sD_Addresses)
        {
            if (ModelState.IsValid)
            {
                db.SD_Addresses.Add(sD_Addresses);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title", sD_Addresses.CityID);
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_Addresses.UserID);
            return View(sD_Addresses);
        }

        // GET: Addresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Addresses sD_Addresses = db.SD_Addresses.Find(id);
            if (sD_Addresses == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title", sD_Addresses.CityID);
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_Addresses.UserID);
            return View(sD_Addresses);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,CityID,Address,IsDefault,FullName,MobileNo")] SD_Addresses sD_Addresses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Addresses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityID = new SelectList(db.BD_Cities, "ID", "Title", sD_Addresses.CityID);
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_Addresses.UserID);
            return View(sD_Addresses);
        }

        // GET: Addresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Addresses sD_Addresses = db.SD_Addresses.Find(id);
            if (sD_Addresses == null)
            {
                return HttpNotFound();
            }
            return View(sD_Addresses);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Addresses sD_Addresses = db.SD_Addresses.Find(id);
            db.SD_Addresses.Remove(sD_Addresses);
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
