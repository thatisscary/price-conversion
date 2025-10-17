namespace currency_conversion_api.Contracts
{
    public class CurrencyConversionRateRequest
    {
        public string CurrencyIdentifier { get; set; }

        public string TransactionDate { get; set; }
    }
}