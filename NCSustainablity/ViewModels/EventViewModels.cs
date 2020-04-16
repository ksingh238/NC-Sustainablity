using NCSustainablity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.ViewModels
{
    public class GetEditEventViewModels
    {
        public Event Event { get; set; }
        public IEnumerable<EventCategory> EventCategories { get; set; }
    }
    public class EventsEventViewModel
    {
        public IEnumerable<EventsSubscribers> EventsSubscribers { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Event> SubscribedEvents { get; set; }
        public int StudentId { get; set; }
    }
}