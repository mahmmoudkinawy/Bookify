using Serilog.Context;

namespace Bookify.Api.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            return next(context);
        }
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId);

        return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
    }

}
