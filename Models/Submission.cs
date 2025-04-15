using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public string FileName { get; set;  }
        public string SubmissionFilePath { get; set; }
        public int? Grade {  get; set; }
    }
}