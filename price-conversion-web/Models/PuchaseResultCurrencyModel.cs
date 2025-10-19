namespace price_conversion_web.Models
{
    using price_conversion_web.Contracts;

    public class PuchaseResultCurrencyModel
    {
        public AvailableCurrencies[] AvailableCurrencies { get; set; }
        public PurchaseResult[] PurchaseResults { get; set; }
    }

    
    }
