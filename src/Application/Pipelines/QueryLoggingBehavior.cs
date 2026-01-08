using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Pipelines;

public class QueryLoggingBehavior<TRequest, TResponse>(ILogger<QueryLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query {QueryName}", typeof(TRequest).Name);
        var response = await next();
        logger.LogInformation("Query {QueryName} completed with success: {IsSuccess}", typeof(TRequest).Name, response.IsSuccess);
        return response;
    }
}
