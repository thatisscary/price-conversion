
namespace currency_conversion_api.Contracts.External
{

    public class FiscalData<TContaintedType>
        where TContaintedType : class, IForeignCurrencyItem
    {
        public FiscalData()
        {
          
        }

        public TContaintedType[] data { get; set; }
        public Meta meta { get; set; }
        public Links links { get; set; }
    }

    public class Meta
    {
        public int count { get; set; }
        public int totalcount { get; set; }
        public int totalpages { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
        public string first { get; set; }
        public object prev { get; set; }
        public object next { get; set; }
        public string last { get; set; }
    }

}