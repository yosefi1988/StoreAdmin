using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;

namespace WebApplicationCar.Controllers
{
    public class ProductsController : Controller
    {
        // صفحه لیست
        public ActionResult Index()
        {
            return View();
        }

        // AJAX: لیست محصولات - مستقیم از DataContext
        [HttpGet]
        public JsonResult GetProducts(int page = 1, int pageSize = 20)
        {
            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var query = from p in db.X_Products
                                join r in db.X_Resources on p.FK_ResourceId equals r.ResourceId
                                where p.IsActive
                                orderby p.ProductId
                                select new
                                {
                                    p.ProductId,
                                    r.NameFa,
                                    r.NameEn,
                                    p.ProductCode,
                                    p.Barcode,
                                    p.IsActive,
                                    p.CreatedAt
                                };

                    var total = query.Count();
                    var totalPages = (int)Math.Ceiling((double)total / pageSize);

                    var data = query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    return Json(new
                    {
                        Total = total,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages,
                        Data = data
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}