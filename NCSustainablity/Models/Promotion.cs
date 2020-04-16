using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCSustainablity.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Offer { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        [DataType(DataType.Date)]
        public string ContactNo { get; set; }
        public int Status { get; set; }
    }
}