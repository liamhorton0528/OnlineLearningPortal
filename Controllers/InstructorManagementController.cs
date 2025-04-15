using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineLearningPortal.Filters;
using OnlineLearningPortal.Models;

namespace OnlineLearningPortal.Controllers
{
    [Auth("ADMIN")]
    public class InstructorManagementController : Controller
    {
        private OnlineLearningPortalContext db = new OnlineLearningPortalContext();

        // GET: Instructors
        public ActionResult Index(string searchQuery, string sortOrder)
        {
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.EmailSortParam = sortOrder == "email" ? "email_desc" : "email";

            var instructors = db.Instructors.AsQueryable();

            if (!String.IsNullOrEmpty(searchQuery))
            {
                instructors = instructors.Where(i =>
                    i.Name.Contains(searchQuery) || i.Email.Contains(searchQuery) || i.Office.Contains(searchQuery));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    instructors = instructors.OrderByDescending(i => i.Name);
                    break;
                case "email":
                    instructors = instructors.OrderBy(i => i.Email);
                    break;
                case "email_desc":
                    instructors = instructors.OrderByDescending(i => i.Email);
                    break;
                default:
                    instructors = instructors.OrderBy(i => i.Name);
                    break;
            }

            return View(instructors.ToList());
        }

        // GET: Instructors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Office,Phone")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Office,Phone")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(instructor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(instructor);
        }

        // GET: Instructors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors.Find(id);
            db.Instructors.Remove(instructor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
