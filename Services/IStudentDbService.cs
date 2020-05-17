using Cw3.Requests;
using Cw3.Responses;

namespace Cw3.Services
{
    public interface IStudentDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        
        PromoteStudentResponse PromoteStudents(PromoteStudentRequest request);
    }
}