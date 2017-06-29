using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index( AspNetUser usr)
        {
            return View();
        }
        public ActionResult HR(AspNetUser usr)
        {
            return View();

        }
        public ActionResult Sales(AspNetUser usr)
        {
            return View();
        }
        public ActionResult Administration(AspNetUser usr)
        {
            return View();
        }
        public ActionResult IT(AspNetUser usr)
        {
            return View();
        }
    }
}