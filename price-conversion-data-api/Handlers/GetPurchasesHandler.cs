namespace price_conversion_data_api.Handlers
{
    using LiteBus.Queries.Abstractions;
    using price_conversion_data_api.Repositories;
    using price_conversion_purchasedb.Entities;

    public sealed record GetPurchasesRequest() : IQuery<List<Purchase>>
    {
    }
    public class GetPurchasesHandler : IQueryHandler<GetPurchasesRequest, List<Purchase>>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ILogger<GetPurchasesHandler> _logger;
        public GetPurchasesHandler(IPurchaseRepository purchaseRepository, ILogger<GetPurchasesHandler> logger)
        {
            _purchaseRepository = purchaseRepository;
            _logger = logger;
        }
        public async Task<List<Purchase>> HandleAsync(GetPurchasesRequest message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling GetPurchasesRequest");
            return await _purchaseRepository.GetAllPurchases();
        }
    
    }
}
