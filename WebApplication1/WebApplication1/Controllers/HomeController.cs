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
            //ViewBag.Username = usr.UserName;
            //ViewBag.UserId = usr.Id;
            //return View();
        }
    }
}