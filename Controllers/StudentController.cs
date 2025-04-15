using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineLearningPortal.Filters;
using OnlineLearningPortal.Models;

namespace OnlineLearningPortal.Controllers
{
    [Auth("STUDENT")]
    public class StudentController : Controller
    {
        OnlineLearningPortalContext db = new OnlineLearningPortalContext();
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        // GET: Student/MyCourses
        public ActionResult MyCourses()
        {
            int studentId = Convert.ToInt32(Session["UserId"]);
            var courseIds = db.Enrollments.Where(e => e.StudentId == studentId).Select(e => e.CourseId).ToList();
            var enrolledCourses = db.Courses.Include(c => c.Instructor).Where(c => courseIds.Contains(c.Id)).ToList(); ;

            return View(enrolledCourses);
        }

        // GET: Student/Course/?
        public ActionResult Course(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Include(c => c.Instructor).Include(c => c.Assignments.Select(a => a.Submissions)).Include(c => c.Announcements).FirstOrDefault(c => c.Id == id);
            if(course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        [HttpPost]
        public PartialViewResult SubmitAssignment(int assignmentId, int courseId, HttpPostedFileBase submission)
        {
            if (submission != null && submission.ContentLength > 0)
            {
                try
                {
                    int studentId = Convert.ToInt32(Session["UserId"]);
                    string fileName = Path.GetFileName(submission.FileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); 
                    string serverPath = Server.MapPath("~/Uploads");

                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }

                    string extension = Path.GetExtension(submission.FileName);
                    string savedFileName = $"assign{assignmentId}-stu{studentId}-{timestamp}{extension}";
                    string fullPath = Path.Combine(serverPath, savedFileName);
                    submission.SaveAs(fullPath);

                    Submission newSubmission = new Submission
                    {
                        AssignmentId = assignmentId,
                        StudentId = studentId,
                        FileName = fileName,
                        SubmissionFilePath = "~/Uploads/" + savedFileName,
                        Grade = null
                    };

                    db.Submissions.Add(newSubmission);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.assignmentId = assignmentId;
                    ViewBag.SubmissionError = "Error during submission: " + ex.Message;
                }
            }
            else
            {

                ViewBag.assignmentId = assignmentId;
                ViewBag.SubmissionError = "Please select a valid file to submit.";
            }

            var assignments = db.Assignments
                .Include("Submissions")
                .Where(a => a.CourseId == courseId)
                .ToList();

            return PartialView("_Assignments", assignments);
        }


        public ActionResult Enroll()
        {
            int studentId = Convert.ToInt32(Session["UserId"]);
            var courseIds = db.Enrollments.Where(e => e.StudentId == studentId).Select(e => e.CourseId).ToList();
            var availableCourses = db.Courses.Where(c => !courseIds.Contains(c.Id)).ToList();

            return View(availableCourses);
        }

        public ActionResult EnrollInCourse(int courseId)
        {
            int studentId = Convert.ToInt32(Session["UserId"]);
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime enrollmentDate = DateTime.Parse(todayDate);

            Enrollment enroll = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = enrollmentDate
            };

            db.Enrollments.Add(enroll);
            db.SaveChanges();

            // Get updated list of courses that the student is not enrolled in
            var courseIds = db.Enrollments.Where(e => e.StudentId == studentId).Select(e => e.CourseId).ToList();
            var availableCourses = db.Courses.Where(c => !courseIds.Contains(c.Id)).ToList();

            return PartialView("_EnrollList", availableCourses); 
        }

        // GET: View my information
        public ActionResult StudentInfo()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            int id = Convert.ToInt32(Session["UserId"]);
            var student = db.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // GET: Student/Edit
        public ActionResult Edit()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            int studentId = Convert.ToInt32(Session["UserId"]);
            var student = db.Students.Find(studentId);

            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,BirthDate")] Student student, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var studentInDb = db.Students.Find(student.Id);
                if (studentInDb == null)
                {
                    return HttpNotFound();
                }

