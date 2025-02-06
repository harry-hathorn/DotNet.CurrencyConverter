using Application.Currencies.ConvertCurrency;
using Application.Currencies.FindLatestCurrency;
using Application.Currencies.SearchCurrency;
using Domain.Common;
using MediatR;

namespace Presentation.Currencies;

public static class CurrencyEndpoints
{
    private const string ServerErrorMessage = "Oops, something went wrong";

    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("currency/latest/{currencyCode}", async (
            string currencyCode,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new FindLatestCurrencyQuery(currencyCode));
            return HandleResult(result);
        })
        .RequireAuthorization(Infrastructure.DependencyInjection.UserPolicy);

        app.MapGet("currency/search/{currencyCode}", async (
            string currencyCode,
            DateTime startDate,
            DateTime endDate,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new SearchCurrencyQuery(currencyCode, startDate, endDate));
            return HandleResult(result);
        })
        .RequireAuthorization(Infrastructure.DependencyInjection.UserPolicy); ;

        app.MapGet("currency/convert/{baseCurrency}/{targetCurrency}/{amount}", async (
           string baseCurrency,
           string targetCurrency,
           decimal amount,
           ISender sender,
           CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ConvertCurrencyQuery(baseCurrency, amount, targetCurrency));
            return HandleResult(result);
        })
        .RequireAuthorization(Infrastructure.DependencyInjection.UserPolicy); ;
    }

    private static IResult HandleResult<T>(Result<T> result)
    {
        if (result.IsFailure && result.Error.Code is ErrorCode.BadInput)
        {
            return Results.Problem(result.Error.Message, statusCode: 400);
        }
        else if (result.IsFailure)
        {
            return Results.Problem(ServerErrorMessage, statusCode: 500);
        }
        return Results.Ok(result.Value);
    }
}
