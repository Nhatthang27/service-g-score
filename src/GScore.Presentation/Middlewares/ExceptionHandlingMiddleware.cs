using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json;
using GScore.Application.Exceptions;
namespace GScore.Presentation.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IOptions<JsonOptions> jsonOptions)
{
    private readonly JsonSerializerOptions _json = jsonOptions.Value.SerializerOptions;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                logger.LogWarning(ex, "Response already started, rethrowing.");
                throw;
            }

            context.Response.Clear();
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, apiError) = ResolveException(ex);

        if (statusCode >= 500)
            logger.LogError(ex, "Unexpected exception");
        else
            logger.LogWarning(ex.Message, "Handled exception");

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(apiError, _json);

    }

    private static (int StatusCode, ApiError Error) ResolveException(Exception ex)
    {
        return ex switch
        {
            FluentValidation.ValidationException fvex => (StatusCodes.Status400BadRequest, BuildValidationError(fvex)),
            NotFoundException => (StatusCodes.Status404NotFound, new ApiError("NotFound", ex.Message, null)),
            ConflictException => (StatusCodes.Status409Conflict, new ApiError("Conflict", ex.Message, null)),
            AuthenticationException => (StatusCodes.Status401Unauthorized, new ApiError("Unauthorized", ex.Message, null)),
            _ => (StatusCodes.Status500InternalServerError, new ApiError("ServerError", "An unexpected error occurred.", null))
        };
    }

    private static ApiError BuildValidationError(FluentValidation.ValidationException fvex)
    {
        var details = fvex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

        return new ApiError("ValidationError", "One or more validation errors occurred.", details);
    }
}
