using System.Net;
using System.Text.Json;

namespace BankAPI.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError; // 500 default

            // Add more exceptions here as needed
            if (ex is ArgumentException)
                statusCode = HttpStatusCode.BadRequest;
            else if (ex is UnauthorizedAccessException)
                statusCode = HttpStatusCode.Unauthorized;

            var errorResponse = new
            {
                message = ex.Message,
                error = ex.GetType().Name,
                status = (int)statusCode
            };

            var result = JsonSerializer.Serialize(errorResponse);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
