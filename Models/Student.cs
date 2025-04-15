using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public string ImagePath { get; set; }
    }

}