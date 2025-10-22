namespace price_conversion_data_api.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using LiteBus.Commands.Abstractions;
    using price_conversion_data_api.Repositories;
    using price_conversion_purchasedb.Entities;

    public sealed record SavePurchaseCommand(Decimal totalAmount, DateTime transactionDate, string description) : ICommand<Guid>
    {
        public class SavePurchaseCommandHandler : ICommandHandler<SavePurchaseCommand, Guid>
        {
            private readonly IPurchaseRepository _repository;
            private readonly ILogger<SavePurchaseCommandHandler> _logger;

            public SavePurchaseCommandHandler(IPurchaseRepository repository, ILogger<SavePurchaseCommandHandler> logger)
            {
                _repository = repository;
                _logger = logger;
            }

            public async Task<Guid> HandleAsync(SavePurchaseCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    var purchase = new Purchase
                    {
                        TotalAmount = command.totalAmount,
                        TransactionDate = command.transactionDate,
                        Description = command.description
                    };

                    await _repository.CreatePurchase(purchase);
                    _logger.LogInformation(new EventId(0, "SavePurchase"), $"Purchase saved: {purchase.Description}");
                    return purchase.PurchaseId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(1, "SavePurchaseError"), ex, $"Error saving purchase: {ex.Message}");
                    throw;
                }
            }
        }
    }
}