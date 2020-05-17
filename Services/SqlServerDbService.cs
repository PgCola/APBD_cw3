using System;
using System.Data;
using System.Data.SqlClient;
using Cw3.Models;
using Cw3.Requests;
using Cw3.Responses;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentDbService
    {
        private const string ConStr = "Data Source=localhost\\SQLEXPRESS,1433;Database=master;User Id=sa; Password=Password123++;";

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            var response = new EnrollStudentResponse {IndexNumber = request.IndexNumber};
            var enrollment = new Enrollment();

            using var con = new SqlConnection(ConStr);
            using var com = new SqlCommand {Connection = con};
            con.Open();

            var transaction = con.BeginTransaction();

            try
            {
                com.CommandText = "SELECT IdStudy FROM Studies where Name=@name";
                com.Parameters.AddWithValue("name", request.Studies);
                com.Transaction = transaction;

                var studs = com.ExecuteReader();
                if (!studs.Read())
                {
                    response.message = "Podane studia nie istnieja";
                    return response;
                }

                var idStudies = (int) studs["IdStudy"];
                studs.Close();

                com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdEnrollment >= "
                                  + "(SELECT MAX(IdEnrollment) FROM Enrollment)";
                var executeReader = com.ExecuteReader();
                if (!executeReader.Read())
                {
                }

                var idEnroll = (int) executeReader["IdEnrollment"] + 10;
                executeReader.Close();

                com.CommandText =
                    "SELECT idEnrollment, StartDate from Enrollment WHERE idStudy=@idStudy AND Semester=1" +
                    "ORDER BY StartDate";
                com.Parameters.AddWithValue("idStudy", idStudies);

                DateTime enrollDate;

                var enrol = com.ExecuteReader();
                if (!enrol.Read())
                {
                    response.message = "Brak rozpoczetej rekrutacji";
                    enrollDate = DateTime.Now;
                    com.CommandText = "INSERT INTO Enrollment VALUES(@id, @Semester, @IdStud, @StartDate)";
                    com.Parameters.AddWithValue("id", idEnroll);
                    com.Parameters.AddWithValue("Semester", 1);
                    com.Parameters.AddWithValue("IdStud", idStudies);
                    com.Parameters.AddWithValue("StartDate", enrollDate);
                    enrol.Close();
                    com.ExecuteNonQuery();
                }
                else
                {
                    idEnroll = (int) enrol["IdEnrollment"];
                    enrollDate = (DateTime) enrol["StartDate"];
                    enrol.Close();
                }

                enrollment.IdEnrollment = idEnroll;
                enrollment.Semester = 1;
                enrollment.IdStudy = idStudies;
                enrollment.StartDate = enrollDate;

                response.enrollment = enrollment;

                com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexNum";
                com.Parameters.AddWithValue("indexNum", request.IndexNumber);

                var bDate = DateTime.ParseExact(request.BirthDate, "dd.MM.yyyy", null);
                var formattedDate = bDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                try
                {
                    com.CommandText =
                        "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES " +
                        "(@index, @fName, @lName, @birthDate, @idEnrollment)";

                    com.Parameters.AddWithValue("index", request.IndexNumber);
                    com.Parameters.AddWithValue("fName", request.FirstName);
                    com.Parameters.AddWithValue("lName", request.LastName);
                    com.Parameters.AddWithValue("birthDate", formattedDate);
                    com.Parameters.AddWithValue("idEnrollment", idEnroll);

                    response.message = "DONE";
                    com.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    response.message = "Student o takim ID juz instnieje";
                }
            }
            catch (SqlException exc)
            {
                transaction.Rollback();
                response.message = exc.Message;
            }

            return response;
        }

        public PromoteStudentResponse PromoteStudents(PromoteStudentRequest request)
        {
            var response = new PromoteStudentResponse();

            using var con = new SqlConnection(ConStr);
            using var com = new SqlCommand("Students.dbo.PromoteStudents", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            com.Parameters.AddWithValue("@Studies", request.Studies);
            com.Parameters.AddWithValue("@semester", request.Semester);

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            response.message = "DONE";

            var command = new SqlCommand {Connection = con};
            con.Open();

            var newSem = request.Semester + 1;

            command.CommandText = "SELECT IdStudy FROM Studies where Name=@name";
            command.Parameters.AddWithValue("name", request.Studies);

            var studs = command.ExecuteReader();
            if (!studs.Read())
            {
                response.message = "Podanych studiow nie ma w bazie";
                return response;
            }

            var idStudies = (int) studs["IdStudy"];
            studs.Close();

            var enrollment = new Enrollment();

            command.CommandText = "SELECT idEnrollment,idStudy,Semester,StartDate FROM Enrollment WHERE " +
                                  "idStudy=@idStudy AND Semester=@Semester";
            command.Parameters.AddWithValue("idStudy", idStudies);
            command.Parameters.AddWithValue("Semester", newSem);

            var enr = command.ExecuteReader();
            if (enr.Read())
            {
                enrollment.IdEnrollment = (int) enr["IdEnrollment"];
                enrollment.IdStudy = (int) enr["IdStudy"];
                enrollment.Semester = (int) enr["Semester"];
                enrollment.StartDate = (DateTime) enr["StartDate"];
            }

            enr.Close();
            response.enrollment = enrollment;

            return response;
        }
        
        public Student GetStudent(string index)
        {
            using var con = new SqlConnection(ConStr);
            using var com = new SqlCommand
            {
                Connection = con,
                CommandText = "SELECT IndexNumber, FirstName, LastName, BirthDate, IdEnrollment " +
                              "FROM Student WHERE IndexNumber=@IndexNum"
            };

            com.Parameters.AddWithValue("IndexNum", index);
            con.Open();

            var student = new Student();

            var executeReader = com.ExecuteReader();
            if (!executeReader.Read()) return null;
                
            student.IndexNumber = index;
            student.FirstName = executeReader["FirstName"].ToString();
            student.LastName = executeReader["LastName"].ToString();
            student.BirthDate= DateTime.ParseExact(executeReader["BirthDate"].ToString(), "dd.MM.yyyy", null);
            student.Studies = executeReader["IdEnrollment"].ToString();
            return student;
        }
    }
}