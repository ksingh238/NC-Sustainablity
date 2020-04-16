using NCSustainablity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.ViewModels
{
    public class GetEditPostViewModel
    {
        public List<EventCategory> EventCategories { get; set; }
        public Post Post { get; set; }
    }
}