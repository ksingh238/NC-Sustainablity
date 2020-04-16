using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ResetPasswordCode { get; set; }
    }
}