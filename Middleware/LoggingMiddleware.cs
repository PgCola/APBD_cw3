using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cw3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                var method = context.Request.Method;
                var queryString = context.Request.QueryString.ToString();
                var bodyString = "";

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyString = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                const string logFile = "StudentsApiLog.txt";

                var loggingText = $"LOG: \nPath: {path}\nMethod: {method}\nQuery: {queryString}\nBody: {bodyString}\n";
                if (File.Exists(logFile))
                {
                    File.AppendAllText(logFile, loggingText);
                }
                else
                {
                    File.WriteAllText(logFile, loggingText);
                }
            }

            if (_next != null) await _next(context);
        }
    }
}