namespace currency_conversion_api.Contracts
{
    using currency_conversion_api.Contracts.External;

    public class AvailableCurrencies
    {
        public AvailableCurrencies()
        {
            Currencies = Enumerable.Empty<ForeignCurrency>();
        }

        public AvailableCurrencies(FiscalData<ForeignCurrencyDescription> currency_list)
        {
            Currencies = currency_list.data.Select(x => new ForeignCurrency(x)).ToList();
        }

        public IEnumerable<ForeignCurrency> Currencies { get; set; }
    }
}