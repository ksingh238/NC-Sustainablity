using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NCSustainablity.Models;

namespace NCSustainablity.Controllers
{
    public class StudentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            GlobalData.SetLoginStatus(false);
            return RedirectToAction("Index", "Index");
        }
    }
}