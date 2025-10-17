namespace currency_conversion_api.Contracts
{
    public class ExchangeRateResponse
    {
        public Decimal ExchangeRate { get; set; }
        public string CurrencyIdentifier { get; set; }

    }
}
