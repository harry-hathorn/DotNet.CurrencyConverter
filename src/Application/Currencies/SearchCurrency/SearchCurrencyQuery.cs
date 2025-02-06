using Application.Currencies.SearchCurrency.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Currencies.SearchCurrency
{
    public record class SearchCurrencyQuery(string CurrencyCode, DateTime StartDate, DateTime EndDate): IRequest<Result<SearchCurrencyDto>>;
}