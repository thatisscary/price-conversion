namespace currency_conversion_api.Contracts
{
    public class CurrencyConversionRateRequest
    {
        public string CurrencyIdentifier { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}