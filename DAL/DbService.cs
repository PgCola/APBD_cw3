using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Cw3.Models;

namespace Cw3.DAL
{
    public class DbService : IDbService
    {
        public IEnumerable<Student> GetStudents()
        {
            var students = new List<Student>();
            using var con =
                new SqlConnection(
                    "Data Source=localhost\\SQLEXPRESS,1433;Database=master;User Id=sa; Password=Password123++;");
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM dbo.Student";

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    students.Add(st);
                }
                con.Close();
            }
            return students;
        }

        public string GetStudentEnrollment(string studentIndex)
        {
            string returnString = null; 
            using var con =
                            new SqlConnection(
                                "Data Source=localhost\\SQLEXPRESS,1433;Database=master;User Id=sa; Password=Password123++;");
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "SELECT Semester, StartDate, dbo.Studies.Name FROM dbo.Enrollment INNER JOIN dbo.Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE (SELECT IdEnrollment FROM dbo.Student WHERE Student.IndexNumber = @param1)=Enrollment.IdEnrollment";
                com.Parameters.Add("@param1", SqlDbType.VarChar).Value = studentIndex;
                con.Open();
                
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    returnString = "Semestr: " + dr["Semester"].ToString() + "\nData rozpoczecia: " +
                                   dr["StartDate"].ToString() + "\nKierunek: " + dr["Name"].ToString();
                }
            }
            return returnString;
        }
    }
}
