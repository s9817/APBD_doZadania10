using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.db;
using WebApplication1.Models_EF;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IEnrollmentDbService, EnrollmentDbService>();
            services.AddScoped<IStudentDbService, StudentDbService>();
            services.AddControllers();

            services.AddDbContext<s9817Context>(o =>
            {
                o.UseSqlServer("Data Source=db-mssql;Initial Catalog=s9817;Integrated Security=True");
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidIssuer = "Gakko",
                            ValidAudience = "Students",
                            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
                        };
                    });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentDbService studentDbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<LoggingMiddleware>(); 

            app.Use(async (context, next) => { 
                if (!context.Request.Headers.ContainsKey("Index"))
                { 
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized; 
                    await context.Response.WriteAsync("nie podano indexu w naglowku");
                    return;
                }

                var index = context.Request.Headers["Index"].ToString();
                if (!studentDbService.CheckIndex(index))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized; 
                    await context.Response.WriteAsync("student o takim indeksie nie istnieje");
                    return;
                }


                await next(); 
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
