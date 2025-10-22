namespace price_conversion_data_api.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using LiteBus.Queries.Abstractions;
    using price_conversion_data_api.Repositories;
    using price_conversion_purchasedb.Entities;

    public sealed record PurchaseByIdRequest(Guid purchaseId) : IQuery<Purchase?>
    {
    }

    public class GetPurchaseByIdHandler : IQueryHandler<PurchaseByIdRequest, Purchase?>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ILogger<GetPurchaseByIdHandler> _logger;

        public GetPurchaseByIdHandler(IPurchaseRepository purchaseRepository, ILogger<GetPurchaseByIdHandler> logger)
        {
            _purchaseRepository = purchaseRepository;
            _logger = logger;
        }

        public async Task<Purchase?> HandleAsync(PurchaseByIdRequest message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling GetPurchaseById for PurchaseId: {PurchaseId}", message.purchaseId);
            return await _purchaseRepository.GetPurchaseById(message.purchaseId);
        }
    }
}