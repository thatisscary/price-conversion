namespace currency_conversion_api.Contracts.External
{
    public class FiscalData<TContaintedType>
        where TContaintedType : class, IForeignCurrencyItem
    {
        public FiscalData()
        {
            data = Array.Empty<TContaintedType>();
            meta = new Meta();
        }

        public TContaintedType[] data { get; set; }
        public Meta meta { get; set; }
    }

    public class Meta
    {
        public int count { get; set; }
        public int totalcount { get; set; }
        public int totalpages { get; set; }
    }
}