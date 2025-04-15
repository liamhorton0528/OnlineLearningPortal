using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLearningPortal.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ThreadId { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public DateTime Timestamp { get; set; }
    }
}