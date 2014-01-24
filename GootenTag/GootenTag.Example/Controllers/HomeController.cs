using System;
using System.Web.Mvc;

namespace GootenTag.Example.Controllers
{
    public class HomeController : Controller
    {
        [GoogleTagManagerVariable("page_type", "home")]
        public ActionResult Index()
        {
            GoogleTagManager.Current.DataLayer.CustomerId = Guid.NewGuid();
            GoogleTagManager.Current.DataLayer.TestString = "GootenTag's test";
            GoogleTagManager.Current.DataLayer.TestNumber = 123;
            GoogleTagManager.Current.DataLayer.TestArray = new[] { "value1", "value2", "value3" };
            return View();
        }
    }
}