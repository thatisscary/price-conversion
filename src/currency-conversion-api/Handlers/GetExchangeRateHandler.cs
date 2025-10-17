namespace currency_conversion_api.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using currency_conversion_api.Contracts;
    using MediatR;

    public class ExchangeRateRequest  : IRequest<ExchangeRateResponse> { 
    public string CurrencyIdentifier { get; set; }
        public string TransactionDate { get; set; }
    }

    public class GetExchangeRateHandler : IRequestHandler<ExchangeRateRequest, ExchangeRateResponse>

    {
        public Task<ExchangeRateResponse> Handle(ExchangeRateRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
