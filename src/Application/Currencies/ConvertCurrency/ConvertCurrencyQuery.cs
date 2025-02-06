using Application.Currencies.ConvertCurrency.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Currencies.ConvertCurrency
{
    public record class ConvertCurrencyQuery(string BaseCurrencyCode, decimal BaseAmount, string TargetCurrencyCode) : IRequest<Result<ConvertCurrencyResultDto>>;
}