using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int CourseId { get; set; }
    }
}