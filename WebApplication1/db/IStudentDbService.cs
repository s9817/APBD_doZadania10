using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models_EF;

namespace WebApplication1.db
{
    public interface IStudentDbService
    {
        Student2 UpdateStudent(Student2 student);
        Student2 DeleteStudent(string indexnr);
        IEnumerable<Student2> GetStudents(); 
        IEnumerable<Models.Enrollment> GetEnrollment(string indexNumber);
        bool CheckIndex(string index);
        Student CheckPass(string v1, string v2);
        void setToken(Student2 s, Guid refreshToken);
        Student CheckToken(string refToken);
        void setToken(Student s, Guid refreshToken);
    }
}
