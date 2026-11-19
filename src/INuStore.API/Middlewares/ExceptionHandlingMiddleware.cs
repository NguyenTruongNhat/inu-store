using System.Text.Json;
using INuStore.Domain.Exceptions;
using INuStore.UseCase.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace INuStore.API.Middlewares;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var response = new
        {
            title = GetTitle(exception),
            status = statusCode,
            detail = GetDetail(exception, statusCode),
            errors = GetErrors(exception),
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            DbUpdateException => StatusCodes.Status500InternalServerError,
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            InvalidTokenException => StatusCodes.Status401Unauthorized,
            ConflictException => StatusCodes.Status409Conflict,
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            //Application.Exceptions.ValidationException => StatusCodes.Status422UnprocessableEntity,
            //FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            FormatException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            DbUpdateConcurrencyException => "Conflict",
            DomainException applicationException => applicationException.Title,
            _ => "Server Error"
        };

    private static string GetDetail(Exception exception, int statusCode) =>
        exception switch
        {
            DbUpdateConcurrencyException => "This record has changed or no longer exists. Please refresh and try again.",
            DbUpdateException => "Record have been changed by another user. Please refresh and try again.",
            _ when statusCode == StatusCodes.Status500InternalServerError => "Something went wrong. Please try again later.",
            _ => exception.Message
        };

    private static IReadOnlyCollection<ValidationError> GetErrors(Exception exception)
    {
        IReadOnlyCollection<ValidationError>? errors = null;

        if (exception is ValidationException validationException)
        {
            errors = validationException.Errors;
        }

        return errors;
    }

}
