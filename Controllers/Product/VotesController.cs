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
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                var sD_Votes = db.SD_Votes.Include(s => s.SD_ProductChargesProperties).Include(s => s.SD_Users);
                return View(sD_Votes.ToList());
            }
            var sD_Votes_id = db.SD_Votes
                                   .Include(s => s.SD_ProductChargesProperties)
                                   .Where(s => s.ProductChargePropertiesID == id);

            if (sD_Votes_id == null)
            {
                return HttpNotFound();
            }
            return View(sD_Votes_id.ToList());
        }

        // GET: Votes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_Votes sD_Votes = db.SD_Votes.Find(id);
            SD_Votes sD_Votes = db.SD_Votes.FirstOrDefault(ps => ps.ID == id);

            if (sD_Votes == null)
            {
                return HttpNotFound();
            }
            return View(sD_Votes);
        }

        // GET: Votes/Create
        public ActionResult Create()
        {
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID");

            
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name");


            var productChargePropertiesForDDL = db.View_Admin_ProductChargePropertiesID.Select(p => new
                                                                    {
                                                                        ProductChargePropertiesID = p.ProductChargePropertiesID,
                                                                        CombinedText = p.ProductName + " (code:" + p.ProductCode + ") Invoice Number:" + p.BuyInvoiceNumber + " Buy Price:" + p.BuyPrice + " - color:" +p.Color
                                                                    }).OrderByDescending(x => x.ProductChargePropertiesID)
                                                                    .ToList();
            ViewBag.ProductChargePropertiesDDL = new SelectList(productChargePropertiesForDDL, "ProductChargePropertiesID", "CombinedText");


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

            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_Votes.ProductChargePropertiesID);
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_Votes.UserID);
            return View(sD_Votes);
        }

        // GET: Votes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_Votes sD_Votes = db.SD_Votes.Find(id);
            SD_Votes sD_Votes = db.SD_Votes.FirstOrDefault(ps => ps.ID == id);

            if (sD_Votes == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_Votes.ProductChargePropertiesID);
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_Votes.UserID);
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
            ViewBag.ProductChargePropertiesID = new SelectList(db.SD_ProductChargesProperties, "ID", "ID", sD_Votes.ProductChargePropertiesID);
            ViewBag.UserID = new SelectList(db.SD_Users, "ID", "Name", sD_Votes.UserID);
            return View(sD_Votes);
        }

        // GET: Votes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SD_Votes sD_Votes = db.SD_Votes.Find(id);
            SD_Votes sD_Votes = db.SD_Votes.FirstOrDefault(ps => ps.ID == id);

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
            //SD_Votes sD_Votes = db.SD_Votes.Find(id);
            SD_Votes sD_Votes = db.SD_Votes.FirstOrDefault(ps => ps.ID == id);

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
