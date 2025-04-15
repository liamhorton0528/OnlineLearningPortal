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
    [Auth("INSTRUCTOR")]
    public class InstructorController : Controller
    {
        OnlineLearningPortalContext db = new OnlineLearningPortalContext();
        // GET: Instructor
        public ActionResult Index()
        {
            return View();
        }

        // GET: MyCourses
        public ActionResult MyCourses()
        {
            int instructorId = Convert.ToInt32(Session["UserId"]);
            var myCourses = db.Courses.Where(c => c.InstructorId == instructorId).ToList();

            return View(myCourses);
        }

        // GET: Instructor/Course/?
        public ActionResult Course(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Include(c => c.Assignments.Select(a => a.Submissions)).Include(c => c.Announcements).FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        [HttpPost]
        public ActionResult UploadCourseImage(int courseId, HttpPostedFileBase courseBanner)
        {
            if(courseBanner != null && courseBanner.ContentLength > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string serverPath = Server.MapPath("~/Uploads/Images/");

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                string extension = Path.GetExtension(courseBanner.FileName);
                string savedFileName = $"course{courseId}-imgbanner-{timestamp}{extension}";
                string fullPath = Path.Combine(serverPath, savedFileName);
                courseBanner.SaveAs(fullPath);

                var course = db.Courses.Find(courseId);
                course.ImagePath = "~/Uploads/Images/" + savedFileName;
                db.SaveChanges();
            }
            else
            {
                TempData["ImageError"] = "Please select a valid image to upload.";
            }

            return RedirectToAction("Course", new {id = courseId});
        }

        public PartialViewResult CreateAnnouncement(string title, string message, int courseId)
        {
            Announcement newAnnouncement = new Announcement
            {
                Title = title,
                Message = message,
                CourseId = courseId
            };

            db.Announcements.Add(newAnnouncement);
            db.SaveChanges();

            var announcements = db.Announcements.Where(a => a.CourseId == courseId).ToList();
            return PartialView("_Announcements", announcements);
        }

        [HttpPost]
        public PartialViewResult CreateAssignment(string name, string description, DateTime dueDate, int courseId, HttpPostedFileBase assignmentDoc)
        {
            if(assignmentDoc != null && assignmentDoc.ContentLength > 0)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string serverPath = Server.MapPath("~/Uploads");

                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }

                    string extension = Path.GetExtension(assignmentDoc.FileName);
                    string savedFileName = $"course{courseId}-assign-{timestamp}{extension}";
                    string fullPath = Path.Combine(serverPath, savedFileName);
                    assignmentDoc.SaveAs(fullPath);

                    Assignment newAssignment = new Assignment
                    {
                        Name = name,
                        Description = description,
                        DueDate = dueDate,
                        CourseId = courseId,
                        DocumentFilePath = "~/Uploads/" + savedFileName
                    };

                    db.Assignments.Add(newAssignment);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Assignment upload error: " + ex.Message;
                }
            }
            else
            {
                Assignment newAssignment = new Assignment
                {
                    Name = name,
                    Description = description,
                    DueDate = dueDate,
                    CourseId = courseId
                };

                db.Assignments.Add(newAssignment);
                db.SaveChanges();
            }

            var assignments = db.Assignments.Where(a => a.CourseId == courseId).ToList();
            return PartialView("_Assignments", assignments);
        }

        public ActionResult ViewSubmissions(int assignmentId)
        {
            var assignment = db.Assignments.Include("Submissions.Student").FirstOrDefault(a => a.Id == assignmentId);

            return View(assignment);
        }

        public PartialViewResult SelectSubmission(int submissionId)
        {
            var submission = db.Submissions.Include("Student").Include("Assignment").FirstOrDefault(s => s.Id == submissionId);

            return PartialView("_Submission", submission);
        }

        [HttpPost]
        public ActionResult GradeSubmission(int submissionId, int grade)
        {
            var submission = db.Submissions.Find(submissionId);

            if(submission ==  null)
            {
                return HttpNotFound();
            }

            submission.Grade = grade;
            db.SaveChanges();

            var gradedSubmission = db.Submissions.Include("Student").FirstOrDefault(s => s.Id == submissionId);

            return PartialView("_Submission", gradedSubmission);
        }

        // GET: View my Information
        public ActionResult InstructorInfo()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            int id = Convert.ToInt32(Session["UserId"]);
            var instructor = db.Instructors.Find(id);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }

        // GET: Edit my info
        public ActionResult EditInstructor(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var instructor = db.Instructors.Find(id);
            if (instructor == null)
                return HttpNotFound();

            return View(instructor);
        }

        // POST: Save updated info
        [HttpPost]
        public ActionResult EditInstructor([Bind(Include = "Id,Name,Email,Office,Phone,Password")] Instructor instructor, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload BEFORE marking as modified
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string extension = Path.GetExtension(ImageFile.FileName);
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string uniqueFileName = $"instructor-profile-{timestamp}{extension}";
                    string filePath = Path.Combine(Server.MapPath("~/Uploads/Images/"), uniqueFileName);
                    ImageFile.SaveAs(filePath);

                    instructor.ImagePath = "~/Uploads/Images/" + uniqueFileName;
                }
                else
                {
                    // Preserve existing image if none uploaded
                    var existing = db.Instructors.AsNoTracking().FirstOrDefault(i => i.Id == instructor.Id);
                    instructor.ImagePath = existing?.ImagePath;
                }

                db.Entry(instructor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("InstructorInfo");
            }

            return View(instructor);
        }


        // GET: CreateCourse
        public ActionResult CreateCourse()
        {
            int instructorId = Convert.ToInt32(Session["UserId"]);
            var allCourses = db.Courses.ToList();  // Get all courses
            var instructorCourses = db.Courses.Where(c => c.InstructorId == instructorId).ToList();  // Get courses assigned to the instructor

            // Create a list to pass to the view that contains course info and whether the instructor has it assigned
            var courseViewModel = allCourses.Select(course => new CourseViewModel
            {
                Course = course,
                IsAssigned = instructorCourses.Any(c => c.Id == course.Id)  // Check if the instructor is assigned to this course
            }).ToList();

            return View(courseViewModel);
        }

        // POST: Assign Course to Instructor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignCourse(int courseId)
        {
            int instructorId = Convert.ToInt32(Session["UserId"]);

            var course = db.Courses.Find(courseId);
            if (course != null)
            {
                // Assign the course to the instructor
                course.InstructorId = instructorId;
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("MyCourses"); // Redirect to MyCourses to see the newly assigned course
        }

        // POST: DropCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DropCourse(int courseId)
        {
            int? instructorId = Session["UserId"] as int?;

            if (instructorId == null)
            {
                return RedirectToAction("Login"); // or show an error message
            }

            var course = db.Courses.Find(courseId);
            if (course != null && course.InstructorId == instructorId)
            {
                // Now that InstructorId is nullable, setting it to null is allowed
                course.InstructorId = null; // This will set the instructor to null
                db.SaveChanges();
            }

            return RedirectToAction("MyCourses");
        }

        [HttpPost]
        public ActionResult CreateNewCourse(string title, string description, string timeslot)
        {
            int instructorId = Convert.ToInt32(Session["UserId"]);

            Course newCourse = new Course
            {
                Title = title,
                Description = description,
                TimeSlot = timeslot,
                InstructorId = instructorId
            };

            db.Courses.Add(newCourse);
            db.SaveChanges();

            return RedirectToAction("CreateCourse");
        }

        public ActionResult MyMessages()
        {
            int instructorId = Convert.ToInt32(Session["UserId"]);

            var threads = db.Threads
                .Include(t => t.Messages)
                .Where(t =>
                    (t.User_1_role == "INSTRUCTOR" && t.User_1_id == instructorId) ||
                    (t.User_2_role == "INSTRUCTOR" && t.User_2_id == instructorId))
                .ToList();

            var existingUserKeys = threads
                .Select(t => t.User_1_id == instructorId ? $"{t.User_2_role}|{t.User_2_id}" : $"{t.User_1_role}|{t.User_1_id}")
                .ToHashSet();

            var instructors = db.Instructors
                .Where(i => i.Id != instructorId)
                .Select(i => new { Id = i.Id, Name = i.Name, Role = "INSTRUCTOR" })
                .ToList(); 

            var students = db.Students
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

        [HttpPost]
        public PartialViewResult SelectThread(int threadId)
        {
            var thread = db.Threads.Include(t => t.Messages).FirstOrDefault(t => t.Id == threadId);

            return PartialView("_Thread", thread);
        }

        [HttpPost]
        public PartialViewResult SendMessage(string content, int threadId)
        {
            var instructorId = Convert.ToInt32(Session["UserId"]);
            var sender = db.Instructors.Where(i => i.Id == instructorId).FirstOrDefault().Name;

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
            System.Diagnostics.Debug.WriteLine("Recipient: " + recipient);
            var parts = recipient.Split('|');
            var role = parts[0];
            var id = int.Parse(parts[1]);

            var instructorId = Convert.ToInt32(Session["UserId"]);
            var user_1 = db.Instructors.FirstOrDefault(i => i.Id == instructorId);
            string user_2_name = "";

            if (role=="INSTRUCTOR")
            {
                user_2_name = db.Instructors.FirstOrDefault(i => i.Id == id).Name;
            }
            else
            {
                user_2_name = db.Students.FirstOrDefault(i => i.Id == id).Name;
            }

            Thread newThread = new Thread
            {
                User_1_role = "INSTRUCTOR",
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
                    (t.User_1_role == "INSTRUCTOR" && t.User_1_id == instructorId) ||
                    (t.User_2_role == "INSTRUCTOR" && t.User_2_id == instructorId))
                .ToList();

            return RedirectToAction("MyMessages");
        }
    }
}