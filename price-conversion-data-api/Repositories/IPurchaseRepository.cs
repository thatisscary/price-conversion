namespace price_conversion_data_api.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using price_conversion_purchasedb.Entities;

    public interface IPurchaseRepository
    {
        Task CreatePurchase(Purchase purchase);
        Task<List<Purchase>> GetAllPurchases();
        Task<Purchase?> GetPurchaseById(Guid id);
    }
}