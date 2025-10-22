namespace price_conversion_web.Controllers
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using price_conversion_web.Contracts;
    using price_conversion_web.Models;
    using price_conversion_web.Services;

    //TODO: Implement logging and error handling.  Also move actions to MetidatR

    public class PurchaseController : Controller
    {
        private readonly CurrencyConversionService _currencyConversionService;
        private readonly PurchaseDataService _purchaseDataService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(CurrencyConversionService currencyConversionService, PurchaseDataService purchaseDataService, ILogger<PurchaseController> _logger)
        {
            _currencyConversionService = currencyConversionService;
            _purchaseDataService = purchaseDataService;
            this._logger = _logger;
        }

        // GET: PurchaseController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> List()
        {
            var purchases = await _purchaseDataService.GetPurchasesDataAsync();

            var availableCurrencies = await _currencyConversionService.GetForeignCurrencies();
            if (availableCurrencies == null)
            {
                _logger.LogWarning("No available currencies retrieved from Currency Conversion Service.");
                availableCurrencies = new AvailableCurrencies { Currencies = new ForeignCurrency[] { new ForeignCurrency { CurrencyIdentifier = "Currency Service Unavailable" } } };
                return View(new PuchaseResultCurrencyModel
                {
                    AvailableCurrencies = availableCurrencies,
                    PurchaseResults = purchases
                });
            }

            var result = new PuchaseResultCurrencyModel()
            {
                AvailableCurrencies = availableCurrencies,
                PurchaseResults = purchases
            };

            return View(result);
        }

        // GET: PurchaseController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PurchaseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PurchaseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PurchaseRequest purchaseRequest)
        {
            try
            {
                var result = await _purchaseDataService.CreatePurchaseAsync(purchaseRequest);
                var purchase = await _purchaseDataService.GetPurchaseByUrlAsync(result.itemLocation);

                return View(nameof(PurchaseResult), purchase);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<JsonResult> ConvertPurchase([FromQuery] Guid purchaseId, [FromQuery] string toCurrency)
        {
            var purchase = await _purchaseDataService.GetPurchaseByIdAsync(purchaseId);

            if (purchase == null)
            {
                var notFoundResult = new { message = $"Purchase with id {purchaseId} not found." };
                return Json(notFoundResult);
            }

            var conversionInfo = await _currencyConversionService.GetConversionRateAsync(toCurrency, purchase.TransactionDate);

            if (conversionInfo != null && !conversionInfo.ConversionFound)
            {
                var failureResult = new { message = conversionInfo.ErrorMessage };
                return Json(failureResult);
            }
            if (conversionInfo != null && conversionInfo.ConversionFound)
            {
                var amount = Math.Round(purchase.TotalAmount * conversionInfo.ExchangeRate, 2);
                var result = new { message = $"{conversionInfo.CurrencySymbol} {amount}" };

                return Json(result);
            }

            var noResult = new { message = $"Error converting {toCurrency}" };
            return Json(noResult);
        }

        //return JsonResult();
    }
}