using System;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentController(IDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }

            return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateResult(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut]
        public IActionResult PutStudent()
        {
            return Ok("Usuwanie zakończone");
        }

        [HttpDelete]
        public IActionResult DeleteStudent()
        {
            return Ok("Usuwanie zakończone");
        }
    }
}