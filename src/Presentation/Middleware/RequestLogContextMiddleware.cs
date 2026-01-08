namespace Presentation.Middleware;

public class RequestLogContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.TraceIdentifier;
        context.Items["CorrelationId"] = correlationId;

        await next(context);
    }
}
