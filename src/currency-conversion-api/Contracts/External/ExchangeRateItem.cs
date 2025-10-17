using currency_conversion_api.Contracts.External;

public class ExchangeRateItem : IForeignCurrencyItem
{
    public string? effective_date { get; set; }
    public string? exchange_rate { get; set; }
    public string? country_currency_desc { get; set; }
    public string? record_date { get; set; }
}