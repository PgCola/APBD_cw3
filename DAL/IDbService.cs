using System.Collections.Generic;
using Cw3.Models;

namespace Cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();

        public string GetStudentEnrollment(string StudentIndex);
    }
}