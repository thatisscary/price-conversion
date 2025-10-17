// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace currency_conversion_api.Controllers
{
    using System.Net.Mime;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Handlers;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrenciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<CurrenciesController>
        [HttpGet]
        [ProducesResponseType(typeof(AvailableCurrencies),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<AvailableCurrencies> Get()

        {
            CurrencyDescriptionRequest currencyDescriptionRequest = new CurrencyDescriptionRequest();
            var result = await _mediator.Send(currencyDescriptionRequest);
            return result; 
        }

        // GET api/<CurrenciesController>/5
        [HttpGet("{CurrencyCodeIdentifier}")]
        
        public string ExchangeRate(string currencyCodeIdentifier, [FromQuery] DateTime transactionDate )
        {
            return "value";
        }

 
    }
}