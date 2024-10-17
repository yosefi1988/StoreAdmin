using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers
{
    public class StoreDetailsController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();


        // GET: StoreDetails/Details/5
        public ActionResult Details()
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            SC_StoreDetails sC_StoreDetails = db.SC_StoreDetails.OrderByDescending(x => x.ID).ToList().FirstOrDefault();
            if (sC_StoreDetails == null)
            {
                return HttpNotFound();
            }
            return View(sC_StoreDetails);
        }

        // GET: StoreDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoreDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StoreName,Logo,AboutStore,ReturnConditions,Instagram,Telegram,twitter,Facebook,Pinterest,Address,Phone,Fax,SupportNo,SupportHours")] SC_StoreDetails sC_StoreDetails)
        {
            if (ModelState.IsValid)
            {
                db.SC_StoreDetails.Add(sC_StoreDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sC_StoreDetails);
        }

        // GET: StoreDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SC_StoreDetails sC_StoreDetails = db.SC_StoreDetails.Find(id);
            if (sC_StoreDetails == null)
            {
                return HttpNotFound();
            }
            return View(sC_StoreDetails);
        }

        // POST: StoreDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StoreName,Logo,AboutStore,ReturnConditions,Instagram,Telegram,twitter,Facebook,Pinterest,Address,Phone,Fax,SupportNo,SupportHours")] SC_StoreDetails sC_StoreDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sC_StoreDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sC_StoreDetails);
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
