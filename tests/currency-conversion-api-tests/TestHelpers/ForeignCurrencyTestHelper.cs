namespace currency_conversion_api_tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ForeignCurrencyTestHelper
    {
        public static FiscalData<ForeignCurrencyDescription> GetCurrencyList()
        {
            return  new FiscalData<ForeignCurrencyDescription>
            {
                data = new[] { new ForeignCurrencyDescription { country_currency_desc = "Albania-Lek" },
                     new ForeignCurrencyDescription { country_currency_desc= "Moldova-Leu" } ,
                            new ForeignCurrencyDescription { country_currency_desc = "Yemen-Rial" }
                    },
                meta = new Meta { totalcount = 3, totalpages = 3, count = 3 },
            };
        }
    }
}