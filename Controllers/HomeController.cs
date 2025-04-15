using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OnlineLearningPortal.Filters;

namespace OnlineLearningPortal.Controllers
{
    [Auth()]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1), // Expire it immediately
                    HttpOnly = true
                };
                Response.Cookies.Add(authCookie);
            }
            return RedirectToAction("Index", "Login");
        }
    }
}