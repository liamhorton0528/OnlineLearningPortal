using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineLearningPortal.Models;
using OnlineLearningPortal.Filters;
using System.IO;

namespace OnlineLearningPortal.Controllers
{
    [Auth("ADMIN")]
    public class CourseManagementController : Controller
    {
        private OnlineLearningPortalContext db = new OnlineLearningPortalContext();

        // GET: Courses
        public ActionResult Index(string searchQuery, string sortOrder)
        {
            ViewBag.TitleSortParam = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

            var courses = db.Courses.Include(c => c.Instructor);

            if (!String.IsNullOrEmpty(searchQuery))
            {
                courses = courses.Where(c => c.Title.Contains(searchQuery) || c.Instructor.Name.Contains(searchQuery));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    courses = courses.OrderByDescending(c => c.Title);
                    break;
                default:
                    courses = courses.OrderBy(c => c.Title);
                    break;
            }

            return View(courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            ViewBag.InstructorId = new SelectList(db.Instructors, "Id", "Name", "TimeSlot");
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course course, HttpPostedFileBase ImagePath)
        {
            if (ModelState.IsValid)
            {
                if (ImagePath != null && ImagePath.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImagePath.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/Images/"), fileName);
                    ImagePath.SaveAs(path);

                    course.ImagePath = "~/Uploads/Images/" + fileName;
                }

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorId = new SelectList(db.Instructors, "Id", "Name", course.InstructorId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstructorId = new SelectList(db.Instructors, "Id", "Name", course.InstructorId);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,InstructorId,TimeSlot")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InstructorId = new SelectList(db.Instructors, "Id", "Name", course.InstructorId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
