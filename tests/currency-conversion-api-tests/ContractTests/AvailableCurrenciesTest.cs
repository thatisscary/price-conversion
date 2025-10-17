namespace currency_conversion_api_tests.ContractTests
{
    
    public class AvailableCurrenciesTest

    {
        [Test]
        public void AvailableCurrencies_ShouldLoadFromExternalContract()
        {
            var externalForeignCurrencies = ForeignCurrencyTestHelper.GetCurrencyList();

            AvailableCurrencies currencies= new AvailableCurrencies( externalForeignCurrencies);

            Assert.That(currencies.Currencies.Count(), Is.EqualTo(3));
            Assert.That(currencies.Currencies.ElementAt(0).CurrencyIdentifier, Is.EqualTo(externalForeignCurrencies.data[0].country_currency_desc));
            Assert.That(currencies.Currencies.ElementAt(1).CurrencyIdentifier, Is.EqualTo(externalForeignCurrencies.data[1].country_currency_desc));
            Assert.That(currencies.Currencies.ElementAt(2).CurrencyIdentifier, Is.EqualTo(externalForeignCurrencies.data[2].country_currency_desc));
        }
    }
}