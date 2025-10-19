namespace price_conversion_web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc;

    public class PurchaseRequest
    {

        [StringLength(50)]
        [Display(Name="Description")]
        [Required]
        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dddd, dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BindProperty,DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]+(\.[0-9]{0,2})$", ErrorMessage = "Purchase amount must be rounded to the nearest cent.")]
        [BindProperty, DataType(DataType.Currency)]
        public Decimal TotalAmount { get; set; }


    }
}
