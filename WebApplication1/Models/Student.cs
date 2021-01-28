using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Student
    {
        public int IdStudent {get; set; }
        public string FirstName {get; set; }
        public string LastName {get; set;}
        public string IndexNumer { get; set; }
        public int IdEnrollment { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string Salt { get; set; }

        public virtual Enrollment IdEnrollmentNavigation { get; set; }
    }
}
