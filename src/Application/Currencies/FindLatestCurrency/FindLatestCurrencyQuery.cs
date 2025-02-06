using Application.Currencies.FindLatestCurrency.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Currencies.FindLatestCurrency
{
    public record class FindLatestCurrencyQuery(string currencyCode): IRequest<Result<FindLatestCurrencyResultDto>>;
}