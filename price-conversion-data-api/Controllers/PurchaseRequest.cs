namespace price_conversion_data_api.Controllers
{
    using System.ComponentModel.DataAnnotations;

    public class PurchaseRequest
    {
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Maximum two decimal places allowed.")]
        public Decimal totalAmount { get; set; }
        
        [Required]
        public DateTime transactionDate { get; set; }

        [Required]
        public string description { get; set; } = string.Empty;
    }
}