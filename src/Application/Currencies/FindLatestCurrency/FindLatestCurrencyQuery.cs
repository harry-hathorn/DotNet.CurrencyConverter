using Application.Currencies.FindLatestCurrency.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Currencies.FindLatestCurrency;

public record FindLatestCurrencyQuery(string CurrencyCode)
    : IRequest<Result<FindLatestCurrencyResultDto>>;
