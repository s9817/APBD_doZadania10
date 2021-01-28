using WebApplication1.dto.request;
using WebApplication1.dto.response;
using WebApplication1.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.db;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentDbService _enrollmentDbService;
        public EnrollmentsController(IEnrollmentDbService enrollmentDbService)
        {
            _enrollmentDbService = enrollmentDbService;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var result = _enrollmentDbService.EnrollStudent(request);
            if (result == null) return BadRequest("Studia nie istnieja");
            return Ok(result);
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]

        public IActionResult PromoteStudents(PromotionRequest request)
        {
            var result = _enrollmentDbService.PromoteStudent(request);
            if (result == null) return NotFound("Nie istnieje wpis na takie studia na takim semestrze.");
            return Ok(result);
        }
    }
}