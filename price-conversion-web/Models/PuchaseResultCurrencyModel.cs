namespace price_conversion_web.Models
{
    using price_conversion_web.Contracts;

    public class PuchaseResultCurrencyModel
    {
        

        public AvailableCurrencies AvailableCurrencies { get; set; } = new AvailableCurrencies();
        public PurchaseResult[] PurchaseResults { get; set; } = Array.Empty<PurchaseResult>();
    }

    
    }
