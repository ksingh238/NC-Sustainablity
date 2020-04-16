using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SurveyNo { get; set; }
        public int Status { get; set; }
        public Student Student { get; set; }
    }
}