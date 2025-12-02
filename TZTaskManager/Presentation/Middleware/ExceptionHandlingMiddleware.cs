using System.Net;
using System.Text.Json;
using TZTaskManager.Application.Exceptions;

namespace TZTaskManager.Presentation.Middleware
{
    /// <summary>
    /// Middleware для глобальной обработки исключений
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Произошла ошибка при обработке запроса");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
                case InvalidOperationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
                case KeyNotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
                default:
                    result = JsonSerializer.Serialize(new { error = "Произошла внутренняя ошибка сервера" });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}

