namespace price_conversion_web.Contracts
{
    public class AvailableCurrencies
    {
        public AvailableCurrencies()
        {
            Currencies = Enumerable.Empty<ForeignCurrency>();
        }

        public IEnumerable<ForeignCurrency> Currencies { get; set; }
    }
}