namespace currency_conversion_api.Contracts
{
    using currency_conversion_api.Contracts.External;

    public class ForeignCurrency
    {
        public ForeignCurrency(ForeignCurrencyDescription description)
        {
            CurrencyIdentifier = description.country_currency_desc ?? string.Empty;
        }

        public ForeignCurrency()
        {
            CurrencyIdentifier = string.Empty;
        }

        public string CurrencyIdentifier { get; set; }
    }
}