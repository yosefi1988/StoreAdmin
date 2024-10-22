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
    public class TransactionsController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        // GET: Transactions
        public ActionResult Index()
        {
            var sD_Transactions = db.SD_Transactions.Include(s => s.BD_PaymentStatusTypes).Include(s => s.SD_Coupons).Include(s => s.SD_ShoppingBasket);
            return View(sD_Transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Transactions sD_Transactions = db.SD_Transactions.Find(id);
            if (sD_Transactions == null)
            {
                return HttpNotFound();
            }
            return View(sD_Transactions);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.PaymentStatusID = new SelectList(db.BD_PaymentStatusTypes, "ID", "Status");
            ViewBag.DiscountCodeID = new SelectList(db.SD_Coupons, "ID", "Code");


            //ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID");

            var shoppingBasketForDDL = db.View_Admin_ShoppingBasketID.Select(p => new
            {
                ProductChargePropertiesID = p.ShoppingBasketID,
                CombinedText = p.Name + " " + p.Family + "  (type:" + p.BasketType + ") mobile:" + p.Mobile + " Email:" + p.Email
            }).OrderByDescending(x => x.ProductChargePropertiesID)
                                                        .ToList();
            ViewBag.ShoppingBasketForDDL = new SelectList(shoppingBasketForDDL, "ProductChargePropertiesID", "CombinedText");



            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ShoppingBasketID,SumTaxAmount,DiscountCodeID,DiscountAmount,SendAmount,SumShoppingBasketPrice,SumShoppingBasketDiscount,AmountForPay,PaymentStatusID,PaymentTrackingNo,PaymentDate")] SD_Transactions sD_Transactions)
        {
            if (ModelState.IsValid)
            {
                db.SD_Transactions.Add(sD_Transactions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PaymentStatusID = new SelectList(db.BD_PaymentStatusTypes, "ID", "Status", sD_Transactions.PaymentStatusID);
            ViewBag.DiscountCodeID = new SelectList(db.SD_Coupons, "ID", "Code", sD_Transactions.DiscountCodeID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_Transactions.ShoppingBasketID);
            return View(sD_Transactions);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Transactions sD_Transactions = db.SD_Transactions.Find(id);
            if (sD_Transactions == null)
            {
                return HttpNotFound();
            }
            ViewBag.PaymentStatusID = new SelectList(db.BD_PaymentStatusTypes, "ID", "Status", sD_Transactions.PaymentStatusID);
            ViewBag.DiscountCodeID = new SelectList(db.SD_Coupons, "ID", "Code", sD_Transactions.DiscountCodeID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_Transactions.ShoppingBasketID);
            return View(sD_Transactions);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShoppingBasketID,SumTaxAmount,DiscountCodeID,DiscountAmount,SendAmount,SumShoppingBasketPrice,SumShoppingBasketDiscount,AmountForPay,PaymentStatusID,PaymentTrackingNo,PaymentDate")] SD_Transactions sD_Transactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sD_Transactions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PaymentStatusID = new SelectList(db.BD_PaymentStatusTypes, "ID", "Status", sD_Transactions.PaymentStatusID);
            ViewBag.DiscountCodeID = new SelectList(db.SD_Coupons, "ID", "Code", sD_Transactions.DiscountCodeID);
            ViewBag.ShoppingBasketID = new SelectList(db.SD_ShoppingBasket, "ID", "ID", sD_Transactions.ShoppingBasketID);
            return View(sD_Transactions);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SD_Transactions sD_Transactions = db.SD_Transactions.Find(id);
            if (sD_Transactions == null)
            {
                return HttpNotFound();
            }
            return View(sD_Transactions);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SD_Transactions sD_Transactions = db.SD_Transactions.Find(id);
            db.SD_Transactions.Remove(sD_Transactions);
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
