using Application.Currencies.ConvertCurrency;
using Application.Currencies.ConvertCurrency.Dtos;
using Application.Currencies.FindLatestCurrency;
using Application.Currencies.FindLatestCurrency.Dtos;
using Application.Currencies.SearchCurrency;
using Application.Currencies.SearchCurrency.Dtos;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Currencies;

[ApiController]
[Route("api/v1/currency")]
[Authorize(Policy = "UserOnly")]
public class CurrencyEndpoints(IMediator mediator) : ControllerBase
{
    [HttpGet("convert/{baseCurrencyCode}/{targetCurrencyCode}/{amount}")]
    [ProducesResponseType(typeof(ConvertCurrencyResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Convert(
        string baseCurrencyCode,
        string targetCurrencyCode,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var query = new ConvertCurrencyQuery(baseCurrencyCode, amount, targetCurrencyCode);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == ErrorCode.BadInput)
            {
                return BadRequest(result.Error);
            }
            return StatusCode(500, result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("latest/{currencyCode}")]
    [ProducesResponseType(typeof(FindLatestCurrencyResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FindLatest(
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var query = new FindLatestCurrencyQuery(currencyCode);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == ErrorCode.BadInput)
            {
                return BadRequest(result.Error);
            }
            return StatusCode(500, result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("search/{currencyCode}")]
    [ProducesResponseType(typeof(SearchCurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search(
        string currencyCode,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var query = new SearchCurrencyQuery(currencyCode, startDate, endDate);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == ErrorCode.BadInput)
            {
                return BadRequest(result.Error);
            }
            return StatusCode(500, result.Error);
        }

        return Ok(result.Value);
    }
}
