using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationStoreAdmin.Models;

namespace WebApplicationStoreAdmin.Controllers
{
    public class HomeController : Controller
    {
        private officia1_StoreEntities db = new officia1_StoreEntities();

        public ActionResult Index()
        {
            var daysToAdd = -7;
            var targetDate = DateTime.Now.AddDays(daysToAdd);

            //User Count
            //ViewBag.UserCount = getUserCount();
            ViewBag.UserCount = 100;

            //Transaction Count
            //var TransactionCountInLastSevenenDay = db.SD_Transactions
            //                        .Where(x => x.PaymentDate <= targetDate)
            //                        .ToList();
            //ViewBag.TransactionCountInLastSevenenDay = TransactionCountInLastSevenenDay.Count;
            ViewBag.TransactionCountInLastSevenenDay = 200;

            //ViewBag.TransactionCount = db.SD_Transactions.Count();
            ViewBag.TransactionCount = 300;


            //Transaction Amount
            //var TransactionAmountInLastSevenenDay = db.SD_Transactions
            //                        .Where(x => x.PaymentDate <= targetDate)
            //                        .ToList();
            //ViewBag.TransactionAmountInLastSevenenDay = TransactionAmountInLastSevenenDay
            //                        .Sum(x => x.SumShoppingBasketPrice);

            ViewBag.TransactionAmountInLastSevenenDay = 400;

            //ViewBag.TransactionAmount = db.SD_Transactions.Sum(x => x.SumShoppingBasketPrice);
            ViewBag.TransactionAmount = 500;

            //Object Counts
            //ViewBag.RemainingObjectCount = db.SD_ProductChargesProperties
            //                        .Sum(x=> x.RemainingCount);
            ViewBag.RemainingObjectCount = 600;

            //ViewBag.AllBuyObjectCount = db.SD_ProductCharges 
            //                        .Sum(x => x.BuyCount);

            ViewBag.AllBuyObjectCount = 700;
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult indexLinks()
        {
            return View();
        }
        public ActionResult IndexUsers()
        {
            //User Count
            ViewBag.UserCount = getUserCount();
            ViewBag.ASPUserCount = getASPUserCount();


            return View();
        }

        private dynamic getUserCount()
        {
            //return db.SD_Users.Count();
            return 52;
        }
        private dynamic getASPUserCount()
        {
            //return db.AspNetUsers.Count();
            return 100;
        }

        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}