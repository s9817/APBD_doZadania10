﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next) { _next = next; }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            var bodyStream = string.Empty;

            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStream = await reader.ReadToEndAsync();
            }
            string logEntry = "Method: " + httpContext.Request.Method.ToString() + "\nPath: "
                + httpContext.Request.Path.ToString() + "\nQuery string: " + httpContext.Request.QueryString.ToString()
                + "\nBody: " + bodyStream + "\n";


            string path = @"Middlewares/requestsLog.txt";

            using (StreamWriter sw = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path))
            {
                sw.WriteLine(logEntry);
            }

            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);
        }
    }
}
