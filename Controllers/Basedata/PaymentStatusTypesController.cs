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
    public class PaymentStatusTypesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: PaymentStatusTypes
        public ActionResult Index()
        {
            return View(db.BD_PaymentStatusTypes.ToList());
        }

        // GET: PaymentStatusTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_PaymentStatusTypes bD_PaymentStatusTypes = db.BD_PaymentStatusTypes.Find(id);
            if (bD_PaymentStatusTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_PaymentStatusTypes);
        }

        // GET: PaymentStatusTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymentStatusTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Status,Description")] BD_PaymentStatusTypes bD_PaymentStatusTypes)
        {
            if (ModelState.IsValid)
            {
                db.BD_PaymentStatusTypes.Add(bD_PaymentStatusTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bD_PaymentStatusTypes);
        }

        // GET: PaymentStatusTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_PaymentStatusTypes bD_PaymentStatusTypes = db.BD_PaymentStatusTypes.Find(id);
            if (bD_PaymentStatusTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_PaymentStatusTypes);
        }

        // POST: PaymentStatusTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Status,Description")] BD_PaymentStatusTypes bD_PaymentStatusTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_PaymentStatusTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bD_PaymentStatusTypes);
        }

        // GET: PaymentStatusTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_PaymentStatusTypes bD_PaymentStatusTypes = db.BD_PaymentStatusTypes.Find(id);
            if (bD_PaymentStatusTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_PaymentStatusTypes);
        }

        // POST: PaymentStatusTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_PaymentStatusTypes bD_PaymentStatusTypes = db.BD_PaymentStatusTypes.Find(id);
            db.BD_PaymentStatusTypes.Remove(bD_PaymentStatusTypes);
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
