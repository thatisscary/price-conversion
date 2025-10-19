namespace price_conversion_web.Models
{
    public class PurchaseResult : PurchaseRequest
    {
        public PurchaseResult( PurchaseRequest request)
        {
            Description = request.Description;
            TransactionDate = request.TransactionDate;
            TotalAmount = request.TotalAmount;

        }

        public PurchaseResult() { }

        public Guid PurchaseId  => Guid.NewGuid();
    }
}
