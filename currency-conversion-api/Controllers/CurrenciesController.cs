// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace currency_conversion_api.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Handlers;
    using LiteBus.Queries.Abstractions;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly IQueryMediator _mediator;

        public CurrenciesController(IQueryMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<CurrenciesController>
        [HttpGet]
        [ProducesResponseType(typeof(AvailableCurrencies), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> Get(CancellationToken token)

        {
            CurrencyDescriptionRequest currencyDescriptionRequest = new CurrencyDescriptionRequest();
            OneOf<AvailableCurrencies, InternalErrorResult> result = await _mediator.QueryAsync(currencyDescriptionRequest, token);
            return result.Match<ActionResult>(
                availableCurrencies =>
                {
                    return Ok(availableCurrencies);
                },
                internalErrorResult =>
                {
                    if (internalErrorResult.StatusCode == (int)StatusCodes.Status404NotFound)
                    {
                        return NotFound(new ProblemDetails
                        {
                            Status = internalErrorResult.StatusCode,
                            Title = "No Currencies Retrieved",
                            Detail = internalErrorResult.Message
                        });
                    }
                    else
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Status = internalErrorResult.StatusCode,
                            Title = "Error retrieving available currencies",
                            Detail = internalErrorResult.Message
                        });
                    }
                });
        }

        // GET api/<CurrenciesController>/5
        [HttpGet("ExchangeRate/{currencyCodeIdentifier}/{transactionDate}")]
        [ProducesResponseType(typeof(ExchangeRateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> ExchangeRate([NotNull, FromRoute] string currencyCodeIdentifier, [FromRoute, NotNull] DateTime transactionDate)
        {
            GetExchangeRateRequest request = new GetExchangeRateRequest(currencyCodeIdentifier, transactionDate);
            var result = await _mediator.QueryAsync(request);
            return result.Match<ActionResult>(
                exchangeRateResponse =>
                {
                    return Ok(exchangeRateResponse);
                },
                internalErrorResult =>
                {
                    if (internalErrorResult.StatusCode == (int)StatusCodes.Status404NotFound)
                    {
                        return NotFound(new ProblemDetails
                        {
                            Status = internalErrorResult.StatusCode,
                            Title = "Exchange Rate Not Found",
                            Detail = internalErrorResult.Message
                        });
                    }

                    return BadRequest(new ProblemDetails
                    {
                        Status = internalErrorResult.StatusCode,
                        Title = "Error retrieving exchange rate",
                        Detail = internalErrorResult.Message
                    });
                });
        }
    }
}