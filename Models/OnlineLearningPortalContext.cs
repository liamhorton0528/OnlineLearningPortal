using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OnlineLearningPortal.Models
{
	public class OnlineLearningPortalContext : DbContext
    {
        public OnlineLearningPortalContext() : base("OnlineLearningPortalDBContext") { }

        public DbSet<Course> Courses{ get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Thread> Threads { get; set; }
    }
}