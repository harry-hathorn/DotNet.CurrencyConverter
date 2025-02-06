using Application.Currencies.CompareCurrency.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Currencies.CompareCurrency
{
    public record class ConvertCurrencyQuery(string BaseCurrencyCode, decimal BaseAmount, string TargetCurrencyCode): IRequest<Result<ConvertCurrencyResultDto>>;
}