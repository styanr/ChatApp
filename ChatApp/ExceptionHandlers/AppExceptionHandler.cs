using ChatApp.Exceptions;
using ChatApp.Models;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace ChatApp.ExceptionHandlers
{
    public class AppExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<AppExceptionHandler> _logger;

        public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            (HttpStatusCode statusCode, string message) = exception switch
            {
                UnauthorizedAccessException ex => (HttpStatusCode.Unauthorized, ex.Message),
                NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
                ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
                ContactAlreadyExistsException ex => (HttpStatusCode.Conflict, ex.Message),
                DirectChatRoomAlreadyExists ex => (HttpStatusCode.Conflict, ex.Message),
                InvalidFileException ex => (HttpStatusCode.BadRequest, ex.Message),
                UserNotInChatRoomException ex => (HttpStatusCode.BadRequest, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Internal server error")
            };

            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse(message), cancellationToken);

            return true;
        }
    }
}