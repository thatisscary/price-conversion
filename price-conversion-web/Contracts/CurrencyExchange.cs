namespace price_conversion_web.Contracts
{
    public class CurrencyExchange
    {
        public Decimal ExchangeRate { get; set; }
        public string? CurrencyIdentifier { get; set; }

        public string? CurrencySymbol { get; set; }
    }
}