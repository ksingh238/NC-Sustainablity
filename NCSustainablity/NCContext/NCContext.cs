using NCSustainablity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NCSustainablity.NCContext
{
    public class NCContext : DbContext
    {
        public NCContext() : base("NCSustainability")
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EventsSubscribers> EventsSubscribers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
    }
}