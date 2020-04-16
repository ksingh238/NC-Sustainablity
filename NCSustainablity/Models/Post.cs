using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ApprovalStatus { get; set; }
        public int EventCategoryId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        public virtual EventCategory EventCategory { get; set; }
    }
}