                studentInDb.Name = student.Name;
                studentInDb.Email = student.Email;
                studentInDb.BirthDate = student.BirthDate;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string extension = Path.GetExtension(ImageFile.FileName);
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Images/"), fileName);
                    ImageFile.SaveAs(path);

                    studentInDb.ImagePath = "~/Uploads/Images/" + fileName;
                }

                db.Entry(studentInDb).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("StudentInfo");
            }

            return View(student);
        }


        public ActionResult MyMessages()
        {
            int studentId = Convert.ToInt32(Session["UserId"]);

            var threads = db.Threads
                .Include(t => t.Messages)
                .Where(t =>
                    (t.User_1_role == "STUDENT" && t.User_1_id == studentId) ||
                    (t.User_2_role == "STUDENT" && t.User_2_id == studentId))
                .ToList();

            var existingUserKeys = threads
                .Select(t => t.User_1_id == studentId ? $"{t.User_2_role}|{t.User_2_id}" : $"{t.User_1_role}|{t.User_1_id}")
                .ToHashSet();

            var instructors = db.Instructors
                .Select(i => new { Id = i.Id, Name = i.Name, Role = "INSTRUCTOR" })
                .ToList();

            var students = db.Students
                .Where(i => i.Id != studentId)
                .Select(s => new { Id = s.Id, Name = s.Name, Role = "STUDENT" })
                .ToList();

            var users = instructors
                .Concat(students)
                .Select(u => new
                {
                    Key = $"{u.Role}|{u.Id}",
                    Text = $"[{u.Role}] - {u.Name}"
                })
                .Where(u => !existingUserKeys.Contains(u.Key))
                .Select(u => new SelectListItem
                {
                    Value = u.Key,
                    Text = u.Text
                })
                .ToList();

            ViewBag.AllUsers = new SelectList(users, "Value", "Text");

            return View(threads);
        }


        public PartialViewResult SelectThread(int threadId)
        {
            var thread = db.Threads.Include(t => t.Messages).FirstOrDefault(t => t.Id == threadId);

            return PartialView("_Thread", thread);
        }

        [HttpPost]
        public PartialViewResult SendMessage(string content, int threadId)
        {
            var studentId = Convert.ToInt32(Session["UserId"]);
            var sender = db.Students.Where(i => i.Id == studentId).FirstOrDefault().Name;

            Message newMessage = new Message
            {
                ThreadId = threadId,
                Content = content,
                Sender = sender,
                Timestamp = DateTime.Now
            };
            db.Messages.Add(newMessage);
            db.SaveChanges();

            var thread = db.Threads.Include(t => t.Messages).FirstOrDefault(t => t.Id == threadId);
            if (thread != null)
            {
                thread.Messages = thread.Messages
                    .OrderBy(m => m.Timestamp)
                    .ToList();
            }

            return PartialView("_Thread", thread);
        }

        [HttpPost]
        public ActionResult CreateThread(string recipient)
        {
            var parts = recipient.Split('|');
            var role = parts[0];
            var id = int.Parse(parts[1]);

            var studentId = Convert.ToInt32(Session["UserId"]);
            var user_1 = db.Students.FirstOrDefault(i => i.Id == studentId);
            string user_2_name = "";

            if (role == "INSTRUCTOR")
            {
                user_2_name = db.Instructors.FirstOrDefault(i => i.Id == id).Name;
            }
            else
            {
                user_2_name = db.Students.FirstOrDefault(i => i.Id == id).Name;
            }

            Thread newThread = new Thread
            {
                User_1_role = "STUDENT",
                User_1_id = user_1.Id,
                User_1_name = user_1.Name,
                User_2_role = role,
                User_2_id = id,
                User_2_name = user_2_name
            };

            db.Threads.Add(newThread);
            db.SaveChanges();

            var threads = db.Threads
                .Include(t => t.Messages)
                .Where(t =>
                    (t.User_1_role == "STUDENT" && t.User_1_id == studentId) ||
                    (t.User_2_role == "STUDENT" && t.User_2_id == studentId))
                .ToList();

            return RedirectToAction("MyMessages");
        }
    }
}