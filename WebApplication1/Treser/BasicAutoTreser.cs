using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebApplication1.db;
using WebApplication1.Models;

namespace WebApplication1.Treser
{
    public class BasicAutoTreser : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IStudentDbService _studentDbService;


        public BasicAutoTreser(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IStudentDbService studentDbService
            ) : base(options, logger, encoder, clock)
        {
            _studentDbService = studentDbService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing authorization header");

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(":");

            if (credentials.Length != 2)
                return AuthenticateResult.Fail("Incorrect authorization header value");

            //TODO check credentials in DB

            Student s = _studentDbService.CheckPass(credentials[0], credentials[1]);

            //
            if (s == null)
            {
                return AuthenticateResult.Fail("Nie ma takiego studenta z takim hasłem");
            }
            else
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, s.IndexNumer),
                new Claim(ClaimTypes.Name, s.LastName),
                new Claim(ClaimTypes.GivenName, s.FirstName),
                new Claim(ClaimTypes.Role, "employee"),
            };

                var identity = new ClaimsIdentity(claims, Scheme.Name); 
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
        }
    }
}
