using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using price_conversion_data_api.Handlers;
using price_conversion_purchasedb.Entities;

namespace price_conversion_data_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IQueryMediator _queryMediator;
        private readonly ICommandMediator _commandMediator;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(IQueryMediator queryMediator, ICommandMediator commandMediator, ILogger<PurchasesController> logger)
        {
            _queryMediator = queryMediator;
            _commandMediator = commandMediator;
            _logger = logger;
        }

        // GET: api/Purchases
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Purchase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            try
            {
                var request = new GetPurchasesRequest();
                var purchases = await _queryMediator.QueryAsync(request);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchases");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Service Unavailable",
                    Detail = ex.Message
                });
            }
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Purchase), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<Purchase>> GetPurchase(Guid id)
        {
            try
            {
                var request = new PurchaseByIdRequest(id);
                var purchase = await _queryMediator.QueryAsync(request);
                if (purchase != null)
                {
                    return Ok(purchase);
                }

                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Purchase Not Found",
                    Detail = $"No purchase found with ID: {id}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase with ID: {PurchaseId}", id);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Service Unavailable",
                    Detail = ex.Message
                });
            }
        }


        


        ////POST: api/Purchases
        ////To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> Post(PurchaseRequest purchase)
        {
            var command = new SavePurchaseCommand(purchase.totalAmount, purchase.transactionDate, purchase.description);
            try
            {

                _logger.LogInformation("Creating a new purchase with description: {Description}", purchase.description);
                var result = await _commandMediator.SendAsync(command);
                var url = $"{Request.Scheme}://{Request.Host}{Request.Path}/{result}";
                return Created(url,result);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase {command}", command);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Service Unavailable",
                    Detail = ex.Message
                });
            }

        }


    }
}