using System.Collections.Generic;
using Cw3.Controllers;
using Cw3.Models;

namespace Cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;
        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IdStudent = 1, FirstName = "Jan", LastName = "Kowalski"},
                new Student{IdStudent = 1, FirstName = "Anna", LastName = "Malewska"},
                new Student{IdStudent = 1, FirstName = "Andrzej", LastName = "Andrzejewski"}
            };
        }
        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public string GetStudentEnrollment(string StudentId)
        {
            throw new System.NotImplementedException();
        }
    }
}