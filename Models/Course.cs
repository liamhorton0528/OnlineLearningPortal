using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeSlot { get; set; }
        public int? InstructorId { get; set; }
        public string ImagePath { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<Announcement> Announcements { get; set; }
    }

}