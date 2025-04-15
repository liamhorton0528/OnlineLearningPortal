using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OnlineLearningPortal.Models;

namespace OnlineLearningPortal.Controllers
{
    public class LoginController : Controller
    {
        private OnlineLearningPortalContext db = new OnlineLearningPortalContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(User model)
        {

            if (model.Role == "ADMIN")
            {
                var user = db.Admins.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["UserRole"] = "ADMIN";
                    var authTicket = new FormsAuthenticationTicket(1, user.Email, DateTime.Now, DateTime.Now.AddMinutes(60), false, user.Email);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);

                    return RedirectToAction("Index", "Admin");
                }
            }
            else if (model.Role == "INSTRUCTOR")
            {
                var user = db.Instructors.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["UserRole"] = "INSTRUCTOR";
                    var authTicket = new FormsAuthenticationTicket(1, user.Email, DateTime.Now, DateTime.Now.AddMinutes(60), false, user.Email);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);
                    return RedirectToAction("Index", "Instructor");
                }
            }
            else if (model.Role == "STUDENT")
            {
                var user = db.Students.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["UserRole"] = "STUDENT";
                    var authTicket = new FormsAuthenticationTicket(1, user.Email, DateTime.Now, DateTime.Now.AddMinutes(60), false, user.Email);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);
                    return RedirectToAction("Index", "Student");
                }
            }

            ViewBag.Error = "Invalid Login Credentials";
            return View("Index", model);
        }
    }
}