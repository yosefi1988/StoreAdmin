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
    public class SendBoxPricesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: SendBoxPrices
        public ActionResult Index()
        {
            var bD_SendBoxPrices = db.BD_SendBoxPrices.Include(b => b.BD_States);
            return View(bD_SendBoxPrices.ToList());
        }

        // GET: SendBoxPrices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendBoxPrices bD_SendBoxPrices = db.BD_SendBoxPrices.Find(id);
            if (bD_SendBoxPrices == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendBoxPrices);
        }

        // GET: SendBoxPrices/Create
        public ActionResult Create()
        {
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title");
            return View();
        }

        // POST: SendBoxPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Price,StateID,Title")] BD_SendBoxPrices bD_SendBoxPrices)
        {
            if (ModelState.IsValid)
            {
                db.BD_SendBoxPrices.Add(bD_SendBoxPrices);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_SendBoxPrices.StateID);
            return View(bD_SendBoxPrices);
        }

        // GET: SendBoxPrices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendBoxPrices bD_SendBoxPrices = db.BD_SendBoxPrices.Find(id);
            if (bD_SendBoxPrices == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_SendBoxPrices.StateID);
            return View(bD_SendBoxPrices);
        }

        // POST: SendBoxPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Price,StateID,Title")] BD_SendBoxPrices bD_SendBoxPrices)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bD_SendBoxPrices).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateID = new SelectList(db.BD_States, "ID", "Title", bD_SendBoxPrices.StateID);
            return View(bD_SendBoxPrices);
        }

        // GET: SendBoxPrices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BD_SendBoxPrices bD_SendBoxPrices = db.BD_SendBoxPrices.Find(id);
            if (bD_SendBoxPrices == null)
            {
                return HttpNotFound();
            }
            return View(bD_SendBoxPrices);
        }

        // POST: SendBoxPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BD_SendBoxPrices bD_SendBoxPrices = db.BD_SendBoxPrices.Find(id);
            db.BD_SendBoxPrices.Remove(bD_SendBoxPrices);
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
