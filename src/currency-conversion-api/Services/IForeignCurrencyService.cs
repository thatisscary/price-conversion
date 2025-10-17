namespace currency_conversion_api.Services
{
    using System.Threading.Tasks;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Contracts.External;

    public interface IForeignCurrencyService
    {
        Task<FiscalData<ForeignCurrencyDescription>> GetAvailableCurrencies();
        Task<FiscalData<ExchangeRateItem>> GetConversionRate(CurrencyConversionRateRequest request);
    }
}