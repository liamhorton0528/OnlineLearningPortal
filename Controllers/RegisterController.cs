using System;
using System.Linq;
using System.Web.Mvc;
using OnlineLearningPortal.Models;

namespace OnlineLearningPortal.Controllers
{
    public class RegisterController : Controller
    {
        private OnlineLearningPortalContext db = new OnlineLearningPortalContext();

        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fix the errors.";
                return View("Index", model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Error = "Password and Confirm Password do not match.";
                return View("Index", model);
            }

            // Check if email already exists
            bool emailExists =
                db.Admins.Any(a => a.Email == model.Email) ||
                db.Instructors.Any(i => i.Email == model.Email) ||
                db.Students.Any(s => s.Email == model.Email);

            if (emailExists)
            {
                ViewBag.Error = "Email already in use.";
                return View("Index", model);
            }

            if (model.Role == "INSTRUCTOR")
            {
                db.Instructors.Add(new Instructor { Email = model.Email, Password = model.Password });
            }
            else if (model.Role == "STUDENT")
            {
                db.Students.Add(new Student { Email = model.Email, Password = model.Password });
            }

            db.SaveChanges();
            ViewBag.Success = "Registration successful. Please login.";
            return RedirectToAction("Index", "Login");
        }
    }
}
