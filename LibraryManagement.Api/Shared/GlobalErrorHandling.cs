using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Shared
{
    public sealed class GlobalErrorHandling : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<GlobalErrorHandling> _logger;

        public GlobalErrorHandling(
            IProblemDetailsService problemDetailsService,
            IWebHostEnvironment environment,
            ILogger<GlobalErrorHandling> logger)
        {
            _problemDetailsService = problemDetailsService;
            _environment = environment;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            httpContext.Response.StatusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = exception is ValidationException
                    ? "Validation Error"
                    : "Internal Server Error",

                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.",

                Instance = httpContext.Request.Path
            };

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });
        }
    }
}
