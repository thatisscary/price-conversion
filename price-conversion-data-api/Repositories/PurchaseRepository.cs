namespace price_conversion_data_api.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using price_conversion_purchasedb;
    using price_conversion_purchasedb.Entities;

    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly PurchaseDbContext _context;

        public PurchaseRepository(PurchaseDbContext context, ILogger<PurchaseRepository> logger)
        {
            _context = context;
        }

        public async Task<Purchase?> GetPurchaseById(Guid id)
        {
            return await _context.Purchases.FindAsync(id);
        }

        public async Task<List<Purchase>> GetAllPurchases()
        {
            return await _context.Purchases.ToListAsync();
        }

        public async Task CreatePurchase(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
        }

    }
}
