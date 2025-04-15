using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public ICollection<Course> Courses { get; set; }
        public string ImagePath { get; set; } = "";
    }

}