namespace price_conversion_web.Models
{
    public class PurchaseResult
    {
        public Guid PurchaseId { get; set; }
        public string Description { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}