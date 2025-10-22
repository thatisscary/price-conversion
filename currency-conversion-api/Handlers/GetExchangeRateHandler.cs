namespace currency_conversion_api.Handlers
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Services;
    using LiteBus.Queries.Abstractions;

    public class GetExchangeRateRequest : IQuery<OneOf<ExchangeRateResponse, InternalErrorResult>>
    {
        public GetExchangeRateRequest(string currencyIdentifier, DateTime transactionDate)
        {
            CurrencyIdentifier = currencyIdentifier;
            TransactionDate = transactionDate;
        }

        public string CurrencyIdentifier { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class GetExchangeRateHandler : IQueryHandler<GetExchangeRateRequest, OneOf<ExchangeRateResponse, InternalErrorResult>>

    {
        private readonly ForeignCurrencyService _foreignCurrencyService;

        public GetExchangeRateHandler(ForeignCurrencyService foreignCurrencyService)
        {
            _foreignCurrencyService = foreignCurrencyService;
        }

        public async Task<OneOf<ExchangeRateResponse, InternalErrorResult>> HandleAsync(GetExchangeRateRequest request, CancellationToken cancellationToken)
        {
            CurrencyConversionRateRequest rateRequest = new CurrencyConversionRateRequest ( request.CurrencyIdentifier, request.TransactionDate );

            OneOf<Contracts.External.FiscalData<ExchangeRateItem>, InternalErrorResult> response = await _foreignCurrencyService.GetConversionRate(rateRequest);

            return response.Match<OneOf<ExchangeRateResponse, InternalErrorResult>>(
                fiscalData =>
                {
                    return new ExchangeRateResponse(fiscalData.data[0], GetExchangeSymbol(fiscalData?.data[0]?.country));
                },
                internalErrorResult => { return internalErrorResult; }
            );
        }

        private static string GetExchangeSymbol(string? countryName)
        {
            if (string.IsNullOrEmpty(countryName))
            {
                return string.Empty;
            }
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
                .DistinctBy(region => region?.ISOCurrencySymbol) // Distinct by currency code in case of multiples
                .ToList();

            return matchingRegions.FirstOrDefault()?.CurrencySymbol ?? string.Empty;
        }
    }
}