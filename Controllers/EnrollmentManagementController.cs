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

namespace OnlineLearningPortal.Controllers
{
    [Auth("ADMIN")]
    public class EnrollmentManagementController : Controller
    {
        private OnlineLearningPortalContext db = new OnlineLearningPortalContext();

        // GET: Enrollments
        public ActionResult Index(string searchQuery, string sortOrder)
        {
            ViewBag.DateSortParam = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.CourseSortParam = sortOrder == "course" ? "course_desc" : "course";
            ViewBag.StudentSortParam = sortOrder == "student" ? "student_desc" : "student";

            var enrollments = db.Enrollments.Include(e => e.Course).Include(e => e.Student);

            if (!String.IsNullOrEmpty(searchQuery))
            {
                enrollments = enrollments.Where(e => e.Student.Name.Contains(searchQuery) || e.Course.Title.Contains(searchQuery));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    enrollments = enrollments.OrderByDescending(e => e.EnrollmentDate);
                    break;
                case "course":
                    enrollments = enrollments.OrderBy(e => e.Course.Title);
                    break;
                case "course_desc":
                    enrollments = enrollments.OrderByDescending(e => e.Course.Title);
                    break;
                case "student":
                    enrollments = enrollments.OrderBy(e => e.Student.Name);
                    break;
                case "student_desc":
                    enrollments = enrollments.OrderByDescending(e => e.Student.Name);
                    break;
                default:
                    enrollments = enrollments.OrderBy(e => e.EnrollmentDate);
                    break;
            }

            return View(enrollments.ToList());
        }

        // GET: Enrollments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollments/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title");
            ViewBag.StudentId = new SelectList(db.Students, "Id", "Name");
            return View();
        }

        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CourseId,StudentId,EnrollmentDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", enrollment.CourseId);
            ViewBag.StudentId = new SelectList(db.Students, "Id", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", enrollment.CourseId);
            ViewBag.StudentId = new SelectList(db.Students, "Id", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CourseId,StudentId,EnrollmentDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", enrollment.CourseId);
            ViewBag.StudentId = new SelectList(db.Students, "Id", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
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
