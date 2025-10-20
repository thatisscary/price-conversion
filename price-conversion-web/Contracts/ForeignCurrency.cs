namespace price_conversion_web.Contracts
{
    public class ForeignCurrency
    {
        public ForeignCurrency()
        {
            CurrencyIdentifier = string.Empty;
        }

        public string CurrencyIdentifier { get; set; }
    }
}