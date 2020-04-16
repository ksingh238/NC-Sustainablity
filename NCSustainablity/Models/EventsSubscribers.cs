using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.Models
{
    public class EventsSubscribers
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int EventId { get; set; }
        public virtual Event Event { get; set; }
        public virtual Student Student { get; set; }
    }
}