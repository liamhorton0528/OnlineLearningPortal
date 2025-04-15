using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{

    public class Thread
    {
        public int Id { get; set; }
        public string User_1_role { get; set; }
        public int User_1_id { get; set; }
        public string User_1_name { get; set; }
        public string User_2_role { get; set; }
        public int User_2_id { get; set; }
        public string User_2_name { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}