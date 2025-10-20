namespace price_conversion_purchasedb.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Purchase
    {
        public Guid PurchaseId { get; set; }
        public required string Description { get; set; }

        public required decimal TotalAmount { get; set; }

        public required DateTime TransactionDate { get; set; }
    }
}
