namespace currency_conversion_api.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Contracts.External;
    using currency_conversion_api.Services;
    using LiteBus.Queries.Abstractions;

    public class CurrencyDescriptionRequest : IQuery<OneOf<AvailableCurrencies, InternalErrorResult>>
    { }

    public class GetCurrencyDescriptionHandler : IQueryHandler<CurrencyDescriptionRequest, OneOf<AvailableCurrencies, InternalErrorResult>>
    {
        private readonly ForeignCurrencyService _foreignCurrencyService;

        public GetCurrencyDescriptionHandler(ForeignCurrencyService foreignCurrencyService)
        {
            _foreignCurrencyService = foreignCurrencyService;
        }

        public async Task<OneOf<AvailableCurrencies, InternalErrorResult>> HandleAsync(CurrencyDescriptionRequest request, CancellationToken cancellationToken)
        {
            OneOf<FiscalData<ForeignCurrencyDescription>, InternalErrorResult> data = await _foreignCurrencyService.GetAvailableCurrencies();

            return data.Match<OneOf<AvailableCurrencies, InternalErrorResult>>(
                fiscalData => { return new AvailableCurrencies(fiscalData); },
                internalErrorResult => { return internalErrorResult; }
            );
        }
    }
}