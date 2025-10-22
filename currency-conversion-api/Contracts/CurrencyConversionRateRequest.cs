namespace currency_conversion_api.Contracts
{
    public class CurrencyConversionRateRequest
    {
        public CurrencyConversionRateRequest(string currencyIdentifier, DateTime transactionDate)
        {
            CurrencyIdentifier = currencyIdentifier;
            TransactionDate = transactionDate;
        }

        public string CurrencyIdentifier { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}