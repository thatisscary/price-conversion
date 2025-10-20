namespace price_conversion_web.Contracts
{
    public class AvailableCurrencies
    {
        public AvailableCurrencies()
        {
            Currencies = Array.Empty<ForeignCurrency>();
        }

        public ForeignCurrency[] Currencies { get; set; }
    }
}