using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.dto.request;
using WebApplication1.dto.response;
using WebApplication1.Models_EF;

namespace WebApplication1.db
{
    public class EnrollmentDbService : IEnrollmentDbService
    {
        private string sqlConn = "Data Source=db-mssql;Initial Catalog = s9817; Integrated Security = True";
        public Models_EF.Enrollment EnrollStudent(EnrollStudentRequest request)
        {

            var enrollresp = new Models_EF.Enrollment();
            var db = new s9817Context();
            var czyjest = db.Student2.SingleOrDefault(st => st.IndexNumber == request.IndexNumber);
            if (czyjest != null) { return null; }

            var idStudies = db.Studies.SingleOrDefault(st => st.Name == request.Studies);
            if (idStudies == null) { return null; }

            enrollresp.IdStudy = idStudies;

            var enrl = db.Enrollment.FirstOrDefault(en => en.IdStudy == idStudies && en.Semester == 1);

            if (enrl == null)
            {
                int enrollmentnr;
                var nrenrl = db.Enrollment.OrderBy(enrl => enrl.IdEnrollment).FirstOrDefault();
                if (nrenrl == null)
                    enrollmentnr = 1;
                else
                    enrollmentnr = nrenrl.IdEnrollment + 1;

                enrl = new Models_EF.Enrollment()
                {
                    IdEnrollment = enrollmentnr,
                    Semester = 1,
                    IdStudy = idStudies,
                    StartDate = DateTime.Now
                };
                db.Enrollment.Add(enrl);
                db.SaveChanges();
            };

            var st = new Student2();
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.IdEnrollment = enrl.IdEnrollment;

            db.Add(st);
            db.SaveChanges();

            return enrl;

        }

        public EnrollStudentResponse PromoteStudent(PromotionRequest prStudent)
        {
            var enrollresp = new EnrollStudentResponse();
            var db = new s9817Context();

            var studies = db.Studies.SingleOrDefault(st => st.Name.Equals( prStudent.Studies));
            if (studies == null)
                return null;   
            else  
            {
                int semNext = prStudent.Semester++; 
                var enrNow = db.Enrollment.SingleOrDefault(st => st.Semester == prStudent.Semester && st.IdStudy == studies.IdStudy);

                if (enrNow == null)
                    return null;
                else
                {
                    var enrlNext = db.Enrollment.SingleOrDefault(st => st.Semester == semNext && st.IdStudy == studies.IdStudy);
                    if (enrlNext == null)
                    {
                        var newEnroll = new Models_EF.Enrollment();
                        newEnroll.IdStudy = studies.IdStudy;
                        newEnroll.Semester = semNext;

                        db.Enrollment.Add(newEnroll);
                        db.SaveChanges();


                        var students = db.Student2.Where(stud => stud.IdEnrollment == enrNow.IdEnrollment);
                        students.ForEachAsync(st => st.IdEnrollment = semNext);
                        db.SaveChanges();

                        enrollresp.IdEnrollment = newEnroll.IdEnrollment;
                        enrollresp.Semester = semNext;
                        enrollresp.StartDate = DateTime.Now;


                        return enrollresp;
                    }
                    else 
                    {

                        var students = db.Student2.Where(stud => stud.IdEnrollment == enrNow.IdEnrollment);
                        students.ForEachAsync(st => st.IdEnrollment = enrlNext.IdEnrollment);
                        db.SaveChanges();

                        enrollresp.IdEnrollment = enrlNext.IdEnrollment;
                        enrollresp.Semester = semNext;
                        enrollresp.StartDate = DateTime.Now;


                        return enrollresp;
                    }
                }
            }
        }
    }
}
