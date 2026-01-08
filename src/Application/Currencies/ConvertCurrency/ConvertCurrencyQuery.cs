using Application.Currencies.ConvertCurrency.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Currencies.ConvertCurrency;

public record ConvertCurrencyQuery(string BaseCurrencyCode, decimal Amount, string TargetCurrencyCode)
    : IRequest<Result<ConvertCurrencyResultDto>>;
