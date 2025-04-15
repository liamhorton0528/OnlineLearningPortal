using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public bool IsAssigned { get; set; }
    }
}