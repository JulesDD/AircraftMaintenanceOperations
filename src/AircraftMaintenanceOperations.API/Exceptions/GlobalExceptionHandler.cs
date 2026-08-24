namespace AircraftMaintenanceOperations.API.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ForbiddenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = exception.Message
                },
                cancellationToken);

            return true;
        }

        logger.LogError(exception, "An unhandled exception occurred.");

        return false;
    }
}
