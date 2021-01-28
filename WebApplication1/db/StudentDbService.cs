using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models_EF;

namespace WebApplication1.db
{
    public class StudentDbService : IStudentDbService
    {
        private static string sqlConn = "Data Source=db-mssql;Initial Catalog = s9817; Integrated Security = True";

        public IEnumerable<Models.Enrollment> GetEnrollment(string indexNumber)
        {
            var output = new List<Models.Enrollment>();
            using (var client = new SqlConnection(sqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT * from Enrollment where IdEnrollment = (select IdEnrollment from Student2 where IndexNumber = @indexNumber)";
                    command.Parameters.AddWithValue("indexNumber", indexNumber);
                    client.Open();
                    var dr = command.ExecuteReader(); 

                    while (dr.Read())
                    {
                        output.Add(new Models.Enrollment
                        {

                            IdEnrollment = int.Parse(dr["IdEnrollment"].ToString()),
                            Semester = int.Parse(dr["Semester"].ToString()),
                            IdStudy = int.Parse(dr["IdStudy"].ToString()),
                            StartDate = DateTime.Parse(dr["StartDate"].ToString())
                        });
                    }

                }
            }
            return output;
        }

        public IEnumerable<Models_EF.Student2> GetStudents()
        {
            var db = new s9817Context();
            return db.Student2.ToList();
        }

        public Student2 UpdateStudent(Student2 student)
        {

            var db = new s9817Context();
            var st = db.Student2.SingleOrDefault(s => s.IndexNumber == student.IndexNumber);

            if (st == null)
                return null;
            else
            {
                st.FirstName = student.FirstName;
                st.LastName = student.LastName;
                st.IdEnrollment = student.IdEnrollment;

                db.Attach(st);
                db.SaveChanges();

                return student;
            }
        }

        public Student2 DeleteStudent(string indexnr)
        {

            var db = new s9817Context();
            var st = db.Student2.SingleOrDefault(s => s.IndexNumber == indexnr);

            if (st == null) return null;

            db.Attach(st);
            db.Remove(st);

            db.SaveChanges();
            return st;

        }

        public bool CheckIndex(string index)
        {
            bool wynik = false;
            using (var client = new SqlConnection(sqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;

                    client.Open();
                    var tran = client.BeginTransaction();
                    command.Transaction = tran;
                    try
                    {
                        command.CommandText = "select * from Student2 where IndexNumber= @index";
                        command.Parameters.AddWithValue("index", index);
                        var dr = command.ExecuteReader();
                        if (!dr.Read())
                        {
                            wynik = false;
                        }
                        else
                            wynik = true;
                    }
                    catch (SqlException e){}
                }
                return wynik;
            }
        }

        public Student CheckPass(string login, string pass)
        {
            var student = new Student();
            using (var client = new SqlConnection(sqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;

                    client.Open();
                    var tran = client.BeginTransaction();
                    command.Transaction = tran;
                    try
                    {
                        command.CommandText = "select * from Student2 where IndexNumber= @index";
                        command.Parameters.AddWithValue("index", login);
                        var dr = command.ExecuteReader();
                        if (!dr.Read())
                        {
                            return null;
                        }
                        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2( 
                            password: pass,
                            salt: Encoding.UTF8.GetBytes(dr["salt"].ToString()),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 10000,
                            numBytesRequested: 256 / 8));

                        if (hashed == dr["password"].ToString())
                        {
                            student.IndexNumer = dr["IndexNumber"].ToString();
                            student.FirstName = dr["FirstName"].ToString();
                            student.LastName = dr["LastName"].ToString();
                            student.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                        }
                        else
                            return null;
                    }
                    catch (SqlException e)
                    {
                        return null;
                    }
                }
                return student;
            }
        }

        public void setToken(Student s, Guid refreshToken)
        {
            string sqlConn = "Data Source=db-mssql;Initial Catalog = s9817; Integrated Security = True";
            s.RefreshToken = refreshToken.ToString();

            using (var client = new SqlConnection(sqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;

                    client.Open();
                    var tran = client.BeginTransaction();
                    command.Transaction = tran;
                    try
                    {
                        command.CommandText = "update Student2 set refrToken = @token where indexNumber = @index";
                        command.Parameters.AddWithValue("index", s.IndexNumer);
                        command.Parameters.AddWithValue("token", s.RefreshToken);
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException e) { }
                }
            }
        }

        public Student CheckToken(string refToken)
        {
            var student = new Student();
            using (var client = new SqlConnection(sqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;

                    client.Open();
                    var tran = client.BeginTransaction();
                    command.Transaction = tran;
                    try
                    {
                        // czy jest taki token
                        command.CommandText = "select * from Student2 where refrToken = @token";
                        command.Parameters.AddWithValue("token", refToken);

                        var dr = command.ExecuteReader();
                        if (!dr.Read())
                        {
                            return null;
                        }
                        else
                        student.IndexNumer = dr["IndexNumber"].ToString();
                        student.FirstName = dr["FirstName"].ToString();
                        student.LastName = dr["LastName"].ToString();
                        student.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                    }
                    catch (SqlException e)
                    {
                        return null;
                    }
                }
                return student;
            }
        }

        Student2 IStudentDbService.UpdateStudent(Student2 student)
        {
            throw new NotImplementedException();
        }

        Student2 IStudentDbService.DeleteStudent(string indexnr)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Student2> IStudentDbService.GetStudents()
        {
            throw new NotImplementedException();
        }

        IEnumerable<Models.Enrollment> IStudentDbService.GetEnrollment(string indexNumber)
        {
            throw new NotImplementedException();
        }

        bool IStudentDbService.CheckIndex(string index)
        {
            throw new NotImplementedException();
        }

        Student IStudentDbService.CheckPass(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        void IStudentDbService.setToken(Student2 s, Guid refreshToken)
        {
            throw new NotImplementedException();
        }

        Student IStudentDbService.CheckToken(string refToken)
        {
            throw new NotImplementedException();
        }
    }
}
