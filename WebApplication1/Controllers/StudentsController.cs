using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Models;
using System.Data.SqlClient;
using WebApplication1.db;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.dto.request;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models_EF;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        private readonly IStudentDbService _studentDbService;
        public StudentsController(IStudentDbService studentDbService, IConfiguration configuration)
        {
            _studentDbService = studentDbService;
            Configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "employee")]
        public IActionResult GetStudents()
        {

            return Ok(_studentDbService.GetStudents());
        }

        [HttpPut()]
        public IActionResult UpdateStudent([FromBody] Models_EF.Student2 student)
        {
            Student2 st = _studentDbService.UpdateStudent(student);
            if (st == null)
                return BadRequest();
            else
                return Ok();
        }

        [HttpDelete("{index}")]
        public IActionResult DeleteStudent(string index)
        {
            Student2 st = _studentDbService.DeleteStudent(index);
            if (st == null)
                return BadRequest("Nie ma studenta o takin numerze indexu w bazie");
            else
                return Ok("Usunieto studenta");
        }

        [HttpGet("{IndexNumber}")]
        public IActionResult GetEnrollment(string IndexNumber)
        {
            return Ok(_studentDbService.GetEnrollment(IndexNumber));

        }

        [HttpPut("{id}")]
        public IActionResult EditStudent(int id)
        {
            if (id == 1)
                return Ok("Aktualizacja dokończona");
            else if (id == 2)
                return Ok("Aktualizacja dokończona");
            return NotFound("Nie znaleziono studenta o takim ID");
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpPost("{id}")]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumer = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginRequestDto request)
        {

            Student s = _studentDbService.CheckPass(request.Login, request.Haslo);

            if (s == null)
            {
                return NotFound("Zly login lub haslo");
            }

            var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, s.IndexNumer),
                new Claim(ClaimTypes.Name, s.LastName),
                new Claim(ClaimTypes.GivenName, s.FirstName),
                new Claim(ClaimTypes.Role, "employee")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            var refreshToken = Guid.NewGuid();
            _studentDbService.setToken(s, refreshToken);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken
            });
        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken)
        {
            Student s = _studentDbService.CheckToken(refToken);

            if (s == null)
            {
                return NotFound("Zly refreshToken");
            }

            var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, s.IndexNumer),
                new Claim(ClaimTypes.Name, s.LastName),
                new Claim(ClaimTypes.GivenName, s.FirstName),
                new Claim(ClaimTypes.Role, "employee")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            var refreshToken = Guid.NewGuid();
            _studentDbService.setToken(s, refreshToken);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken
            });
        }
    }
}