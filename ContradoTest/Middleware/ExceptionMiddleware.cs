using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ContradoTest.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {ErrorMessage}", ex.Message);

              
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

              
                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An error occurred.",
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
