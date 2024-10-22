﻿using System;
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
    public class ProductChargesPropertiesController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: ProductChargesProperties
        public ActionResult Index()
        {
            var sD_ProductChargesProperties = db.SD_ProductChargesProperties
                .Include(s => s.SD_Color)
                .Include(s => s.SD_ProductCharges);
            return View(sD_ProductChargesProperties.ToList());
        }

        // GET: ProductChargesProperties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ProductChargesProperties sD_ProductChargesProperties = db.SD_ProductChargesProperties.Find(id);
            if (sD_ProductChargesProperties == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductChargesProperties);
        }

        // GET: ProductChargesProperties/Create
        public ActionResult Create()
        {
            ViewBag.ColorID = new SelectList(db.SD_Color, "ID", "Title");

            //v1
            //ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber");
            //v2
            //ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "IDDDL");
            
            //v3
            var productChargesForDDL = db.View_Admin_ProductChargeID.Select(p => new
                                                                                {
                                                                                    ProductChargeID = p.ProductChargeID,
                                                                                    CombinedText = p.Product + " (code:" + p.ProductCode + ") Invoice Number:" + p.BuyInvoiceNumber + " (buy count:" + p.BuyCount +")"
                                                                        }).OrderByDescending(x => x.ProductChargeID)
                                                                        .ToList();
            ViewBag.ProductChargeDDL = new SelectList(productChargesForDDL, "ProductChargeID", "CombinedText");
            // end v3

            return View();
        }

        // POST: ProductChargesProperties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ID,ProductChargeID,ColorID,Discount,RemainingCount")]
            SD_ProductChargesProperties sD_ProductChargesProperties)
        {
            if (ModelState.IsValid)
            {
                db.SD_ProductChargesProperties.Add(sD_ProductChargesProperties);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColorID = new SelectList(db.SD_Color, "ID", "Title", sD_ProductChargesProperties.ColorID);
            //ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber", sD_ProductChargesProperties.ProductChargeID);
            //xxx2
            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "IDDDL", sD_ProductChargesProperties.ProductChargeID);
            return View(sD_ProductChargesProperties);
        }

        // GET: ProductChargesProperties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ProductChargesProperties sD_ProductChargesProperties = db.SD_ProductChargesProperties.Find(id);
            if (sD_ProductChargesProperties == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColorID = new SelectList(db.SD_Color, "ID", "Title", sD_ProductChargesProperties.ColorID);
            //ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber", sD_ProductChargesProperties.ProductChargeID);
            //xxx3
            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "IDDDL", sD_ProductChargesProperties.ProductChargeID);
            return View(sD_ProductChargesProperties);
        }

        // POST: ProductChargesProperties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductChargeID,ColorID,Discount,RemainingCount")] SD_ProductChargesProperties sD_ProductChargesProperties)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_ProductChargesProperties).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColorID = new SelectList(db.SD_Color, "ID", "Title", sD_ProductChargesProperties.ColorID);
            //ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "BuyInvoiceNumber", sD_ProductChargesProperties.ProductChargeID);
            //xxx4
            ViewBag.ProductChargeID = new SelectList(db.SD_ProductCharges, "ID", "IDDDL", sD_ProductChargesProperties.ProductChargeID);
            return View(sD_ProductChargesProperties);
        }

        // GET: ProductChargesProperties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_ProductChargesProperties sD_ProductChargesProperties = db.SD_ProductChargesProperties.Find(id);
            if (sD_ProductChargesProperties == null)
            {
                return HttpNotFound();
            }
            return View(sD_ProductChargesProperties);
        }

        // POST: ProductChargesProperties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_ProductChargesProperties sD_ProductChargesProperties = db.SD_ProductChargesProperties.Find(id);
            db.SD_ProductChargesProperties.Remove(sD_ProductChargesProperties);
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




        ////Redirect
        //public ActionResult Pictures()
        //{
        //    return RedirectToAction("Index" ,"ProductSizes" ,"12");
        //}
        //public ActionResult Sizes()
        //{
        //    return RedirectToAction("Index");
        //}
        //public ActionResult Votes()
        //{
        //    return RedirectToAction("Index");
        //}

    }
}
