using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public string DocumentFilePath { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }
}