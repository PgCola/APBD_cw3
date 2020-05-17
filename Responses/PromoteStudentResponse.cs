using Cw3.Models;

namespace Cw3.Responses
{
    public class PromoteStudentResponse
    {
        public Enrollment enrollment { get; set; }
        public string message { get; set; }
    }
}