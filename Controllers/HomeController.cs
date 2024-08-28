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
            ViewBag.UserCount = db.SD_Users.Count();

            //Transaction Count
            var TransactionCountInLastSevenenDay = db.SD_Transactions
                                    .Where(x => x.PaymentDate <= targetDate)
                                    .ToList();
            ViewBag.TransactionCountInLastSevenenDay = TransactionCountInLastSevenenDay
                                    .Count;
            ViewBag.TransactionCount = db.SD_Transactions.Count();


            //Transaction Amount
            var TransactionAmountInLastSevenenDay = db.SD_Transactions
                                    .Where(x => x.PaymentDate <= targetDate)
                                    .ToList();
            ViewBag.TransactionAmountInLastSevenenDay = TransactionAmountInLastSevenenDay
                                    .Sum(x => x.SumShoppingBasketPrice);
            ViewBag.TransactionAmount = db.SD_Transactions
                                    .Sum(x => x.SumShoppingBasketPrice);

            //Object Counts
            ViewBag.RemainingObjectCount = db.SD_ProductChargesProperties
                                    .Sum(x=> x.RemainingCount);


            ViewBag.AllBuyObjectCount = db.SD_ProductCharges 
                                    .Sum(x => x.BuyCount);
            return View();
        }
        public ActionResult Index3()
        {
            return View();
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