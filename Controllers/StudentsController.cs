using System;
using System.Collections.Generic;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

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
        public IActionResult GetStudent(string id)
        {
            string returnString = _dbService.GetStudentEnrollment(id);
            if (returnString != null)
            {
                return Ok(returnString);
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