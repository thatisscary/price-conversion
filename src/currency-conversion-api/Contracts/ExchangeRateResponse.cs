namespace currency_conversion_api.Contracts
{
    public class ExchangeRateResponse
    {

        public ExchangeRateResponse()
        {

        }

        public ExchangeRateResponse(ExchangeRateItem item)
        {
            ExchangeRate = Convert.ToDecimal(item.exchange_rate);
            CurrencyIdentifier = item.country_currency_desc;
        }

        public Decimal ExchangeRate { get; set; }
        public string? CurrencyIdentifier { get; set; }

    }
}
