namespace currency_conversion_api.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using currency_conversion_api.Services;
    using MediatR;
    using System.Linq;
    using currency_conversion_api.Contracts.External;
    using currency_conversion_api.Contracts;

    public class CurrencyDescriptionRequest : IRequest<AvailableCurrencies>
    { }


    public class GetCurrencyDescriptionHandler : IRequestHandler<CurrencyDescriptionRequest, AvailableCurrencies>
    {
        private readonly ForeignCurrencyService _foreignCurrencyService;

        public GetCurrencyDescriptionHandler(ForeignCurrencyService foreignCurrencyService )
        {
            _foreignCurrencyService = foreignCurrencyService;
        }

        public async Task<AvailableCurrencies> Handle(CurrencyDescriptionRequest request, CancellationToken cancellationToken)
        {
            FiscalData<ForeignCurrencyDescription> data= await _foreignCurrencyService.GetAvailableCurrencies();

            AvailableCurrencies response = new AvailableCurrencies(data);

            return response;

        }
    }
}
