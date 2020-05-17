using Cw3.Models;

namespace Cw3.Responses
{
    public class EnrollStudentResponse
        {
            public string IndexNumber { get; set; }
            public Enrollment enrollment { get; set; }
            public string message { get; set; }
        }
}