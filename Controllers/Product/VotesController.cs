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
    public class VotesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Votes
        public ActionResult Index()
        {
            return View(db.SD_Votes.ToList());
        }

        // GET: Votes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Votes sD_Votes = db.SD_Votes.Find(id);
            if (sD_Votes == null)
            {
                return HttpNotFound();
            }
            return View(sD_Votes);
        }

        // GET: Votes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Votes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,ProductChargePropertiesID,Star,Comment")] SD_Votes sD_Votes)
        {
            if (ModelState.IsValid)
            {
                db.SD_Votes.Add(sD_Votes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sD_Votes);
        }

        // GET: Votes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Votes sD_Votes = db.SD_Votes.Find(id);
            if (sD_Votes == null)
            {
                return HttpNotFound();
            }
            return View(sD_Votes);
        }

        // POST: Votes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,ProductChargePropertiesID,Star,Comment")] SD_Votes sD_Votes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Votes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sD_Votes);
        }

        // GET: Votes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Votes sD_Votes = db.SD_Votes.Find(id);
            if (sD_Votes == null)
            {
                return HttpNotFound();
            }
            return View(sD_Votes);
        }

        // POST: Votes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Votes sD_Votes = db.SD_Votes.Find(id);
            db.SD_Votes.Remove(sD_Votes);
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
