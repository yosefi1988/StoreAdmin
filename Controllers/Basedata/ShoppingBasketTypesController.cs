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
    public class ShoppingBasketTypesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ShoppingBasketTypes
        public ActionResult Index()
        {
            return View(db.BD_ShoppingBasketTypes.ToList());
        }

        // GET: ShoppingBasketTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_ShoppingBasketTypes bD_ShoppingBasketTypes = db.BD_ShoppingBasketTypes.Find(id);
            if (bD_ShoppingBasketTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_ShoppingBasketTypes);
        }

        // GET: ShoppingBasketTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShoppingBasketTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Status,Description")] BD_ShoppingBasketTypes bD_ShoppingBasketTypes)
        {
            if (ModelState.IsValid)
            {
                db.BD_ShoppingBasketTypes.Add(bD_ShoppingBasketTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bD_ShoppingBasketTypes);
        }

        // GET: ShoppingBasketTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_ShoppingBasketTypes bD_ShoppingBasketTypes = db.BD_ShoppingBasketTypes.Find(id);
            if (bD_ShoppingBasketTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_ShoppingBasketTypes);
        }

        // POST: ShoppingBasketTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Status,Description")] BD_ShoppingBasketTypes bD_ShoppingBasketTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_ShoppingBasketTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bD_ShoppingBasketTypes);
        }

        // GET: ShoppingBasketTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_ShoppingBasketTypes bD_ShoppingBasketTypes = db.BD_ShoppingBasketTypes.Find(id);
            if (bD_ShoppingBasketTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_ShoppingBasketTypes);
        }

        // POST: ShoppingBasketTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_ShoppingBasketTypes bD_ShoppingBasketTypes = db.BD_ShoppingBasketTypes.Find(id);
            db.BD_ShoppingBasketTypes.Remove(bD_ShoppingBasketTypes);
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
