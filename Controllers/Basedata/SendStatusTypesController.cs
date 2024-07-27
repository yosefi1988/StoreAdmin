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
    public class SendStatusTypesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: SendStatusTypes
        public ActionResult Index()
        {
            return View(db.BD_SendStatusTypes.ToList());
        }

        // GET: SendStatusTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendStatusTypes bD_SendStatusTypes = db.BD_SendStatusTypes.Find(id);
            if (bD_SendStatusTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendStatusTypes);
        }

        // GET: SendStatusTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SendStatusTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Status,Description")] BD_SendStatusTypes bD_SendStatusTypes)
        {
            if (ModelState.IsValid)
            {
                db.BD_SendStatusTypes.Add(bD_SendStatusTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bD_SendStatusTypes);
        }

        // GET: SendStatusTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendStatusTypes bD_SendStatusTypes = db.BD_SendStatusTypes.Find(id);
            if (bD_SendStatusTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendStatusTypes);
        }

        // POST: SendStatusTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Status,Description")] BD_SendStatusTypes bD_SendStatusTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_SendStatusTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bD_SendStatusTypes);
        }

        // GET: SendStatusTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendStatusTypes bD_SendStatusTypes = db.BD_SendStatusTypes.Find(id);
            if (bD_SendStatusTypes == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendStatusTypes);
        }

        // POST: SendStatusTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_SendStatusTypes bD_SendStatusTypes = db.BD_SendStatusTypes.Find(id);
            db.BD_SendStatusTypes.Remove(bD_SendStatusTypes);
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
