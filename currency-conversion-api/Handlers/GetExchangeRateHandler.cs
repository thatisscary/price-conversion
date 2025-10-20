namespace currency_conversion_api.Handlers
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Services;
    using MediatR;

    public class GetExchangeRateRequest : IRequest<ExchangeRateResponse>
    {
        public string CurrencyIdentifier { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class GetExchangeRateHandler : IRequestHandler<GetExchangeRateRequest, ExchangeRateResponse>

    {
        private readonly ForeignCurrencyService _foreignCurrencyService;

        public GetExchangeRateHandler(ForeignCurrencyService foreignCurrencyService)
        {
            _foreignCurrencyService = foreignCurrencyService;
        }

        public async Task<ExchangeRateResponse> Handle(GetExchangeRateRequest request, CancellationToken cancellationToken)
        {
            CurrencyConversionRateRequest rateRequest = new CurrencyConversionRateRequest { CurrencyIdentifier = request.CurrencyIdentifier, TransactionDate = request.TransactionDate };

            var response = await _foreignCurrencyService.GetConversionRate(rateRequest);

            if (response is not null && response.data.Length > 0)
            {
                return new ExchangeRateResponse(response?.data[0], GetExchangeSymbol(response?.data[0].country));
            }

            return new ExchangeRateResponse();
        }

        private static string GetExchangeSymbol(string countryName)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            // Find matching cultures where the region's EnglishName matches the country name (case-insensitive)
            var matchingRegions = cultures
                .Select(culture =>
                {
                    try
                    {
                        return new RegionInfo(culture.Name);
                    }
                    catch
                    {
                        return null; // Skip invalid regions
                    }
                })
                .Where(region => region != null && region.EnglishName.Equals(countryName, StringComparison.OrdinalIgnoreCase))
                .DistinctBy(region => region.ISOCurrencySymbol) // Distinct by currency code in case of multiples
                .ToList();

            return matchingRegions.FirstOrDefault()?.CurrencySymbol ?? string.Empty;
        }
    }
}